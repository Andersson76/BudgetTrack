namespace BudgetTrack;

public class BudgetManager
{
    private readonly List<Transaction> _transactions = new();
    private int _nextId = 1;

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
}