using BudgetTrack;
using System.Globalization;
using Spectre.Console;
using System.Linq;

namespace BudgetTrack
{
    internal class Program
    {
        enum MenuChoice
        {
            Add, List, Balance, Delete, FilterByCategory, Stats, Exit
        }

        static MenuChoice AskMenu()
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]Vad vill du göra?[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Aqua, decoration: Decoration.Bold))
                    .AddChoices(new[]
                    {
                "➕ Lägg till transaktion",
                "📋 Visa alla transaktioner",
                "💰 Visa total balans",
                "🗑️ Ta bort transaktion",
                "🔎 Filtrera per kategori",
                "📊 Visa statistik",
                "❌ Avsluta"
                    }));

            return choice switch
            {
                "➕ Lägg till transaktion" => MenuChoice.Add,
                "📋 Visa alla transaktioner" => MenuChoice.List,
                "💰 Visa total balans" => MenuChoice.Balance,
                "🗑️ Ta bort transaktion" => MenuChoice.Delete,
                "🔎 Filtrera per kategori" => MenuChoice.FilterByCategory,
                "📊 Visa statistik" => MenuChoice.Stats,
                _ => MenuChoice.Exit
            };
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var manager = new BudgetManager();

            // Header titel
            AnsiConsole.Write(
                new FigletText("BudgetTrack")
                    .Centered()
                    .Color(Color.Aqua));

            while (true)
            {
                AnsiConsole.Write(new Rule("[bold deepskyblue1]Personal Budget Tracker[/]").Centered());

                switch (AskMenu())
                {
                    case MenuChoice.Add:
                        AddTransactionFlow(manager);
                        break;

                    case MenuChoice.List:
                        RenderTable(manager);
                        break;

                    case MenuChoice.Balance:
                        {
                            var bal = manager.CalculateBalance();
                            var color = bal >= 0 ? "green" : "red";
                            AnsiConsole.MarkupLine($"Balans: [bold {color}]{bal:0.00}[/]");
                            break;
                        }

                    case MenuChoice.Delete:
                        DeleteFlow(manager);
                        break;

                    case MenuChoice.Exit:
                        AnsiConsole.MarkupLine("[grey]Tack och välkommen åter![/]");
                        return;

                    case MenuChoice.FilterByCategory:
                        FilterByCategoryFlow(manager);
                        break;

                    case MenuChoice.Stats:
                        ShowStatistics(manager);
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
        static void FilterByCategoryFlow(BudgetManager manager)
        {
            var cats = manager.GetDistinctCategories().ToList();
            if (cats.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Inga kategorier ännu.[/]");
                return;
            }

            var chosen = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj [bold]kategori[/]:")
                    .AddChoices(cats));

            var list = manager.GetByCategory(chosen);
            if (list.Count == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]Inga transaktioner i kategorin '{chosen}'.[/]");
                return;
            }

            // återanvänd tabellrendering men för ett urval
            var table = new Table().Border(TableBorder.Rounded)
                                   .Title($"[bold underline]Transaktioner – {chosen}[/]");
            table.AddColumn("Id");
            table.AddColumn("Datum");
            table.AddColumn("Kategori");
            table.AddColumn(new TableColumn("Belopp").RightAligned());
            table.AddColumn("Beskrivning");

            foreach (var t in list)
            {
                var amountMarkup = t.Amount >= 0 ? $"[green]{t.Amount:0.00}[/]" : $"[red]{t.Amount:0.00}[/]";
                table.AddRow(t.Id.ToString(), t.Date, t.Category, amountMarkup, t.Description);
            }

            var sum = list.Sum(t => t.Amount);
            var sumColor = sum >= 0 ? "green" : "red";

            AnsiConsole.Write(table);
            AnsiConsole.Write(new Rule());
            AnsiConsole.MarkupLine($"Summa för [bold]{chosen}[/]: [bold {sumColor}]{sum:0.00}[/]");
        }

        static void ShowStatistics(BudgetManager manager)
        {
            var (count, income, expense, balance) = manager.CalculateStats();

            var panel = new Panel(
                new Rows(
                    new Markup($"Antal poster: [bold]{count}[/]"),
                    new Markup($"Total inkomst: [bold green]{income:0.00}[/]"),
                    new Markup($"Total utgift: [bold red]{expense:0.00}[/]"),
                    new Markup($"Balans: [bold {(balance >= 0 ? "green" : "red")}]{balance:0.00}[/]")
                )
            ).Header("📊 Statistik", Justify.Center).Border(BoxBorder.Rounded);

            AnsiConsole.Write(panel);
        }
    }
}