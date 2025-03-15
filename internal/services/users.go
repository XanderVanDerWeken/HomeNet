package services

import (
	"github.com/xandervanderweken/HomeNet/internal/domain"
	"github.com/xandervanderweken/HomeNet/internal/interfaces"
)

type userService struct {
	repo interfaces.UserRepository
}

func NewUserService(repo interfaces.UserRepository) interfaces.UserService {
	return &userService{
		repo: repo,
	}
}

func (us userService) GetByUsername(username string) (*domain.User, error) {
	return us.repo.GetByUsername(username)
}

func (us userService) SignupUser(user *domain.User) error {
	return us.repo.CreateUser(user)
}

func (us userService) LoginUser(username string, password string) bool {
	fetchedUser, err := us.repo.GetByUsernameAndPassword(username, password)

	if fetchedUser == nil || err != nil {
		return false
	}

	return true
}
