namespace QBReconcile.Models;

public class QBTransaction
{
    public string? Type { get; set; }
    public DateTime? Date { get; set; }
    public string? Num { get; set; }
    public string? Name { get; set; }
    public bool? Clr { get; set; }
    public decimal? Amount { get; set; }
}
