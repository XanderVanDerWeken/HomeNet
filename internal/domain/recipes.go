package domain

import "time"

type Recipe struct {
	ID          int64
	Name        string
	Ingredients []string
	Steps       []string
	CreatedAt   time.Time
}
