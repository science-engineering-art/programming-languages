

public class Fork
{

}

public class Philosopher
{

    public static int timeMin = 3000, timeMaxEat = 5000, timeMaxThink = 8000; 
    Random r;
    public Fork left, right;
    string name;
    
    public object Ticket;

    public Philosopher(Fork left, Fork right, string name, Random r)
    {
        this.left = left; this.right = right; this.name = name;
        this.r = r;
        Ticket = new object();
    }

    bool TryTakeFork()
    {
        lock(Program2.notInUse)
        {
            lock(Program2.noWaitingForIt)
            {
                int l = Program2.Positions[left];
                int r = Program2.Positions[right];
                if(Program2.noWaitingForIt[l] && Program2.noWaitingForIt[r])
                {
                    return true;
                }
                else
                {
                    Program2.noWaitingForIt[l] = false;
                    Program2.noWaitingForIt[r] = false;
                    return false;
                }
            }
        }
    }

    public void Live()
    {
        bool ate = false;
        while(true)
        {
            ate = true;
            System.Console.WriteLine("{0, -30}{1}", "Pensando ...", name);
            Thread.Sleep(r.Next(timeMin,timeMaxThink));
            System.Console.WriteLine("{0, -30}{1}", "Queriendo Comer ...", name);
            
            
            lock(Program2.noWaitingForIt)
            {
                int l = Program2.Positions[left];
                int r = Program2.Positions[right];
                if(Program2.noWaitingForIt[l] && Program2.noWaitingForIt[r])
                {
                    if(Monitor.TryEnter(Program2.notInUse[l]))
                    {
                        if(Monitor.TryEnter(Program2.notInUse[r]))
                        {
                            Eat();
                            Monitor.Exit(Program2.notInUse[r]);
                            ate = true;
                        }
                        Monitor.Exit(Program2.notInUse[l]);
                    }
                }
                else
                {
                    Program2.noWaitingForIt[l] = false;
                    Program2.noWaitingForIt[r] = false;
                    Program2.Waiting.Add(this);
                }
            }
            if(!ate)
            {
                lock(Ticket)
                {
                    Monitor.Wait(Ticket);
                    Eat();
                }
            }
        }

        void Eat()
        {
            Monitor.Enter(Program2.notInUse[Program2.Positions[left]]);
            Monitor.Enter(Program2.notInUse[Program2.Positions[right]]);
            Thread.Sleep(10);
            System.Console.WriteLine("{0, -30}{1}", "Comiendo ...", name);
            Thread.Sleep(r.Next(timeMin,timeMaxEat));
            Monitor.Exit(Program2.notInUse[Program2.Positions[left]]);
            Monitor.Exit(Program2.notInUse[Program2.Positions[right]]);
            lock(Program2.Waiter)
                Monitor.PulseAll(Program2.Waiter);
        }
    }

}