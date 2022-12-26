package main

import(
	"fmt"
	"sync"
	"math/rand"
	"time"
	"strconv"
)

var wg sync.WaitGroup

type Server struct {
	Port80  chan chan string
	Accept  chan int
}

type Client struct {
	Port  chan string
	ID    int
}

// The server is waiting for new clients to join it 
func (s Server) Listening() {
	
	for {
		// Communication was established with the server
		ID := <-s.Accept

		// The server decides whether or not to accept the client
		if GenRandom(2) == 0 {
			s.Accept <- 0 
			continue
		}
		s.Accept <- 1
		
		// The request is accepted and a port to communicate is sent to the client
		port := make(chan string, 1)
		
		s.Port80 <- port

		wg.Add(1)
		go Receive(ID, port)
	}
}

// The client tries to connect to the server
func (c *Client) Connect(s *Server) bool {
	
	// An attempt is made to establish communication with the server
	s.Accept <- c.ID 

	// Checks if the server has enabled a port for communication
	//fmt.Printf("(client%d): checks if the server has enabled a port for communication\n", c.ID)
	accepted := <-s.Accept
	if accepted == 0 {
		return false
	}

	// If the request has been accepted, the enabled port is saved
	c.Port = <-s.Port80

	return true
}

// The server is set to receive information on a specific port
func Receive(ID int, port chan string) {

	for {
		select{
		case r := <- port:
			fmt.Printf("\n(server%d): %s\n", ID, r)

		case <-time.After(time.Duration(40) * time.Millisecond):
			fmt.Printf("\n(server%d): timing out, cut communication\n", ID)

			// send signal kill
			fmt.Printf("\n(server%d): send KILL\n", ID)
			port <- "KILL" 
			wg.Done()
			return
		}
	}
	wg.Done()
}

// The client tries to send information to the server
func (c *Client) Send(s *Server, amount int) {

	// The first thing the client does is to try to connect to the server
	if !c.Connect(s) {
		fmt.Printf("\n(client%d): the server rejected the request\n", c.ID)
		wg.Done()
		return
	}

	fmt.Printf("\n(client%d): starts sending information\n", c.ID)
	i := 0
	pkg := ""

	// Send information
	for ;  i < amount ; i++ {
		pkg = "port" + strconv.Itoa(c.ID) + "--->" + strconv.Itoa(GenRandom(100))

		select {
		case <-time.After(time.Duration(GenRandom(50)) * time.Millisecond):
			c.Port <- pkg
		case <- c.Port:
			fmt.Printf("\n(client%d): interrumpted, %d/%d packages sent\n", 
				c.ID, i, amount)
			wg.Done()
			return
		}
	}

	// Checks if all the information packets have been sent to the server 
	// or if the sending has been interrupted.
	fmt.Printf("\n(client%d): finished, %d/%d packages sent\n", 
		c.ID, i, amount)
	wg.Done()
}

func GenRandom(n int) int {
	s1 := rand.NewSource(time.Now().UnixNano())
	r1 := rand.New(s1)
	return r1.Intn(n)
}

func main() {
	fmt.Println("(server): is created")
	var s Server
	s.Port80  = make(chan chan string, 1)
	s.Accept  = make(chan int, 1)

	go s.Listening()

	// Creates new clients to send information to the server 
	createClient := func(id int, amountOfPkg int) {
		wg.Add(1)

		fmt.Printf("\n(client%d): is created\n", id)
		c := Client{ ID: id }
		
		go c.Send(&s, amountOfPkg)
	}

	// Attempts to send information to the server from 10 different clients
	for i := 0; i < 10; i++ {
		createClient(i, GenRandom(20) + 1)
		time.Sleep(time.Duration(GenRandom(50)) * time.Millisecond)
	}
	wg.Wait()

	fmt.Println("\nTransmissions terminated")
	return
}

