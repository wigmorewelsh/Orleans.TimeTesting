namespace Orleans.TimeTesting;

public class DelayTimer
{
    private readonly TimeSpan duration;
    private readonly CancellationToken cancellationToken;
    private readonly TaskCompletionSource taskCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly DelayTimerQueue delayTimerQueue;

    public DelayTimer(DelayTimerQueue delayTimerQueue, TimeSpan duration, CancellationToken cancellationToken)
    {
        this.delayTimerQueue = delayTimerQueue;
        this.duration = duration;
        this.cancellationToken = cancellationToken;

        delayTimerQueue.Add(this);
            
        FireTask().Ignore();
        cancellationToken.Register(() => Cancel());
    }

    public void Cancel()
    {
        taskCompletionSource.TrySetCanceled();
        delayTimerQueue.Remove(this);
    }

    public void FireNow()
    {
        taskCompletionSource.TrySetResult();
        Cancel();
    }

    private async Task FireTask()
    {
        try
        {
            await Task.Delay(duration, cancellationToken);
            taskCompletionSource.TrySetResult();
            delayTimerQueue.Remove(this);
        }
        catch(Exception err)
        {
            taskCompletionSource.TrySetException(err);
            delayTimerQueue.Remove(this);
            throw;
        }
    }

    public Task Task => taskCompletionSource.Task;
}