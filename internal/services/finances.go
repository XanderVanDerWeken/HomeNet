package services

import (
	"github.com/xandervanderweken/HomeNet/internal/domain"
	"github.com/xandervanderweken/HomeNet/internal/interfaces"
)

type financeService struct {
	repo interfaces.FinanceRepository
}

func NewFinanceService(repo interfaces.FinanceRepository) interfaces.FinanceService {
	return &financeService{
		repo: repo,
	}
}

func (fs *financeService) GetFinances() ([]*domain.FinanceEntry, error) {
	return fs.repo.GetFinances()
}

func (fs *financeService) GetFinancesofYear(year int64) ([]*domain.FinanceEntry, error) {
	return fs.repo.GetFinancesOfYear(year)
}

func (fs *financeService) GetFinancesOfYearAndMonth(year int64, month int64) ([]*domain.FinanceEntry, error) {
	return fs.repo.GetFinancesOfYearAndMonth(year, month)
}

func (fs *financeService) CreateIncome(income *domain.Income) error {
	return fs.repo.CreateIncome(income)
}

func (fs *financeService) CreateInvoice(invoice *domain.Invoice) error {
	return fs.repo.CreateInvoice(invoice)
}
