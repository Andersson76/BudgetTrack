using BudgetTrack;
using System.Globalization;
using Spectre.Console;

namespace BudgetTrack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var manager = new BudgetManager();

            // Header
            AnsiConsole.Write(
                new FigletText("BudgetTrack")
                    .Centered()
                    .Color(Color.Aqua));

            while (true)
            {
                Console.WriteLine("\n=== Personal Budget Tracker ===");
                Console.WriteLine("1) Lägg till transaktion");
                Console.WriteLine("2) Visa alla transaktioner");
                Console.WriteLine("3) Visa total balans");
                Console.WriteLine("4) Ta bort transaktion");
                Console.WriteLine("5) Avsluta");
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
                        var bal = manager.CalculateBalance();
                        var color = bal >= 0 ? "green" : "red";
                        AnsiConsole.MarkupLine($"Balans: [bold {color}]{bal:0.00}[/]");
                        break;

                    case "4":
                        DeleteFlow(manager);
                        break;

                    case "5":
                        Console.WriteLine("Tack och välkommen åter.");
                        return;

                    default:
                        Console.WriteLine("Ogiltigt val, försök igen.");
                        break;
                }
            }
        }
        static void DeleteFlow(BudgetManager manager)
        {
            var list = manager.GetAll();
            if (list.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Inga transaktioner att ta bort.[/]");
                return;
            }

            // Skapa en lista med Id:n som val
            var pick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj [bold]Id[/] att ta bort:")
                    .AddChoices(list.Select(t => $"{t.Id} – {t.Description} ({t.Amount:0.00})")));

            // Extrahera Id (första delen före '–')
            var idString = pick.Split('–')[0].Trim();

            if (int.TryParse(idString, out int id) && manager.DeleteTransaction(id))
                AnsiConsole.MarkupLine("[green]🗑️ Post borttagen.[/]");
            else
                AnsiConsole.MarkupLine("[red]Hittade ingen post med det Id:t.[/]");
        }

        static void AddTransactionFlow(BudgetManager manager)
        {
            var desc = AnsiConsole.Prompt(
                new TextPrompt<string>("Beskrivning:")
                    .Validate(s => string.IsNullOrWhiteSpace(s)
                        ? ValidationResult.Error("[red]Beskrivning krävs[/]")
                        : ValidationResult.Success()));

            var amount = AnsiConsole.Prompt(
                new TextPrompt<decimal>("Belopp ([green]+[/] = inkomst, [red]-[/] = utgift):")
                    .Validate(a => a == 0
                        ? ValidationResult.Error("[red]Belopp kan inte vara 0[/]")
                        : ValidationResult.Success()));

            var category = AnsiConsole.Prompt(
                new TextPrompt<string>("Kategori (t.ex. Mat, Transport, Hyra, Inkomst):")
                    .DefaultValue("Övrigt"));

            var date = AnsiConsole.Prompt(
                new TextPrompt<string>("Datum (YYYY-MM-DD):")
                    .DefaultValue(DateTime.Now.ToString("yyyy-MM-dd"))
                    .Validate(s =>
                        DateTime.TryParseExact(s, "yyyy-MM-dd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                            ? ValidationResult.Success()
                            : ValidationResult.Error("[red]Fel format. Använd YYYY-MM-DD.[/]")));

            manager.AddTransaction(desc, amount, category, date);
            AnsiConsole.MarkupLine("[green]✅ Transaktion tillagd![/]");
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