partial class TestBarrier
{
    // public static MyBarrier barrier = new MyBarrier(4);
    public static MyBarrier barrier = new MyBarrier(4, (b) =>
        {
            Console.WriteLine("Post-Phase action: count={0}, phase={1}", count, b.CurrentPhaseNumber);
            System.Console.WriteLine("Barrier in {0}",Thread.CurrentThread.Name);
            if (b.CurrentPhaseNumber == 2) throw new Exception("D'oh!");
        });
    // public static Barrier barrier = new Barrier(4, (b) =>
    //     {
    //         Console.WriteLine("Post-Phase action: count={0}, phase={1}", count, b.CurrentPhaseNumber);
    //         System.Console.WriteLine("Barrier in {0}",Thread.CurrentThread.Name);
    //         if (b.CurrentPhaseNumber == 2) throw new Exception("D'oh!");
    //     });
    public static int count;
    public static void Test()
    {

        Thread t1 = new Thread(new ThreadStart(Do));
        Thread t2 = new Thread(new ThreadStart(Do));
        Thread t3 = new Thread(new ThreadStart(Do));
        Thread t4 = new Thread(new ThreadStart(Do));
        t1.Name = "t1";
        t2.Name = "t2";
        t3.Name = "t3";
        t4.Name = "t4";
        t1.Start();t2.Start();t4.Start();t3.Start();
        t1.Join();t2.Join();t3.Join();t4.Join();
        barrier.AddParticipant();
        t1 = new Thread(new ThreadStart(Do));
        t2 = new Thread(new ThreadStart(Do));
        t3 = new Thread(new ThreadStart(Do));
        t4 = new Thread(new ThreadStart(Do));
        Thread t5 = new Thread(new ThreadStart(Do));
        t1.Name = "t1";
        t2.Name = "t2";
        t3.Name = "t3";
        t4.Name = "t4";
        t5.Name = "t5";
        t1.Start();t2.Start(); t3.Start();t4.Start();t5.Start();
        // Thread.Sleep(2);
        // barrier.AddParticipant();
        // System.Console.WriteLine("disposed");
        // barrier.Dispose();
        // t1 = new Thread(Do);
        // t1.Start();


    }
    public static void Do()
    {
        // Thread.Sleep(1);
        // Thread.Sleep(1000);
        // System.Console.WriteLine("Hello from {0}", Thread.CurrentThread.Name);
        Interlocked.Increment(ref count);
        barrier.SignalAndWait(); // during the post-phase action, count should be 4 and phase should be 0
        Interlocked.Increment(ref count);
        // System.Console.WriteLine("Hello 2 from {0}", Thread.CurrentThread.Name);
        barrier.SignalAndWait(); // during the post-phase action, count should be 8 and phase should be 1

        // The third time, SignalAndWait() will throw an exception and all participants will see it
        Interlocked.Increment(ref count);
        try
        {
            barrier.SignalAndWait();
        }
        catch (BarrierPostPhaseException bppe)
        {
            Console.WriteLine("Caught BarrierPostPhaseException: {0}", bppe.Message);
        }

        // The fourth time should be hunky-dory
        Interlocked.Increment(ref count);
        // System.Console.WriteLine("{0} {1}",barrier.CurrentPhaseNumber, Thread.CurrentThread.Name);
        barrier.SignalAndWait(); // during the post-phase action, count should be 16 and phase should be 3
    }
}