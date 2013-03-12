namespace Ach.Fulfillment.Data.Common
{
    public interface IEnqueueData : IActionData
    {
        object Instance { get; }
    }
}