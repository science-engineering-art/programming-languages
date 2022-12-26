package main

import (
	"fmt"
	"sync"
	"time"
)

var wg sync.WaitGroup

type PrivKey struct {
	sync.RWMutex
	PrivKey string
}

func grpc_service(amount int) {
	time.Sleep(time.Duration(amount) * time.Second)
}

func (pvKey *PrivKey) Change(privKey string) {
	
	fmt.Printf("Enter to Change() function with %s as private key...\n\n", privKey)

	pvKey.Lock()
	fmt.Printf("Change() function was blocked for setting %s as private key...\n\n", privKey)
	
	pvKey.PrivKey = privKey
	
	pvKey.Unlock()
	fmt.Printf("Change() function was unblocked and it was setting %s as private key...\n\n", privKey)
}


// Try using Lock and Unlock in this function and analyze what is happening, 
// why is it taking so long? 
func (pvKey *PrivKey) Signature(gRPC_service func(int), serviceId int) {

	defer wg.Done()
	fmt.Printf("(%d): Enter to Signature() function using %s as private key...\n\n", serviceId, pvKey.PrivKey)

	fmt.Printf("(%d): Signature() function was blocked. (%s) \n\n", serviceId, pvKey.PrivKey)
	pvKey.RLock()

	// gRPC service call
	gRPC_service(10)
	fmt.Printf("(%d): Private key signature successfully completed (%s). \n\n", serviceId, pvKey.PrivKey)

	pvKey.RUnlock()
	fmt.Printf("(%d): Signature() function was unblocked. (%s) \n\n", serviceId, pvKey.PrivKey)
}

func main() {

	privKey := PrivKey{PrivKey: "Signature_0"}

	for i := 0; i < 30; i++ {
		wg.Add(1)
		go privKey.Signature(grpc_service, i)
	}

	wg.Add(1)
	go func() {
		defer wg.Done()
		privKey.Change("Signature_1")
	}()

	wg.Add(1)
	go func() {
		defer wg.Done()
		privKey.Change("Signature_2")
	}()

	wg.Wait()

	// Direct access to Password.password
	fmt.Println("Current password value:", privKey.PrivKey)
}
