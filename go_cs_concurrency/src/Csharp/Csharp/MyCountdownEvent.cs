
public class MyCountdownEvent
{
    // Number of remaining signals to set the event
    private int counter;
    // Initial number of signals needed to set the event
    private int init_counter;
    // Indicates if the number of remaining signals has reached zero
    private bool counter_equals_zero;

    // Reference object for locks
    private static object LockObj = new object(); 

    public MyCountdownEvent(int init_count)
    {
        this.init_counter = init_count;
        this.counter = init_count;

        if (init_count == 0)        
            this.counter_equals_zero = true;
        else
            this.counter_equals_zero = false;
        
    }

    // Propierties (readonly)
    public int CurrentCount => counter;
    public int InitialCount => init_counter;
    public bool IsSet => counter_equals_zero;

    // Increments the CountdownEvent's current count by N.
    public void AddCount(int N = 1)
    {
        lock (LockObj)
            this.counter += N;
    }

    // Decrements the CountdownEvent's current count by N, and indicates weather the event was set or not
    public bool Signal(int N = 1)
    {
        lock (LockObj)
        {
            this.counter -= N;

            if (this.counter == 0)
            {
                this.counter_equals_zero = true;
                Monitor.PulseAll(LockObj);
            }
        }
        return this.counter_equals_zero;
    }

    // Sets the CountdownEvent's current count to N.
    public void Reset(int N = -1)
    {
        lock (LockObj)
        {
            if (N < 0)
                this.counter = this.InitialCount;
            else
                this.counter = N;

            if (this.counter == 0)
            {
                this.counter_equals_zero = true;
                Monitor.PulseAll(LockObj);
            }
        }
    }

    // Blocks the current thread until the CountdownEvent is set
    public void Wait()
    {
        lock (LockObj)
        {
            while (!this.counter_equals_zero)
            {
                Monitor.Wait(LockObj);
            }
        }
    }
}

