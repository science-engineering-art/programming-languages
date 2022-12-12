package main

import(
	"sync"
	"fmt"
	"strconv"
	"math/rand"
	"os"
	"time"
)

type Person struct {
	sync.Mutex
	Name  string
	Votes int
}

func main() {
	
	candidates := [3]Person{
		Person { Name: "Roberto Calaz Lantigua" },
		Person { Name: "Jose Maduro Perez" },
		Person { Name: "Ignacio Silva Montero" },
	}

	if len(os.Args) != 2 {
		fmt.Println("Please give me the amount of voters!")
		return
	}

	nVoters, err := strconv.Atoi(os.Args[1])
	if err != nil {
		fmt.Println(err)
		return
	}
	
	var wg sync.WaitGroup
	wg.Add(nVoters)

	for i := 0; i < nVoters; i++ {
		go func() {		
			defer wg.Done()
			
			// generate a random vote
			time.Sleep(time.Duration(1 + rand.Int63n(10)) * time.Microsecond)
			r := rand.Int63n(10) % 3
			
			// voting
			candidates[r].Lock()
			candidates[r].Votes++
			candidates[r].Unlock()
		}()
	}
	
	wg.Wait()

	for i := range candidates {
		fmt.Println(candidates[i].Name, candidates[i].Votes)
	}

	return
}
