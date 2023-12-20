using Orleans.Services;

namespace Orleans.TimeTesting;

public interface ITimeService : IGrainService
{
    Task SetTime(DateTime dateTime);
    Task SetTimeIncrementing(DateTime dateTime);
    Task Reset();
    public Task FireAllTimers();
}