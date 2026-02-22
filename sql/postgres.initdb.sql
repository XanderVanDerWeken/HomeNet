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

CREATE TABLE IF NOT EXISTS finances.categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS finances.transactions(
    id SERIAL PRIMARY KEY,
    category_id INT REFERENCES finances.categories(id),
    amount DECIMAL(18, 2) NOT NULL CHECK (amount > 0),
    type VARCHAR(10) NOT NULL CHECK (type IN ('Income', 'Expense')),
    source VARCHAR(20) NOT NULL DEFAULT 'Manual' CHECK (source IN ('Manual', 'FixedCost')),
    description TEXT NULL,
    date DATE NOT NULL
);

CREATE INDEX idx_transactions_date ON finances.transactions(date);
CREATE INDEX idx_transactions_category_id ON finances.transactions(category_id);

CREATE TABLE IF NOT EXISTS finances.fixed_costs (
    id SERIAL PRIMARY KEY,
    category_id INT REFERENCES finances.categories(id),
    name VARCHAR(100) NOT NULL,
    day_of_month INT NOT NULL CHECK (day_of_month BETWEEN 1 AND 28),
);

CREATE TABLE IF NOT EXISTS finances.fixed_cost_versions (
    id SERIAL PRIMARY KEY,
    fixed_cost_id INT REFERENCES finances.fixed_costs(id),
    amount DECIMAL(18, 2) NOT NULL CHECK (amount > 0),
    valid_from DATE NOT NULL,
    valid_to DATE NULL,

    CONSTRAINT valid_range CHECK (valid_to IS NULL OR valid_to > valid_from
);

CREATE INDEX idx_fixed_cost_versions_fixed_cost_id ON finances.fixed_cost_versions(fixed_cost_id);
CREATE INDEX idx_fixed_cost_versions_valid_range ON finances.fixed_cost_versions(fixed_cost_id, valid_from, valid_to);

COMMIT;