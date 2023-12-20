using Orleans.Services;

namespace Orleans.TimeTesting;

public interface ITimeServiceClient : IGrainServiceClient<ITimeService>, ITimeService {
        
}