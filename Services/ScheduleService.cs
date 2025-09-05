using Scheduler.Models;

namespace Scheduler.Services;

public class SchedulerService
{
    
    private readonly Dictionary<DateOnly, DaySchedule> _days = new();

    public DaySchedule GetDay(DateOnly date)
    {
        if (!_days.TryGetValue(date, out var day))
        {
            day = new DaySchedule(date);
            _days[date] = day;
        }
      
        day.Items.Sort((a, b) => a.Start.CompareTo(b.Start));
        return day;
    }

    public void AddContent(DateOnly date, ContentItem item)
    {
        var day = GetDay(date);

        
        var startOfDay = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Local);
        var endOfDay = DateTime.SpecifyKind(date.AddDays(1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Local);

        if (item.Start < startOfDay) item.Start = startOfDay;
        if (item.End > endOfDay) item.End = endOfDay;

        if (item.End <= item.Start)
            throw new ArgumentException("Item must have positive duration within the day.");

        
        var toInsertAfter = new List<ContentItem>();
        foreach (var existing in day.Items.ToList())
        {
            bool overlaps = item.Start < existing.End && item.End > existing.Start;
            if (!overlaps) continue;

            if (existing is Music)
            {
                
                day.Items.Remove(existing);

        
                if (existing.Start < item.Start)
                {
                    toInsertAfter.Add(new Music
                    {
                        Title = existing.Title,
                        Start = existing.Start,
                        End = item.Start,
                        PlaylistTag = (existing as Music)?.PlaylistTag
                    });
                }

           
                if (item.End < existing.End)
                {
                    toInsertAfter.Add(new Music
                    {
                        Title = existing.Title,
                        Start = item.End,
                        End = existing.End,
                        PlaylistTag = (existing as Music)?.PlaylistTag
                    });
                }
            }
            else
            {
                
                throw new InvalidOperationException(
                    $"Overlap between '{existing.Title}' and '{item.Title}'.");
            }
        }

        day.Items.Add(item);
        if (toInsertAfter.Count > 0)
            day.Items.AddRange(toInsertAfter);

        day.Items.Sort((a, b) => a.Start.CompareTo(b.Start));
    }

    public bool RemoveContent(DateOnly date, Guid id)
    {
        var day = GetDay(date);
        var removed = day.Items.RemoveAll(x => x.Id == id) > 0;
        return removed;
    }

    public void FillGapsWithMusic(DateOnly date)
    {
        var day = GetDay(date);
        var startOfDay = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Local);
        var endOfDay = DateTime.SpecifyKind(date.AddDays(1).ToDateTime(TimeOnly.MinValue), DateTimeKind.Local);

        
        if (day.Items.Count == 0)
        {
            day.Items.Add(new Music
            {
                Title = "Auto Music",
                Start = startOfDay,
                End = endOfDay
            });
            return;
        }

        var sorted = day.Items.OrderBy(i => i.Start).ToList();
        var auto = new List<ContentItem>();

        
        if (sorted.First().Start > startOfDay)
        {
            auto.Add(new Music { Title = "Auto Music", Start = startOfDay, End = sorted.First().Start });
        }

      
        for (int i = 0; i < sorted.Count - 1; i++)
        {
            var gapStart = sorted[i].End;
            var gapEnd = sorted[i + 1].Start;
            if (gapEnd > gapStart)
            {
                auto.Add(new Music { Title = "Auto Music", Start = gapStart, End = gapEnd });
            }
        }

      
        if (sorted.Last().End < endOfDay)
        {
            auto.Add(new Music { Title = "Auto Music", Start = sorted.Last().End, End = endOfDay });
        }

        day.Items.AddRange(auto);
        day.Items.Sort((a, b) => a.Start.CompareTo(b.Start));
    }

    public void EnsureNextSevenDaysCoverage(DateOnly startDate)
    {
        for (int i = 0; i < 7; i++)
        {
            var d = startDate.AddDays(i);
            FillGapsWithMusic(d);
        }
    }

    public IEnumerable<DaySchedule> GetWeek(DateOnly startDate, int days = 7)
    {
        for (int i = 0; i < days; i++)
            yield return GetDay(startDate.AddDays(i));
    }
}
