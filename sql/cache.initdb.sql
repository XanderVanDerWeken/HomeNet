CREATE TABLE IF NOT EXISTS monthly_timelines (
    year INTEGER NOT NULL,
    month INTEGER NOT NULL,
    income_amount NUMERIC(12, 2) NOT NULL,
    expense_amount NUMERIC(12, 2) NOT NULL,
    net_total NUMERIC(12, 2) NOT NULL,

    UNIQUE (year, month)
);