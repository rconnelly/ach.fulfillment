namespace Ach.Fulfillment.Data.Common
{
    public interface IDeleteByQueryActionData : IActionData
    {
        string Query { get; }
    }
}
