package services

import (
	"github.com/xandervanderweken/HomeNet/internal/domain"
	"github.com/xandervanderweken/HomeNet/internal/interfaces"
)

type choreService struct {
	repo interfaces.ChoreRepository
}

func NewChoreService(repo interfaces.ChoreRepository) interfaces.ChoreService {
	return &choreService{
		repo: repo,
	}
}

func (cs *choreService) GetAllChores() ([]*domain.Chore, error) {
	return cs.repo.GetAllChores()
}

func (cs *choreService) GetAllChoreByUsername(username string) ([]*domain.Chore, error) {
	return cs.repo.GetAllChoresByUsername(username)
}

func (cs *choreService) CreateOrUpdateChore(chore *domain.Chore) error {
	if chore.ID == 0 {
		return cs.repo.CreateChore(chore)
	}
	return cs.repo.UpdateChore(chore)
}

func (cs *choreService) DeleteChore(id int64) error {
	return cs.repo.DeleteChore(id)
}
