package main

import(
	"sync"
	"fmt"
	"os"
	"time"
	"math/rand"
	"strconv"
)

type Fork chan bool

type Philosopher struct {
	Name  string	
	Left  *Fork
	Right *Fork
}

func (phil *Philosopher) Eat() {
	fmt.Printf("%s began to eat.\n", phil.Name)
	time.Sleep(time.Duration(1 + rand.Int63n(10)) * time.Second)
}

func (phil *Philosopher) Think() {
	fmt.Printf("%s is thinking.\n", phil.Name)
	time.Sleep(time.Duration(1 + rand.Int63n(10)) * time.Second)
}

func (phil *Philosopher) GetForks() {
	getLeft, getRight := false, false

	for {
		select{
		case <- *phil.Left:
			getLeft = true
		case <- *phil.Right:
			getRight = true
		case <-time.After(time.Duration(1 + rand.Int63n(10)) * time.Second):
			if getLeft {
				*phil.Left	<- true
				getLeft = false
			} 
			if getRight {
				*phil.Right <- true
				getRight = false
			}
			phil.Think()
		}
		if getLeft && getRight {
			break
		}
	}	
}

func (phil *Philosopher) DropForks() {
	fmt.Printf("%s finished eating.\n", phil.Name)
	*phil.Left  <- true
	*phil.Right <- true
}

func main() {

	// get the amount of Philosophers
	if len(os.Args) != 2 {
		fmt.Println("Enter the amount of Philosophers.")
		return
	}
	nPhil, err := strconv.Atoi(os.Args[1])

	if err != nil {
		fmt.Println("ERROR:", err)
		return
	}

	// create and initialize the Forks 
	forks := make([]Fork, nPhil)
	
	for i := 0; i < nPhil; i++ {
		forks[i] =  make(chan bool, 1)
		forks[i] <- true
	}

	// create and initialize the Philosophers
	phils := make([]Philosopher, nPhil)

	for i := 0; i < nPhil; i++ {
		phils[i].Name  = "Philosopher_" + strconv.Itoa(i + 1)
		phils[i].Left  = &forks[i]
		phils[i].Right = &forks[(i + 1) % nPhil] 
	}

	var wg sync.WaitGroup

	//for j := 0; true; j++ {
	//	
	//	fmt.Printf("\n========== ROUND %d ==========\n\n", j + 1)
	//
	//	wg.Add(nPhil)
	//	for i := range phils {
	//		go func(i int){
	//			defer wg.Done()	
	//			phils[i].Think()
	//			phils[i].GetForks()
	//			phils[i].Eat()
	// 			phils[i].DropForks()
	// 		}(i)
	// 	}
	//	wg.Wait()
	//}

	//wg.Add(nPhil)
	//for i := range phils {
	//	go func(i int){
	//		for {
	//			defer wg.Done()	
	//			phils[i].Think()
	//			phils[i].GetForks()
	//			phils[i].Eat()
	//			phils[i].DropForks()
	// 		}
	//	}(i)
	//}
	//wg.Wait()

	wg.Add(nPhil)

	for i := range phils {

		go func(phil *Philosopher) {
			defer wg.Done()
			var waitGroup sync.WaitGroup
			
			for {
				waitGroup.Add(1)
				go func() {
					defer waitGroup.Done()
					phil.Think()
					phil.GetForks()
					phil.Eat()
					phil.DropForks()
				} ()
				waitGroup.Wait()
			}
		}(&phils[i])
	}

	wg.Wait()

	return
}
