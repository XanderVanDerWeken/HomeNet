BEGIN;

CREATE SCHEMA IF NOT EXISTS persons;

CREATE TABLE IF NOT EXISTS persons.persons (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    alias_name VARCHAR(100) NULL
);

CREATE SCHEMA IF NOT EXISTS cards;

CREATE TABLE IF NOT EXISTS cards.cards (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    expiration_date DATE NOT NULL
);

CREATE SCHEMA IF NOT EXISTS finances;

CREATE TABLE IF NOT EXISTS finances.categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS finances.transactions (
    id SERIAL PRIMARY KEY,
    amount NUMERIC(12, 2) NOT NULL CHECK (amount >= 0),
    year int NOT NULL,
    month int NOT NULL,
    category_id INT REFERENCES finances.categories(id),
    transaction_type int NOT NULL CHECK (transaction_type IN (0, 1)),
    store VARCHAR(255) NULL,
    income_source VARCHAR(255) NULL,

    CONSTRAINT fk_category
        FOREIGN KEY(category_id)
        REFERENCES finances.categories(id),

    CONSTRAINT check_year
        CHECK (year >= 2000 AND year <= 3000),
    
    CONSTRAINT check_month
        CHECK (month >= 1 AND month <= 12)
);

COMMIT;