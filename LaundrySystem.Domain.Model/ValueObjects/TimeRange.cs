namespace LaundrySystem.Domain.Model.ValueObjects
{
    public class TimeRange
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        public TimeRange(DateTime start, DateTime end)
        {
            if (start >= end)
                throw new ArgumentException("Start time must be before end time.");

            Start = start;
            End = end;
        }

        public bool OverlapsWith(TimeRange other)
        {
            return Start < other.End && End > other.Start;
        }
    }
}