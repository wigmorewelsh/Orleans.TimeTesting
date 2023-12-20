namespace Orleans.TimeTesting;

public class DelayTimerQueue
{
    private List<DelayTimer> delayTimers = new List<DelayTimer>();
    private object _lock = new();
        
    public void Add(DelayTimer delayTimer)
    {
        lock (_lock)
        {
            delayTimers.Add(delayTimer);
            if (pendingCallback != null)
            {
                pendingCallback.TrySetResult();
                pendingCallback = null;
            }
        }
    }

    public void Remove(DelayTimer delayTimer)
    {
        lock (_lock)
        {
            delayTimers.Remove(delayTimer);
            if (pendingCallback != null)
            {
                pendingCallback.TrySetResult();
                pendingCallback = null;
            }
        }
    }

    public void FireAllDelayTimers()
    {
        List<DelayTimer> timers;
        lock (_lock)
        {
            timers = delayTimers.ToList();
        }

        foreach (var timer in timers)
        {
            timer.FireNow();
        }
    }

    private TaskCompletionSource? pendingCallback;
        
    public Task WaitForTimers()
    {
        if (pendingCallback != null) return pendingCallback.Task;
        lock (_lock)
        {
            pendingCallback = new TaskCompletionSource();
        }

        return pendingCallback.Task;
    }
}