package services

import (
	"github.com/xandervanderweken/HomeNet/internal/domain"
	"github.com/xandervanderweken/HomeNet/internal/interfaces"
)

type articleService struct {
	repo interfaces.ArticleRepository
}

type shoppingService struct {
	repo interfaces.ShoppingRepository
}

func NewArticleService(repo interfaces.ArticleRepository) interfaces.ArticleService {
	return &articleService{
		repo: repo,
	}
}

func NewShoppingService(repo interfaces.ShoppingRepository) interfaces.ShoppingService {
	return &shoppingService{
		repo: repo,
	}
}

func (as articleService) GetAllArticles() ([]*domain.Article, error) {
	return as.repo.GetAllArticles()
}

func (as articleService) CreateArticle(article *domain.Article) error {
	return as.repo.CreateArticle(article)
}

func (as articleService) DeleteArticle(id int64) error {
	return as.repo.DeleteArticle(id)
}

func (ss shoppingService) GetShoppingList() (*domain.ShoppingList, error) {
	return ss.repo.GetShoppingList()
}

func (ss shoppingService) UpdateShoppingList(shoppingList *domain.ShoppingList) error {
	return ss.repo.UpdateShoppingList(shoppingList)
}

func (ss shoppingService) ClearShoppingList(shoppingList *domain.ShoppingList) error {
	return ss.repo.ClearShoppingList(shoppingList)
}
