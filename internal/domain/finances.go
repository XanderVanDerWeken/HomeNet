package domain

import "time"

type FinanceEntryType int

const (
	FinanceUnknown FinanceEntryType = iota
	FinanceIncome
	FinanceInvoice
)

var financeEntryName = map[FinanceEntryType]string{
	FinanceUnknown: "Unknown",
	FinanceIncome:  "Income",
	FinanceInvoice: "Invoice",
}

func (ft FinanceEntryType) String() string {
	return financeEntryName[ft]
}

type Income struct {
	ID     int64
	Name   string
	Amount float64
	Date   time.Time
}

type Invoice struct {
	ID     int64
	Name   string
	Amount float64
	Date   time.Time
}

type FinanceEntry struct {
	ID          int64
	Name        string
	Date        time.Time
	FinanceType FinanceEntryType
}
