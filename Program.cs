using Scheduler.Models;
using Scheduler.Services;

namespace Scheduler;

class Program
{
    static void Main(string[] args)
    {
        var svc = new SchedulerService();
        var today = DateOnly.FromDateTime(DateTime.Today);

        svc.AddContent(today, new Reportage
        {
            Title = "Morning Reportage",
            Start = DateTime.Today.AddHours(9),
            End = DateTime.Today.AddHours(10),
            Producer = "Alice"
        });

        svc.AddContent(today, new LiveSession
        {
            Title = "Lunch Live",
            Start = DateTime.Today.AddHours(12),
            End = DateTime.Today.AddHours(14),
            HostCount = 2,
            HasGuest = true
        });

        svc.FillGapsWithMusic(today);

        PrintDay(svc, today);

        Console.WriteLine("\nPress Enter to exit...");
        Console.ReadLine();
    }

    static void PrintDay(SchedulerService svc, DateOnly date)
    {
        var day = svc.GetDay(date);
        Console.WriteLine($"=== Schedule for {date:yyyy-MM-dd} ===");

        foreach (var item in day.Items)
        {
            var type = item.GetType().Name;
            Console.WriteLine($"{item.Start:HH:mm}-{item.End:HH:mm}  {item.Title}  [{type}]");

            if (item is LiveSession ls)
                Console.WriteLine($"   Studio: {ls.Studio}, Guest: {(ls.HasGuest ? "Yes" : "No")}");
        }
    }
}
