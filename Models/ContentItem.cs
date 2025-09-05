namespace Scheduler.Models;

public abstract class ContentItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;

  
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public TimeSpan Duration => End - Start;
}
