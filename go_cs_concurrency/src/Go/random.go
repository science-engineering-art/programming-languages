package main 

import(
	"fmt"
	"time"
	"math/rand"
)

func main() {
	s1 := rand.NewSource(time.Now().UnixNano())
    r1 := rand.New(s1)
	r1.Intn(100)
}
