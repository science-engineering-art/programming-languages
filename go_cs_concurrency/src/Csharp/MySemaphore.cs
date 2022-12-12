public class MySemaphore
{
    // Reference object for locks
    private static object LockObj = new object();

    // Number of threads that can enter the semaphore currently
    public int Count { get; set; }
    // Maximum number of threads that can enter the semaphore
    public int Capacity { get; set; }
    // Name of the semaphore (nullable)
    public string? Name { get; set; }

    public MySemaphore(int initial_entries, int maximum_entries)
    {
        if (maximum_entries < initial_entries)
            throw new System.Exception("Number of threads in semaphore cannot be greater than its capacity.");

        this.Count = initial_entries;
        this.Capacity = maximum_entries;
    }

    public MySemaphore(int initial_entries, int maximum_entries, string name)
    {
        if (maximum_entries < initial_entries)
            throw new System.Exception("Number of threads in semaphore cannot be greater than its capacity.");

        this.Count = initial_entries;
        this.Capacity = maximum_entries;
        this.Name = name;
    }

    // A thread enters the semaphore
    public void WaitOne()
    {
        lock (LockObj)
        {
            if (this.Count == 0)
            {
                Console.WriteLine("Semaphore is full, waiting...");
                System.Threading.Monitor.Wait(LockObj);
            }
            this.Count--;
        }
    }
    // An N quantity of threads release the semaphore
    public int Release(int N = 1)
    {
        lock (LockObj)
        {
            this.Count += N;
            if (this.Count > this.Capacity)
            {
                this.Count -= N; // If Count > Capacity it stays as it was before the call to Release
                throw new Exception("Semaphore is empty, nothing to release.");
            }
            System.Threading.Monitor.PulseAll(LockObj);
        }
        return this.Count - N;
    }
}


