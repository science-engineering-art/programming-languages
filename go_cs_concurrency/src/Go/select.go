package main

import(
	"fmt"
	"sync"
	"math/rand"
	"time"
)

var wg sync.WaitGroup

type Server struct {
	Port1 *chan int
	Port2 *chan int
}

type Client struct {
	Port  *chan int
	ID    int
}

func (s *Server) Receive() {
	defer wg.Done()

	fmt.Println("(server): starts receiving information.")
	receiving := true

	for receiving {
		select{
		case r := <- *s.Port1:
			fmt.Printf("\n(server-port1): %d\n", r)

		case r := <- *s.Port2:
			fmt.Printf("\n(server-port2): %d\n", r)

		case <-time.After(time.Duration(40) * time.Millisecond):
			fmt.Println("\n(server-port1/2): timing out, cut communication.")

			// send signal kill
			*s.Port1 <- -1 
			*s.Port2 <- -1

			receiving = false
			break
		}
	}
}

func (c Client) Send(amount int) {
	defer wg.Done()
	fmt.Printf("(client%d): starts sending information.\n", c.ID)
	signal := false
	i := 0

	for ;  i < amount && !signal; i++ {

		select {
		case <-time.After(time.Duration(GenRandom(50)) * time.Millisecond):
			*c.Port <- GenRandom(100)
		
		case tmp := <-*c.Port:
			if tmp == -1 {
				signal = true	
				break
			} else {
				*c.Port <- tmp
			}
		}
	}
	
	if !signal {
		<-*c.Port
		fmt.Printf("\n(client%d): finished, %d/%d packages sent.\n", c.ID, i, amount)
	} else {
		fmt.Printf("\n(client%d): interrumpted, %d/%d packages sent.\n", c.ID, i, amount)
	}
}

func GenRandom(n int) int {
	s1 := rand.NewSource(time.Now().UnixNano())
	r1 := rand.New(s1)
	return r1.Intn(n)
}

func main() {
	fmt.Println("\n(client1): is created.")
	c1 := Client{ ID: 1 }

	fmt.Println("(client2): is created.")
	c2 := Client{ ID: 2 }

	fmt.Println("(server): is created.\n")
	var s Server

	chan1  := make(chan int, 1)
	chan2  := make(chan int, 1)
	c1.Port = &chan1
	c2.Port = &chan2

	fmt.Println("(client1): establishes communication with the server" + 
		", through port 1, and is accepted by the server.")
	s.Port1 = &chan1

	fmt.Println("(client2): establishes communication with the server" + 
		", through port 2, and is accepted by the server.\n")
	s.Port2 = &chan2

	wg.Add(3)

	go c1.Send(10)
	go c2.Send(20)
	go s.Receive()

	wg.Wait()

	fmt.Println("\nTransmissions terminated.")
	return
}
