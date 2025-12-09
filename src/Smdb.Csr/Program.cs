namespace Smdb.Csr;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("CWD = " + Directory.GetCurrentDirectory());
        
        App app = new App();
        await app.Start();
    }

    
    
}

