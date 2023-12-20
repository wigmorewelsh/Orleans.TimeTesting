using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Core;
using Orleans.Runtime;

namespace Orleans.TimeTesting;

[Reentrant]
public class TimeService : GrainService, ITimeService
{
    private readonly TestClock testClock;
    private IGrainFactory GrainFactory;

    public TimeService(IServiceProvider services, IGrainIdentity id, Silo silo, ILoggerFactory loggerFactory, IGrainFactory grainFactory) : 
        base(id, silo, loggerFactory) 
    {
        GrainFactory = grainFactory;
        testClock = services.GetRequiredService<TestClock>();
    }

    public Task SetTimeIncrementing(DateTime dateTime)
    {
        testClock.SetToIncrementingTime(dateTime);
        return Task.CompletedTask;
    }
        
    public Task SetTime(DateTime dateTime)
    {
        testClock.SetToFixedTime(dateTime);
        return Task.CompletedTask;
    }
        
    public Task Reset()
    {
        testClock.Reset();
        return Task.CompletedTask;
    }

    public Task FireAllTimers()
    {
        testClock.FireAllDelayTimers();
        return Task.CompletedTask;
    }
}