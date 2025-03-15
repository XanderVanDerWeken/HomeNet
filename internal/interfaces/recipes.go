package interfaces

import "github.com/xandervanderweken/HomeNet/internal/domain"

type RecipeService interface {
	GetAll() ([]*domain.Recipe, error)
	GetByName(name string) (*domain.Recipe, error)

	Create(recipe domain.Recipe) error
	Delete(id int64) error
}

type RecipeRepository interface {
	GetAll() ([]*domain.Recipe, error)
	GetByName(name string) (*domain.Recipe, error)
	GetById(id int64) (*domain.Recipe, error)

	Create(recipe domain.Recipe) error
	Delete(id int64) error
}
