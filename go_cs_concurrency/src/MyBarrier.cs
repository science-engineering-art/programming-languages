public class MyBarrier
{
    Action<MyBarrier>? postAction; //Keeps the post phase action
    int participantCount, numberExceptions, phaseNumber, maxParticipants;   
    static object waitForOthers = new Object(), waitForExiting = new Object();  //Object used for the monitor to change change the executing thread.
    bool ObjectDisposed = false, exiting = false, fullcapacity = false;     //If disposed / If exiting (all the threads launching BarrierPostPhaseException's) / If changing phase
    BarrierPostPhaseException? exceptionPostPhase = null; // Keeps the BarrierPostPhaseException to throw it in every thread (participant)
    public int CurrentPhaseNumber { get => phaseNumber; }
    
    
    public MyBarrier(int maxParticipants, Action<MyBarrier>? postAction = null) //ctor
    {
        this.maxParticipants = maxParticipants;
        this.postAction = postAction;
        phaseNumber = numberExceptions = participantCount = 0;
    }

    void BarrierReachedForAll()  //The barrier was reached by everyone
    {
        fullcapacity = true;
        try //tries to execute the action
        {
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
            if(exiting)
                Monitor.Wait(waitForExiting);
        }

        lock (waitForOthers)    
        {

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