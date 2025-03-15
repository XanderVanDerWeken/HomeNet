package interfaces

import "github.com/xandervanderweken/HomeNet/internal/domain"

type FinanceService interface {
	GetFinances() ([]*domain.FinanceEntry, error)
	GetFinancesofYear(year int64) ([]*domain.FinanceEntry, error)
	GetFinancesOfYearAndMonth(year int64, month int64) ([]*domain.FinanceEntry, error)

	CreateIncome(income *domain.Income) error
	CreateInvoice(invoice *domain.Invoice) error
}

type FinanceRepository interface {
	GetFinances() ([]*domain.FinanceEntry, error)
	GetFinancesOfYear(year int64) ([]*domain.FinanceEntry, error)
	GetFinancesOfYearAndMonth(year int64, month int64) ([]*domain.FinanceEntry, error)

	CreateIncome(income *domain.Income) error
	CreateInvoice(invoice *domain.Invoice) error
}
