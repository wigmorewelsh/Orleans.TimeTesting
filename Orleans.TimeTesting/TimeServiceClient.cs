using Orleans.Runtime;
using Orleans.Runtime.Services;
using Orleans.Serialization;

namespace Orleans.TimeTesting;

public class TimeServiceClient : GrainServiceClient<ITimeService>, ITimeServiceClient {
    private readonly IGrainReferenceConverter grainReferenceConverter;
    private readonly IGrainFactory factory;

    public TimeServiceClient(IServiceProvider serviceProvider, IGrainReferenceConverter grainReferenceConverter, IGrainFactory factory) : base(serviceProvider)
    {
        this.grainReferenceConverter = grainReferenceConverter;
        this.factory = factory;
    }

    public Task SetTime(DateTime dateTime)
    {
        return Broadcast(silo => silo.SetTime(dateTime));
    }

    public Task SetTimeIncrementing(DateTime dateTime)
    {
        return Broadcast(silo => silo.SetTimeIncrementing(dateTime));
    }
        
    public Task Reset()
    {
        return Broadcast(silo => silo.Reset());
    }

    public Task FireAllTimers()
    {
        return Broadcast(silo => silo.FireAllTimers());
    }

    private async Task Broadcast(Func<ITimeService, Task> action)
    {
        var managementGrain = factory.GetGrain<IManagementGrain>(0);
        var grainReference = GrainService as GrainReference;
        var keyInfo = grainReference!.ToKeyInfo().Key;
        var silos = await managementGrain.GetHosts();
        foreach (var (silo, status) in silos)
        {
            if(status != SiloStatus.Active) continue;

            var siloKey = new GrainReferenceKeyInfo(keyInfo, (silo.Endpoint, silo.Generation));
            var siloReference = grainReferenceConverter.GetGrainFromKeyInfo(siloKey);
            var siloGrain = siloReference.Cast<ITimeService>();

            await action(siloGrain);
        } 
    }
}