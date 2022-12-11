public class Meeting
{
    static public List<Philosopher> Waiting = new List<Philosopher>(); //Line or queue
    static public List<Philosopher> Waiting2 = new List<Philosopher>(); //Queue for locking
    public static object Waiter = new object(); //Object used to monitoring when someone ended eating
    public static List<int> WaitingForIt = new List<int>(){0, 0, 0, 0, 0};  //Indicates how many people is waiting for each fork
    public static List<bool> allowed = new List<bool>(){true, true, true, true, true};  //Used for the waiter to know the priorities.
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
                Monitor.Wait(Waiter);       //Waiter is waiting for someone that will finish eating
                lock(Waiting)   
                {
                    Waiting2.AddRange(Waiting); //move all the line to other line, to iterate for it without locking the original line.
                    Waiting.Clear();
                }
                allowed = new List<bool>(){true, true, true, true, true}; 
                List<Philosopher> toRemove  = new List<Philosopher>();  //To remove from line after finished iterating
                foreach (var phil in Waiting2)
                {
                    int l = Meeting.Positions[phil.left];
                    int r = Meeting.Positions[phil.right];
                    if(allowed[l] &&  allowed[r])   //If no one who came before the phil in the line, needed the same forks as he need, he could take them
                    {
                        if(Monitor.TryEnter(Meeting.forks[l]))  //The phil must also check if none is using them
                        {
                            if(Monitor.TryEnter(Meeting.forks[r]))
                            {
                                
                                Monitor.Exit(Meeting.forks[r]);

                                lock(WaitingForIt)  //The phil is not waiting for the forks any more. He got them!
                                {
                                    Meeting.WaitingForIt[l]--;
                                    Meeting.WaitingForIt[r]--;
                                }

                                lock(phil.Ticket)   //Give the philosopher is ticket (Resume his thread)
                                    Monitor.Pulse(phil.Ticket);
                                toRemove.Add(phil); //Remove the phil from the line
                            }
                            Monitor.Exit(Meeting.forks[l]);
                        }
                    }
                    allowed[Positions[phil.left]] = false;  //The ones who come after the current phil cannot take the forks I need because I am first
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