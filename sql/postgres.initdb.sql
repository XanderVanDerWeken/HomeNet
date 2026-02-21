BEGIN;

CREATE SCHEMA IF NOT EXISTS persons;

CREATE TABLE IF NOT EXISTS persons.persons (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    alias_name VARCHAR(100) NULL,
    is_inactive BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE SCHEMA IF NOT EXISTS auth;

CREATE TABLE IF NOT EXISTS auth.users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(10) NOT NULL,
    person_id INT REFERENCES persons.persons(id),
    CONSTRAINT role_check CHECK (role IN ('Admin', 'User'))
);

CREATE SCHEMA IF NOT EXISTS cards;

CREATE TABLE IF NOT EXISTS cards.cards (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    expiration_date DATE NOT NULL,
    person_id INT REFERENCES persons.persons(id)
);

CREATE SCHEMA IF NOT EXISTS finances;

COMMIT;