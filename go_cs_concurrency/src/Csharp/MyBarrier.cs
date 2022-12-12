public class MyBarrier
{
    //Keeps the post phase action
    Action<MyBarrier>? postAction; 
    int participantCount, numberExceptions, phaseNumber, maxParticipants;
    //Object used for the monitor to change change the executing thread.
    static object waitForOthers = new Object(), waitForExiting = new Object();
    //If disposed / If exiting (all the threads launching BarrierPostPhaseException's) / If changing phase
    bool ObjectDisposed = false, exiting = false, fullcapacity = false;
    // Keeps the BarrierPostPhaseException to throw it in every thread (participant)
    BarrierPostPhaseException? exceptionPostPhase = null; 
    public int CurrentPhaseNumber { get => phaseNumber; }
    
    
    public MyBarrier(int maxParticipants, Action<MyBarrier>? postAction = null) //ctor
    {
        if(maxParticipants < 0 || maxParticipants > 32767)
            throw new ArgumentOutOfRangeException();
        this.maxParticipants = maxParticipants;
        this.postAction = postAction;
        phaseNumber = numberExceptions = participantCount = 0;
    }

    //The barrier was reached by everyone
    void BarrierReachedForAll()  
    {
        fullcapacity = true;
        //tries to execute the action
        try
        {
            if(postAction != null)
                postAction(this);
        }
        //catch the exception if any and stores it
        catch (Exception ex)    
        {
            exceptionPostPhase = new BarrierPostPhaseException(ex);
        }
        phaseNumber++;    
        exiting = true;
        //Resume all participants execution
        lock (waitForOthers)     
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
            //Wait for the other participants
            PostPhaseExceptionLauncher();   
        }
    }

    //If an exception was catch from the post-phase action this throws it in any participant
    void PostPhaseExceptionLauncher() 
    {
        if (exceptionPostPhase != null)
        {
            // How many exceptions are already ready to throw
            numberExceptions++;
            // If all of them...
            if (numberExceptions == maxParticipants)    
            {
                exceptionPostPhase = null;  
                numberExceptions = 0;   
                lock (waitForOthers)
                    //throw them
                    Monitor.PulseAll(waitForOthers);    
            }
            else
            {
                //ready...
                lock (waitForOthers)    
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
                //If something is waiting for the end of the exiting process, this alerts it. 
                Monitor.PulseAll(waitForExiting); 
        }
    }

    public void Dispose()
    {
        //check the barrier disposal
        if (ObjectDisposed) 
            throw new ObjectDisposedException("The current instance has already been disposed.");
        //Ensures that no other thread try to do anything while changing the total amount of participants
        Monitor.Enter(waitForOthers);    
        ObjectDisposed = true;
        // If already in post-phase throw exception
        if (exiting || fullcapacity)   
        {
            throw new InvalidOperationException("The method was invoked from within a post-phase action");
        }
        Monitor.Exit(waitForOthers);
    }

    public void AddParticipant()
    {
        if (ObjectDisposed) //check the barrier disposal
            throw new ObjectDisposedException("The current instance has already been disposed.");

        //Ensures that no other thread try to do anything while changing the total amount of participants
        Monitor.Enter(waitForOthers);
        // If already in post-phase throw exception 
        if (exiting || fullcapacity)     
        {
            throw new InvalidOperationException("The method was invoked from within a post-phase action");
        }
        maxParticipants++;
        Monitor.Exit(waitForOthers);
    }
    public void RemoveParticipant()
    {
        //check the barrier disposal
        if (ObjectDisposed) 
            throw new ObjectDisposedException("The current instance has already been disposed.");
        //Ensures that no other thread try to do anything while changing the total amount of participants
        Monitor.Enter(waitForOthers);
        // If already in post-phase throw exception
        if (exiting || fullcapacity)     
        {
            throw new InvalidOperationException("The method was invoked from within a post-phase action");
        }
        maxParticipants--;
        Monitor.Exit(waitForOthers);
    }
}