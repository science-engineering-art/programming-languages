package main

import(
	"fmt"
	"time"
)

func main() {
	pipeline := make(chan int)

	go func() {
		for {
			time.Sleep(time.Second)
			fmt.Printf("Reading %d.\n", <-pipeline)
		}
	} ()

	for i := 0; i < 10; i++ {
		pipeline <- i
		fmt.Printf("Writing %d.\n", i)
	}
	return
}
