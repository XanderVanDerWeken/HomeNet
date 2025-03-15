package domain

import "time"

type ChoreRepetition int

const (
	ChoreNever ChoreRepetition = iota
	ChoreDaily
	ChoreWeekly
	ChoreBiWeekly
	ChoreMonthly
)

var choreRepetitionName = map[ChoreRepetition]string{
	ChoreNever:    "Never",
	ChoreDaily:    "Daily",
	ChoreWeekly:   "Weekly",
	ChoreBiWeekly: "Bi-Weekly",
	ChoreMonthly:  "Monthly",
}

func (cr ChoreRepetition) String() string {
	return choreRepetitionName[cr]
}

type Chore struct {
	ID            int64
	Name          string
	Description   string
	Repetition    ChoreRepetition
	LastExecution time.Time
}
