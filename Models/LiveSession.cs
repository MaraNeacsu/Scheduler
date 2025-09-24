namespace Scheduler.Models;

public class LiveSession : ContentItem
{
    public int HostCount { get; set; } = 1;
    public bool HasGuest { get; set; }

  
    public Studio Studio => HostCount == 1 ? Studio.Studio1 : Studio.Studio2;

    public string? Notes { get; set; }
}

