using Orleans.Concurrency;
using Orleans.Runtime;

namespace Orleans.TimeTesting;

[Reentrant]
public class TimeManagementGrain : Grain, ITimeManagementGrain {

    readonly ITimeServiceClient timeServiceClient;

    public TimeManagementGrain(IGrainActivationContext grainActivationContext, ITimeServiceClient timeServiceClient) {
        this.timeServiceClient = timeServiceClient;
    }
      
    public Task SetTimeIncrementing(DateTime dateTime)
    {
        return timeServiceClient.SetTimeIncrementing(dateTime);
    }
      
    public Task SetTime(DateTime dateTime)
    {
        return timeServiceClient.SetTime(dateTime);
    }
        
    public Task Reset()
    {
        return timeServiceClient.Reset();
    }

    public Task FireAllTimers()
    {
        return timeServiceClient.FireAllTimers();
    }
}