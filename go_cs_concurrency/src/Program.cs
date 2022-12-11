

class Program
{
    #region MySemaphoreTest
    //private static MySemaphore semaforo = new MySemaphore(3, 3, "libro");
    //private static int count = 5;

    //static void Main(string[] args)
    //{
    //    Console.Clear();
    //    for (int i = 1; i <= 5; i++)
    //    {
    //        Thread hebra = new Thread(new ThreadStart(Lector));
    //        hebra.Name = "Lector[" + i + "]";
    //        hebra.Start();
    //    }
    //}

    //private static void Lector()
    //{
    //    while (count > 0)
    //    {
    //        Leyendo();
    //        count--;
    //    }
    //}

    //private static void Leyendo()
    //{
    //    Console.WriteLine("{0} está esperando por el libro ...", Thread.CurrentThread.Name);
    //    semaforo.WaitOne();

    //    Console.WriteLine("{0} está leyendo ...", Thread.CurrentThread.Name);
    //    Thread.Sleep(2000);

    //    Console.WriteLine("{0} terminó de leer y devuelve el libro.", Thread.CurrentThread.Name);
    //    semaforo.Release();
    //}
    #endregion
   
    #region MyCountdownEventTest
    static void Main(string[] args)
    {
        Meeting.StartMeeintg();
        // MyCountdownEvent countObject = new MyCountdownEvent(10);
        // int[] result = new int[10];


        // for (int i = 0; i < 10; ++i)
        // {
        //     int j = i;
        //     Task.Factory.StartNew(() =>
        //     {
        //         Thread.Sleep(TimeSpan.FromSeconds(3));
        //         result[j] = j * 10;

        //         countObject.Signal();
        //     });
        // }

        // countObject.Wait();

        // foreach (var r in result)
        // {
        //     Console.WriteLine(r);
        // }

        // Console.ReadLine();
    }
    #endregion
}