```sql

CREATE TABLE organizers (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255),
    description TEXT,
    email VARCHAR(255),
    logourl VARCHAR(255)
);

CREATE TABLE events (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    date TIMESTAMP,
    description TEXT,
    imageurl VARCHAR(255),
    organizerid INTEGER,
    FOREIGN KEY (organizerid) REFERENCES organizers(id)
);

CREATE TABLE people (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100),
    email VARCHAR(100),
    persontype VARCHAR(20) NOT NULL DEFAULT 'Standard'
);

CREATE TABLE tickets (
    id SERIAL PRIMARY KEY,
    eventid INTEGER NOT NULL,
    ticketholderid INTEGER NOT NULL,
    FOREIGN KEY (eventid) REFERENCES events(id),
    FOREIGN KEY (ticketholderid) REFERENCES people(id)
);
```
