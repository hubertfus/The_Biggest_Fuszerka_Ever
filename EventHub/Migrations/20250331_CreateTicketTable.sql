CREATE TABLE tickets (
                         id SERIAL PRIMARY KEY,
                         eventid INTEGER NOT NULL,
                         ticketholderid INTEGER NOT NULL,
                         FOREIGN KEY (eventid) REFERENCES events(id),
                         FOREIGN KEY (ticketholderid) REFERENCES people(id)
);