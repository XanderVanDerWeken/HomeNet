package domain

type Article struct {
	ID          int64
	Name        string
	Description string
}

type ShoppingItem struct {
	ID       int64
	Quantity uint64
	Article  Article
}

type ShoppingList struct {
	ID    int64
	Items []ShoppingItem
}
