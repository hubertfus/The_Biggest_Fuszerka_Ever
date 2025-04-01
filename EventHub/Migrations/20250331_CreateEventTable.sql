CREATE TABLE events (
                        id SERIAL PRIMARY KEY,
                        name VARCHAR(255) NOT NULL,
                        date TIMESTAMP,
                        description TEXT,
                        imageurl VARCHAR(255),
                        organizerid INTEGER,
                        FOREIGN KEY (organizerid) REFERENCES organizers(id)
);