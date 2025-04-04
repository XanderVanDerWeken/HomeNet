package main

import (
	"context"
	"log"
	"time"

	"github.com/xandervanderweken/HomeNet/internal/domain"
	"github.com/xandervanderweken/HomeNet/internal/repositories"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

func main() {

	ctx, cancel := context.WithTimeout(context.Background(), 2*time.Second)
	defer cancel()

	clientOptions := options.Client().ApplyURI("mongodb://127.0.0.1:27017")
	client, err := mongo.Connect(ctx, clientOptions)

	if err != nil {
		log.Panic(err)
	}

	recipe := domain.Recipe{
		ID:          1,
		Name:        "test",
		Ingredients: []string{"test"},
		Steps:       []string{"test"},
		CreatedAt:   time.Now(),
	}

	repo := repositories.NewRecipeRepository(client)
	repo.Create(recipe)

	all, err := repo.GetAll()

	if err != nil {
		log.Panic(err)
	}

	print(len(all))

	print("Hello, World!")
}
