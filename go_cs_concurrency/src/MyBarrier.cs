public class MyBarrier
{
    Action<MyBarrier>? postAction; //Keeps the post phase action
    int participantCount, numberExceptions, phaseNumber, maxParticipants;   
    static object waitForOthers, waitForExiting;  //Object used for the monitor to change change the executing thread.
    bool ObjectDisposed = false, exiting = false, fullcapacity = false;     //If disposed / If exiting (all the threads launching BarrierPostPhaseException's) / If changing phase
    BarrierPostPhaseException? exceptionPostPhase = null; // Keeps the BarrierPostPhaseException to throw it in every thread (participant)
    public int CurrentPhaseNumber { get => phaseNumber; }
    
    
    public MyBarrier(int maxParticipants, Action<MyBarrier>? postAction = null) //ctor
    {
        this.maxParticipants = maxParticipants;
        this.postAction = postAction;
        phaseNumber = numberExceptions = participantCount = 0;
        waitForOthers = new object();
        waitForExiting = new object();
    }

    void BarrierReachedForAll()  //The barrier was reached by everyone
    {
        fullcapacity = true;
        try //tries to execute the action
        {
            // System.Console.WriteLine("trying from {0}", Thread.CurrentThread.Name);     //debugging
            if(postAction != null)
                postAction(this);
        }
        catch (Exception ex)    //catch the exception if any and stores it
        {
            exceptionPostPhase = new BarrierPostPhaseException(ex);
        }
        phaseNumber++;    
        exiting = true;
        lock (waitForOthers)     //Resume all participants execution
            Monitor.PulseAll(waitForOthers);

    }


    public void SignalAndWait()
    {
        if (ObjectDisposed)
            throw new ObjectDisposedException("The current instance has already been disposed.");
        
        lock(waitForExiting) //Wait if exiting
        {
            // System.Console.WriteLine("Infinite loops from {0}", Thread.CurrentThread.Name);     //debuging
            if(exiting)
                Monitor.Wait(waitForExiting);
        }

        lock (waitForOthers)    
        {

            // System.Console.WriteLine("didnt wait {0}", Thread.CurrentThread.Name);       //degugging
            if (fullcapacity)
            {
                throw new InvalidOperationException("The number of threads using the barrier exceeded the total number of registered participants");
            }
            participantCount++;
            if (participantCount == maxParticipants)
            {
                
                BarrierReachedForAll(); 
            }
            else
                Monitor.Wait(waitForOthers);
            PostPhaseExceptionLauncher();   //Wait for the other participants
        }
    }

    void PostPhaseExceptionLauncher() //If an exception was catch from the post-phase action this throws it in any participant
    {
        if (exceptionPostPhase != null)
        {
            // System.Console.WriteLine("In Excepcion {0}", Thread.CurrentThread.Name);         //debugging
            numberExceptions++; // How many exceptions are already ready to throw
            if (numberExceptions == maxParticipants)    // If all of them...
            {
                exceptionPostPhase = null;  
                numberExceptions = 0;   
                lock (waitForOthers)
                    Monitor.PulseAll(waitForOthers);    //throw them
            }
            else
            {
                lock (waitForOthers)    //ready...
                    Monitor.Wait(waitForOthers);
            }
            // System.Console.WriteLine("was from here {0}", Thread.CurrentThread.Name);       //debugging
            Exiting();
            throw new BarrierPostPhaseException(exceptionPostPhase);
        }
        else
        {
            Exiting();
        }
    }
    void Exiting() 
    {
        exiting = true;
        // System.Console.WriteLine("exiting {0}", Thread.CurrentThread.Name);     //debugging
        participantCount--;
        if(participantCount == 0)
        {
            fullcapacity = false;
            exiting = false;
            lock(waitForExiting)        
                Monitor.PulseAll(waitForExiting); //If something is waiting for the end of the exiting process, this alerts it. 
        }
    }

    public void Dispose()
    {
        if (ObjectDisposed) //check the barrier disposal
            throw new ObjectDisposedException("The current instance has already been disposed.");
        Monitor.Enter(waitForOthers);    //Ensures that no other thread try to do anything while changing the total amount of participants
        ObjectDisposed = true;      
        if(exiting || fullcapacity)  // If already in post-phase throw exception 
        {
            throw new InvalidOperationException("The method was invoked from within a post-phase action");
        }
        Monitor.Exit(waitForOthers);
    }

    public void AddParticipant()
    {
        if (ObjectDisposed) //check the barrier disposal
            throw new ObjectDisposedException("The current instance has already been disposed.");
        
        Monitor.Enter(waitForOthers);   //Ensures that no other thread try to do anything while changing the total amount of participants
        if(exiting || fullcapacity)     // If already in post-phase throw exception 
        {
            throw new InvalidOperationException("The method was invoked from within a post-phase action");
        }
        maxParticipants++;
        Monitor.Exit(waitForOthers);
    }
    public void RemoveParticipant()
    {
        if (ObjectDisposed) //check the barrier disposal
            throw new ObjectDisposedException("The current instance has already been disposed.");
        Monitor.Enter(waitForOthers);     //Ensures that no other thread try to do anything while changing the total amount of participants
        if(exiting || fullcapacity)    // If already in post-phase throw exception 
        {
            throw new InvalidOperationException("The method was invoked from within a post-phase action");
        }
        maxParticipants--;
        Monitor.Exit(waitForOthers);
    }
}