using System.Diagnostics;

namespace Orleans.TimeTesting;

public enum ClockMode : long
{
    RealTime, Fixed, Incrementing
}
    
public class TestClock : ISystemClock
{
    private long mode = (long)ClockMode.RealTime;
    private DateTime? fixedTime;
    private Stopwatch? timer;

    private ClockMode Mode
    {
        get => (ClockMode) Interlocked.Read(ref mode);
        set => Interlocked.Exchange(ref mode, (long)value);
    }

    public void SetToIncrementingTime(DateTime dateTime)
    {
        timer = new Stopwatch();
        timer.Start();
        fixedTime = dateTime;
        Mode = ClockMode.Incrementing;
    }
        
    public void SetToFixedTime(DateTime dateTime)
    {
        timer = null;
        fixedTime = dateTime;
        Mode = ClockMode.Fixed;
    }
        
    public void Reset()
    {
        timer = null;
        fixedTime = null;
        Mode = ClockMode.RealTime;
    }

    public DateTime CurrentTime
    {
        get
        {
            var currentMode = Mode;
            if (currentMode == ClockMode.RealTime)
            {
                return DateTime.UtcNow;
            }

            if (fixedTime is { } dateTime)
            {
                if (currentMode == ClockMode.Fixed)
                {
                    return dateTime;
                }

                if (currentMode == ClockMode.Incrementing && timer is {} elapsed)
                {
                    return dateTime.Add(elapsed.Elapsed);
                }
            }

            return DateTime.UtcNow;
        }
    }

    private readonly DelayTimerQueue delayTimerQueue = new();
        
    public Task Delay(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        var timer = new DelayTimer(delayTimerQueue, duration, cancellationToken);
        return timer.Task;
    }

    public void FireAllDelayTimers()
    {
        delayTimerQueue.FireAllDelayTimers();
    }

    public Task WaitForTimers()
    {
        return delayTimerQueue.WaitForTimers();
    }
}