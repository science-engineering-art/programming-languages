package main

import(
	"fmt"
	"time"
	"sync"
)

func main() {
	pipeline := make(chan int, 5)
	var wg sync.WaitGroup

	go func() {
		for {
			time.Sleep(time.Second)
			fmt.Printf("Reading %d.\n", <-pipeline)
			wg.Done()
		}
	} ()

	for i := 0; i < 10; i++ {
		wg.Add(1)
		pipeline <- i
		fmt.Printf("Writing %d.\n", i)
	}
	wg.Wait()
	
	return
}
