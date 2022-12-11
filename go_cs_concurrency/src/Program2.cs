public class Program2
{
    object WaiterControl = new object(), WaiterAction = new object();

    static public List<Philosopher> Waiting;
    static public List<Philosopher> Waiting2;
    public static object Waiter = new object();
    public static List<bool> noWaitingForIt = new List<bool>(){true, true, true, true, true};
    public static List<bool> allowed = new List<bool>(){true, true, true, true, true};
    public static List<object> notInUse = new List<object>(){new object(), new object(), new object(), new object(), new object()};
    public static Dictionary<Fork,int> Positions;
    void WaiterMethod()
    {
        
    }
    static public Random r = new Random();
    public static void Main2()
    {
        Fork[] forks = new Fork[5] { new Fork(), new Fork(), new Fork(), new Fork(), new Fork()};
        Positions = new Dictionary<Fork, int>();
        Positions[forks[0]] = 0;
        Positions[forks[1]] = 1;
        Positions[forks[2]] = 2;
        Positions[forks[3]] = 3;
        Positions[forks[4]] = 4;
        Philosopher Socrates = new Philosopher(forks[0], forks[1], "Socrates", r);
        Philosopher Platon = new Philosopher(forks[1], forks[2], "Platon", r);
        Philosopher Seneca = new Philosopher(forks[2], forks[3], "Seneca", r);
        Philosopher Diogenes = new Philosopher(forks[3], forks[4], "Diogenes", r);
        Philosopher Aristoteles = new Philosopher(forks[4], forks[0], "Aristoteles", r);


        Waiting = new List<Philosopher>();
        Waiting2 = new List<Philosopher>();
        
        new Thread(Socrates.Live).Start();
        new Thread(Platon.Live).Start();
        new Thread(Seneca.Live).Start();
        new Thread(Diogenes.Live).Start();
        new Thread(Aristoteles.Live).Start();

        while(true)
        {
            lock(Waiter)
            {
                Monitor.Wait(Waiter);
                lock(Waiting)
                    Waiting2.AddRange(Waiting);

                foreach (var phil in Waiting2)
                {
                    if(allowed[Positions[phil.left]] &&  allowed[Positions[phil.right]])
                    {
                        if(Monitor.TryEnter(notInUse[Positions[phil.left]]))
                        {
                            if(Monitor.TryEnter(notInUse[Positions[phil.right]]))
                            {
                                Monitor.Exit(notInUse[Positions[phil.left]]);
                                Monitor.Exit(notInUse[Positions[phil.right]]);

                                lock(phil.Ticket)
                                    Monitor.Pulse(phil.Ticket);
                                Waiting2.Remove(phil);
                            }
                            Monitor.Exit(notInUse[Positions[phil.left]]);
                        }
                    }
                    else
                    {
                        allowed[Positions[phil.left]] = false;
                        allowed[Positions[phil.right]] = false;
                    }
                }
            }
        }
    }
}