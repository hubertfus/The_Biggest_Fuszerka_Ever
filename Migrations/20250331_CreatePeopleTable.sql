CREATE TABLE people (
                        id SERIAL PRIMARY KEY,
                        name VARCHAR(100),
                        email VARCHAR(100),
                        persontype VARCHAR(20) NOT NULL DEFAULT 'Standard'
);