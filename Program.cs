using BudgetTrack;

namespace BudgetTrack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Testar Transaction klassen ===");

            //Skapa ett objekt av Transaction
            Transaction t1 = new Transaction
            {
                Id = 1,
                Description = "Lön",
                Amount = 25000.0m,
                Category = "Inkomst",
                Date = "2025-10-19"
            };

            // Anropa ShowInfo() för att skriva ut informationen
            t1.ShowInfo();

            Console.WriteLine("\nTryck valfri tangent för att avsluta....");
            Console.ReadKey();
         }
    }
}
