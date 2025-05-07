using System.Collections.ObjectModel;

namespace TodayWebAPi.DAL.Data.Entities;

public class OrderStatus
{
    public const string Pending = "Pending";

    public const string Processing = "Processing";

    public const string Shipped = "Shipped";

    public const string Delivered = "Delivered";

    public const string Cancelled = "Cancelled";

    private static readonly IReadOnlyCollection<string> _allStatuses = new ReadOnlyCollection<string>(new List<string>
    {
        Pending, Processing, Shipped, Delivered, Cancelled
    });

    public static IReadOnlyCollection<string> AllStatuses => _allStatuses;

    public static bool IsValid(string status)
    {
        return _allStatuses.Contains(status);
    }
}


