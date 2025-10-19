using BudgetTrack;
using System.Globalization;

namespace BudgetTrack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var manager = new BudgetManager();

            while (true)
            {
                Console.WriteLine("\n=== Personal Budget Tracker ===");
                Console.WriteLine("1) Lägg till transaktion");
                Console.WriteLine("2) Visa alla transaktioner");
                Console.WriteLine("3) Visa total balans");
                Console.WriteLine("4) Avsluta");
                Console.Write("Val: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTransactionFlow(manager);
                        break;

                    case "2":
                        ListTransactions(manager);
                        break;

                    case "3":
                        Console.WriteLine($"Balans: {manager.CalculateBalance():0.00}");
                        break;

                    case "4":
                        Console.WriteLine("Hejdå!");
                        return;

                    default:
                        Console.WriteLine("Ogiltigt val, försök igen.");
                        break;
                }
            }
        }

        static void AddTransactionFlow(BudgetManager manager)
        {
            Console.Write("Beskrivning: ");
            var desc = Console.ReadLine() ?? "";

            Console.Write("Belopp (positivt = inkomst, negativt = utgift): ");
            if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal amount))
            {
                Console.WriteLine("Fel: belopp måste vara ett tal.");
                return;
            }

            Console.Write("Kategori: ");
            var cat = Console.ReadLine() ?? "";

            Console.Write("Datum (YYYY-MM-DD): ");
            var date = Console.ReadLine() ?? "";

            manager.AddTransaction(desc, amount, cat, date);
            Console.WriteLine("✅ Transaktion tillagd!");
        }

        static void ListTransactions(BudgetManager manager)
        {
            var transactions = manager.GetAll();

            if (transactions.Count == 0)
            {
                Console.WriteLine("Inga transaktioner ännu.");
                return;
            }

            Console.WriteLine("\nId  Datum        Kategori      Belopp       Beskrivning");
            Console.WriteLine("---------------------------------------------------------");

            foreach (var t in transactions)
            {
                Console.WriteLine($"{t.Id,2}  {t.Date,-10}  {t.Category,-12}  {t.Amount,10:0.00}  {t.Description}");
            }

            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine($"Saldo: {manager.CalculateBalance():0.00}");
        }
    }
}