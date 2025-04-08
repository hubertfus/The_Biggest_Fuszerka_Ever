
# Dokumentacja aplikacji EventHub

## 1. Opis aplikacji
EventHub to aplikacja desktopowa do zarządzania wydarzeniami, zbudowana w technologii WPF (.NET) z wykorzystaniem PostgreSQL jako bazy danych. Główne funkcjonalności:

- 🎭 Zarządzanie wydarzeniami (dodawanie, edycja, usuwanie)
- 👥 Organizacja uczestników (standardowi, VIP, osoby z niepełnosprawnością)
- 🎟 Generowanie i śledzenie biletów
- 🏢 Zarządzanie organizatorami wydarzeń
- 🔌 Konfiguracja połączenia z bazą danych

## 2. Architektura bazy danych


### Tabele

| Tabela     | Kolumny                                | Relacje                                    |
|------------|----------------------------------------|--------------------------------------------|
| Organizers | Id, Name, Email, Description          | 1:N z Events                              |
| Events     | Id, Name, Date, Description           | N:1 z Organizers, 1:N z Tickets           |
| People     | Id, Name, Email, PersonType           | 1:N z Tickets                             |
| Tickets    | Id, EventId, TicketHolderId           | N:1 z Events i People                     |

## 3. Konfiguracja i uruchomienie

### Wymagania systemowe
- .NET 6.0+
- PostgreSQL 12+
- 2 GB wolnego miejsca na dysku

### Instalacja
1. Skonfiguruj plik `.env`:
   ```ini
   DATABASE_URL=Host=localhost;Database=eventhub;Username=postgres;Password=yourpassword
   ```

2. Wykonaj migrację bazy danych:
   ```bash
    $env:DATABASE_URL="Host=localhost;Port=5432;Database=baza;Username=user;Password=password"
    dotnet ef migrations add Init      
    dotnet ef database update
   ```

3. Uruchom aplikację:
   ```bash
   dotnet run
   ```

## 4. Instrukcja obsługi - przypadki brzegowe

### 🔴 Próba usunięcia organizatora z przypisanymi wydarzeniami
1. Przejdź do zakładki "Organizers"
2. Wybierz organizatora z przypisanymi wydarzeniami
3. Kliknij "🗑️ Delete Organizer"
4. System wyświetli ostrzeżenie:  
   "Cannot delete the organizer because they are assigned to one or more events."

### 🔴 Generowanie biletu z niepoprawnym emailem
1. Otwórz szczegóły wydarzenia
2. Kliknij "🎟️ Generate Ticket"
3. Wprowadź niepoprawny email (np. "jan.kowalski")
4. System zablokuje zapis z komunikatem:  
   "Invalid email address. Please use format user@example.com"

### 🔴 Edycja daty wydarzenia na przeszłą
1. Edytuj istniejące wydarzenie
2. Zmień datę na wcześniejszą niż dzisiejsza
3. System pozwoli zapisać, ale:
    - Przycisk generowania biletów zostanie ukryty
    - Wydarzenie oznaczy się szarym kolorem


## 5. Najczęstsze problemy i rozwiązania

| Problem                                | Przyczyna                               | Rozwiązanie                                      |
|----------------------------------------|-----------------------------------------|--------------------------------------------------|
| Błąd połączenia z DB                  | Nieprawidłowe dane w `.env`            | Sprawdź poprawność hosta, nazwy użytkownika i hasła |
| Brak widocznych wydarzeń               | Data wydarzenia jest przeszła          | Użyj filtra "Show past events"                   |
| Błąd przy usuwaniu biletu              | Inna aplikacja blokuje rekord           | Zrestartuj aplikację i spróbuj ponownie           |
