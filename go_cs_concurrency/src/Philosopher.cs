

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
            bool ate = false;                   //Indicates if already ate
            System.Console.WriteLine("{0, -30}{1}", "Pensando ...", name);
            Thread.Sleep(ran.Next(timeMin,timeMaxThink));
            System.Console.WriteLine("{0, -30}{1}", "Queriendo Comer ...", name);
            
            
            Monitor.Enter(Meeting.WaitingForIt);
            if(Meeting.WaitingForIt[l] == 0 && Meeting.WaitingForIt[r] == 0)    //Check if someone in the line is already asking for the forks he need to eat
            {
                if(Monitor.TryEnter(Meeting.forks[l]))  //Check if the left fork is in use
                {
                    if(Monitor.TryEnter(Meeting.forks[r]))  //Check if the right fork is in use
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
            if(!ate)      //If he couldn't skeep the line....
            {
                //He must add himself to the line and indicate that he is requesting the forks.
                Meeting.WaitingForIt[l]++;  
                Meeting.WaitingForIt[r]++;
                Meeting.Waiting.Add(this);
                Monitor.Exit(Meeting.WaitingForIt);
            }
            
            if(!ate)    //If he couldn't skeep the line....
            {
                lock(Ticket)   
                {
                    Monitor.Wait(Ticket);   //Waiting for his turn
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
            lock(Meeting.Waiter)    //Indicate to the waiter that he finished eating
                Monitor.PulseAll(Meeting.Waiter);
        }
    }
}