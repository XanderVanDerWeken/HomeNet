package repositories

import (
	"context"
	"log"
	"time"

	"github.com/xandervanderweken/HomeNet/internal/domain"
	"github.com/xandervanderweken/HomeNet/internal/interfaces"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
)

type recipeRepository struct {
	client *mongo.Client
}

func NewRecipeRepository(client *mongo.Client) interfaces.RecipeRepository {
	return &recipeRepository{client: client}
}

const (
	recipesDatabase   = "homenet"
	recipesCollection = "recipes"
)

type RecipeDoc struct {
	ID          int64     `bson:"_id"`
	Name        string    `bson:"name"`
	Ingredients []string  `bson:"ingredients"`
	Steps       []string  `bson:"steps"`
	CreatedAt   time.Time `bson:"created_at"`
}

func (doc RecipeDoc) ToDomain() domain.Recipe {
	return domain.Recipe{
		ID:          doc.ID,
		Name:        doc.Name,
		Ingredients: doc.Ingredients,
		Steps:       doc.Steps,
		CreatedAt:   doc.CreatedAt,
	}
}

func fromRecipe(recipe domain.Recipe) RecipeDoc {
	return RecipeDoc{
		ID:          recipe.ID,
		Name:        recipe.Name,
		Ingredients: recipe.Ingredients,
		Steps:       recipe.Steps,
		CreatedAt:   recipe.CreatedAt,
	}
}

func (repo *recipeRepository) GetAll() ([]*domain.Recipe, error) {
	collection := repo.client.Database(recipesDatabase).Collection(recipesCollection)

	ctx, cancel := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel()

	cursor, err := collection.Find(ctx, bson.D{})
	if err != nil {
		log.Fatal(err)
		return nil, err
	}

	var results []*RecipeDoc
	defer cursor.Close(ctx)
	if err = cursor.All(ctx, &results); err != nil {
		log.Fatal(err)
		return nil, err
	}

	fetchedRecipes := make([]*domain.Recipe, len(results))
	for i, t := range results {
		p := t.ToDomain()
		fetchedRecipes[i] = &p
	}

	return fetchedRecipes, nil
}

func (repo *recipeRepository) GetByName(name string) (*domain.Recipe, error) {
	collection := repo.client.Database(recipesDatabase).Collection(recipesCollection)

	ctx, cancel := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel()

	var result RecipeDoc
	filter := bson.D{{Key: "name", Value: name}}
	err := collection.FindOne(ctx, filter).Decode(&result)

	if err != nil {
		if err == mongo.ErrNoDocuments {
			return nil, nil
		}
		return nil, err
	}

	foundRecipe := result.ToDomain()

	return &foundRecipe, nil
}

func (repo *recipeRepository) GetById(id int64) (*domain.Recipe, error) {
	collection := repo.client.Database(recipesDatabase).Collection(recipesCollection)

	ctx, cancel := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel()

	var result RecipeDoc
	filter := bson.D{{Key: "_id", Value: id}}
	err := collection.FindOne(ctx, filter).Decode(&result)

	if err != nil {
		if err == mongo.ErrNoDocuments {
			return nil, nil
		}
		return nil, err
	}

	foundRecipe := result.ToDomain()

	return &foundRecipe, nil
}

func (repo *recipeRepository) Create(recipe domain.Recipe) error {
	collection := repo.client.Database(recipesDatabase).Collection(recipesCollection)

	ctx, cancel := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel()

	document := fromRecipe(recipe)

	_, err := collection.InsertOne(ctx, document)

	return err
}

func (repo *recipeRepository) Delete(id int64) error {
	collection := repo.client.Database(recipesDatabase).Collection(recipesCollection)

	ctx, cancel := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel()

	filter := bson.D{{Key: "_id", Value: id}}
	_, err := collection.DeleteOne(ctx, filter)

	return err
}
