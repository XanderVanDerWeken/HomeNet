package services

import (
	"github.com/xandervanderweken/HomeNet/internal/domain"
	"github.com/xandervanderweken/HomeNet/internal/interfaces"
)

type recipeService struct {
	repo interfaces.RecipeRepository
}

func NewRecipeService(repo interfaces.RecipeRepository) interfaces.RecipeService {
	return &recipeService{
		repo: repo,
	}
}

func (rs recipeService) GetAll() ([]*domain.Recipe, error) {
	return rs.repo.GetAll()
}

func (rs recipeService) GetByName(name string) (*domain.Recipe, error) {
	return rs.repo.GetByName(name)
}

func (rs recipeService) Create(recipe domain.Recipe) error {
	return rs.repo.Create(recipe)
}

func (rs recipeService) Delete(id int64) error {
	return rs.repo.Delete(id)
}
