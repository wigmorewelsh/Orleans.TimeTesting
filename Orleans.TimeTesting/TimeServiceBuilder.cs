using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Orleans.TestingHost;

namespace Orleans.TimeTesting;

public interface ISystemClock
{
    DateTime CurrentTime { get; }

    Task Delay(TimeSpan duration, CancellationToken cancellationToken = default);
}

public sealed class SystemClock : ISystemClock
{
    public DateTime CurrentTime => DateTime.UtcNow;
    public Task Delay(TimeSpan duration, CancellationToken cancellationToken = default)
    {
        return Task.Delay(duration, cancellationToken);
    }
}
    
public class TimeServiceBuilder : IClientBuilderConfigurator, ISiloConfigurator 
{
    public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
    {
    }

    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.ConfigureServices(services =>
        {
            var descriptor =
                from s in services
                where s.ServiceType == typeof(ISystemClock)
                select s;

            foreach (var serviceDescriptor in descriptor)
            {
                services.Remove(serviceDescriptor);
            }

            services.AddSingleton<TestClock>();
            services.AddSingleton<ISystemClock>(ctx => ctx.GetRequiredService<TestClock>());
            services.AddSingleton<ITimeServiceClient, TimeServiceClient>();
        });
        siloBuilder.AddGrainService<TimeService>();
        siloBuilder.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(TimeServiceBuilder).Assembly));
    }
}