package main

import(
	"fmt"
	"sync"
	"strconv"
)

var amountOfPersons int
var mutex			sync.Mutex

type Person struct {
	ID   string
	Name string
	Age  int
	Once sync.Once
}

func (p *Person) NewPerson() {
	// default values
	p.Name = "fill in"
	p.Age  = 40

	// Try to remove the mutex here and analyze what happens
	mutex.Lock()
	p.ID = strconv.Itoa(amountOfPersons)
	amountOfPersons++
	mutex.Unlock()

	fmt.Printf("(%s): Initialize person.\n", p.ID)
}

func transferToDb(p *Person) {
	// Initialize a person in order to be able to distinguish them by their ID
	p.Once.Do(p.NewPerson)
	
	// Any operation to the database 
	fmt.Printf("(%s): Transfer to Db.\n", p.ID)
}

func main() {
	var wg sync.WaitGroup

	people := [10]Person{}
	
	for i := 0; i < 100; i++ {
		wg.Add(1)
		go func(j int) {
			defer wg.Done()
			transferToDb(&people[j%10])
		}(i) 
	}
	wg.Wait()

	return
}
