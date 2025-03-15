package interfaces

import (
	"github.com/xandervanderweken/HomeNet/internal/domain"
)

type ChoreService interface {
	GetAllChores() ([]*domain.Chore, error)
	GetAllChoreByUsername(username string) ([]*domain.Chore, error)

	CreateOrUpdateChore(chore *domain.Chore) error
	DeleteChore(id int64) error
}

type ChoreRepository interface {
	GetAllChores() ([]*domain.Chore, error)
	GetAllChoresByUsername(username string) ([]*domain.Chore, error)
	GetChoreById(id int64) (*domain.Chore, error)

	CreateChore(chore *domain.Chore) error
	UpdateChore(chore *domain.Chore) error
	DeleteChore(id int64) error
}
