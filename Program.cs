using BudgetTrack;
using System.Globalization;
using Spectre.Console;

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
                        RenderTable(manager);
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

        static void RenderTable(BudgetManager manager)
        {
            var list = manager.GetAll();

            if (list.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Inga transaktioner ännu.[/]");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title("[bold underline]Transaktioner[/]");

            table.AddColumn("Id");
            table.AddColumn("Datum");
            table.AddColumn("Kategori");
            table.AddColumn(new TableColumn("Belopp").RightAligned());
            table.AddColumn("Beskrivning");

            foreach (var t in list)
            {
                // färga inkomster grönt, utgifter rött
                var amountMarkup = t.Amount >= 0
                    ? $"[green]{t.Amount:0.00}[/]"
                    : $"[red]{t.Amount:0.00}[/]";

                table.AddRow(
                    t.Id.ToString(),
                    t.Date,
                    t.Category,
                    amountMarkup,
                    t.Description
                );
            }

            var balance = manager.CalculateBalance();
            var balanceColor = balance >= 0 ? "green" : "red";

            AnsiConsole.Write(table);
            AnsiConsole.Write(new Rule());
            AnsiConsole.MarkupLine($"Saldo: [bold {balanceColor}]{balance:0.00}[/]");
        }
    }
}