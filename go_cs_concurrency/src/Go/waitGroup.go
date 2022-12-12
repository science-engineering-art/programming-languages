package main

import(
	"fmt"
	"sync"
)

func main() {
	var wg sync.WaitGroup

	for i := 0; i < 10; i++ {
		
		// Add the next Goroutine to the WaitGroup
		wg.Add(1)
		go func(j int)  {

			// When the goroutine is finished, the group is notified 
			// that the task has been successfully executed
			defer wg.Done()
			
			fmt.Printf("Goroutine %d ...\n", j)
		}(i)
	}

	// If this line is commented out it is very likely that no value 
	// will be printed, analyze why. 
	// Hint: race conditions
	wg.Wait()
	
	return
}
