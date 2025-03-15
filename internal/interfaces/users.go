package interfaces

import "github.com/xandervanderweken/HomeNet/internal/domain"

type UserService interface {
	GetByUsername(username string) (*domain.User, error)

	SignupUser(user *domain.User) error
	LoginUser(username string, password string) bool
}

type UserRepository interface {
	GetByUsername(username string) (*domain.User, error)
	GetByUsernameAndPassword(username string, password string) (*domain.User, error)

	CreateUser(user *domain.User) error
}
