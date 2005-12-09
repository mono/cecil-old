using System;

public class Singleton
{
     private static volatile Singleton instance;
     private static object syncRoot = new Object();

     private Singleton()
     {
     }

     public static Singleton Instance
     {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new Singleton();
                }
            }
            return instance;
        }
    }
}

public class Wrapper {
    public static void Main(string[] args)
    {
    }
}
