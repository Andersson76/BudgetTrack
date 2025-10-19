namespace BudgetTrack;

public class Transaction
{
    // KRAV: Description, Amount, Category, Date (text)
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }            // + = inkomst, - = utgift
    public string Category { get; set; } = "";
    public string Date { get; set; } = "";         // t.ex. "2025-10-10"

    // KRAV: ShowInfo()
    public void ShowInfo()
    {
        Console.WriteLine($"{Id,2}  {Date,-10}  {Category,-12}  {Amount,10:0.00}  {Description}");
    }
}