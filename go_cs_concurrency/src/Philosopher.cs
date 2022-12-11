

public class Fork
{

}

public class Philosopher
{

    public static int timeMin = 3000, timeMaxEat = 5000, timeMaxThink = 8000; 
    Random ran;
    public Fork left, right;
    string name;
    
    public object Ticket;

    public Philosopher(Fork left, Fork right, string name, Random r)
    {
        this.left = left; this.right = right; this.name = name;
        this.ran = r;
        Ticket = new object();
    }

    public void Live()
    {
        int l = Meeting.Positions[left];    //Positions of its corrsponding forks
        int r = Meeting.Positions[right];
        while(true)
        {
            bool ate = false;                   //If already ate
            System.Console.WriteLine("{0, -30}{1}", "Pensando ...", name);
            Thread.Sleep(ran.Next(timeMin,timeMaxThink));
            System.Console.WriteLine("{0, -30}{1}", "Queriendo Comer ...", name);
            
            
            Monitor.Enter(Meeting.WaitingForIt);
            if(Meeting.WaitingForIt[l] == 0 && Meeting.WaitingForIt[r] == 0)
            {
                if(Monitor.TryEnter(Meeting.forks[l]))
                {
                    if(Monitor.TryEnter(Meeting.forks[r]))
                    {   
                        System.Console.WriteLine("Didn't need to wait for the line {0}", name);
                        Monitor.Exit(Meeting.WaitingForIt);
                        Eat();
                        ate = true;
                        Monitor.Exit(Meeting.forks[r]);
                    }
                    Monitor.Exit(Meeting.forks[l]);
                }
            }
            if(!ate)
            {
                Meeting.WaitingForIt[l]++;
                Meeting.WaitingForIt[r]++;
                Meeting.Waiting.Add(this);
                Monitor.Exit(Meeting.WaitingForIt);
            }
            
            if(!ate)
            {
                lock(Ticket)
                {
                    Monitor.Wait(Ticket);
                    System.Console.WriteLine("His turn in the line came {0}", name);
                    Monitor.Enter(Meeting.forks[l]);
                    Monitor.Enter(Meeting.forks[r]);   
                    Eat();
                    Monitor.Exit(Meeting.forks[l]);
                    Monitor.Exit(Meeting.forks[r]);
                }
            }
        }

        void Eat()  //Start to Eat
        {
            Thread.Sleep(10);
            System.Console.WriteLine("{0, -30}{1}", "Comiendo ...", name);
            Thread.Sleep(ran.Next(timeMin,timeMaxEat));
            System.Console.WriteLine("{0, -30}{1}", "Finished eating ...", name);
            lock(Meeting.Waiter)    //Indicate to the waiter it finished eating
                Monitor.PulseAll(Meeting.Waiter);
        }
    }
}