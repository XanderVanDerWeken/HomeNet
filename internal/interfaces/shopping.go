package interfaces

import "github.com/xandervanderweken/HomeNet/internal/domain"

type ArticleService interface {
	GetAllArticles() ([]*domain.Article, error)

	CreateArticle(article *domain.Article) error
	DeleteArticle(id int64) error
}

type ShoppingService interface {
	GetShoppingList() (*domain.ShoppingList, error)

	UpdateShoppingList(shoppingList *domain.ShoppingList) error
	ClearShoppingList(shoppingList *domain.ShoppingList) error
}

type ArticleRepository interface {
	GetAllArticles() ([]*domain.Article, error)
	GetArticleById(id int64) (*domain.Article, error)

	CreateArticle(article *domain.Article) error
	DeleteArticle(id int64) error
}

type ShoppingRepository interface {
	GetShoppingList() (*domain.ShoppingList, error)

	UpdateShoppingList(shoppingList *domain.ShoppingList) error
	ClearShoppingList(shoppingList *domain.ShoppingList) error
}
