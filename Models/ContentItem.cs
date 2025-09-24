namespace Scheduler.Models;

public class ContentItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;


    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    
}






