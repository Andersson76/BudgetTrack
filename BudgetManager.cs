namespace BudgetTrack;
using System.Linq;
public class BudgetManager
{
    private readonly List<Transaction> _transactions = new();
    private int _nextId = 1;

    //konstruktorn
    public BudgetManager() { }

    public void AddTransaction(string description, decimal amount, string category, string date)
    {
        _transactions.Add(new Transaction
        {
            Id = _nextId++,
            Description = description,
            Amount = amount,
            Category = category,
            Date = date
        });
    }

    public bool DeleteTransaction(int id)
    {
        var t = _transactions.FirstOrDefault(x => x.Id == id);
        if (t is null)
            return false;

        _transactions.Remove(t);
        return true;
    }

    public decimal CalculateBalance() => _transactions.Sum(t => t.Amount);

    public IReadOnlyList<Transaction> GetAll() => _transactions.AsReadOnly();

    public IReadOnlyList<Transaction> GetByCategory(string category)
    {
        return _transactions
            .Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();
    }

    public IEnumerable<string> GetDistinctCategories()
    {
        return _transactions
            .Select(t => t.Category)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c);
    }

    public (int Count, decimal Income, decimal Expense, decimal Balance) CalculateStats()
    {
        var income = _transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
        var expense = _transactions.Where(t => t.Amount < 0).Sum(t => t.Amount); // negativ summa
        var balance = income + expense;
        var count = _transactions.Count;
        return (count, income, expense, balance);
    }
}