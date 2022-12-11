public class Meeting
{
    static public List<Philosopher> Waiting = new List<Philosopher>();
    static public List<Philosopher> Waiting2 = new List<Philosopher>();
    public static object Waiter = new object();
    public static List<int> WaitingForIt = new List<int>(){0, 0, 0, 0, 0};
    public static List<bool> allowed = new List<bool>(){true, true, true, true, true};  //
    public static Fork[] forks = new Fork[5] { new Fork(), new Fork(), new Fork(), new Fork(), new Fork()}; //used also to monitoring threads
    public static Dictionary<Fork,int> Positions = Positions = new Dictionary<Fork, int>(); //Inverted indexes of forks
    static public Random r = new Random();
    public static void StartMeeintg()
    {
        for (int i = 0; i < 5; i++) Positions[forks[i]] = i;
        Philosopher Socrates = new Philosopher(forks[0], forks[1], "Socrates", r);
        Philosopher Platon = new Philosopher(forks[1], forks[2], "Platon", r);
        Philosopher Seneca = new Philosopher(forks[2], forks[3], "Seneca", r);
        Philosopher Diogenes = new Philosopher(forks[3], forks[4], "Diogenes", r);
        Philosopher Aristoteles = new Philosopher(forks[4], forks[0], "Aristoteles", r);


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
                {
                    Waiting2.AddRange(Waiting);
                    Waiting.Clear();
                }
                allowed = new List<bool>(){true, true, true, true, true};  
                List<Philosopher> toRemove  = new List<Philosopher>();
                foreach (var phil in Waiting2)
                {
                    int l = Meeting.Positions[phil.left];
                    int r = Meeting.Positions[phil.right];
                    if(allowed[l] &&  allowed[r])
                    {
                        if(Monitor.TryEnter(Meeting.forks[l]))
                        {
                            if(Monitor.TryEnter(Meeting.forks[r]))
                            {
                                Monitor.Exit(Meeting.forks[r]);

                                lock(WaitingForIt)
                                {
                                    Meeting.WaitingForIt[l]--;
                                    Meeting.WaitingForIt[r]--;
                                }

                                lock(phil.Ticket)
                                    Monitor.Pulse(phil.Ticket);
                                toRemove.Add(phil);
                            }
                            Monitor.Exit(Meeting.forks[l]);
                        }
                    }
                    allowed[Positions[phil.left]] = false;
                    allowed[Positions[phil.right]] = false;
                }
                foreach (var item in toRemove)
                {
                    Waiting2.Remove(item);
                }
            }
        }
    }
}