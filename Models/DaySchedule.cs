namespace Scheduler.Models;

public class DaySchedule
{
    public DateOnly Date { get; set; }
    public List<ContentItem> Items { get; } = new();

    public DaySchedule(DateOnly date)
    {
        Date = date;
    }
}
