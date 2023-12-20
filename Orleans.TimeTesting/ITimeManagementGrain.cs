namespace Orleans.TimeTesting;

public interface ITimeManagementGrain : IGrainWithIntegerKey
{
    Task SetTime(DateTime dateTime);
    Task SetTimeIncrementing(DateTime dateTime);
    Task Reset();
    Task FireAllTimers();
}