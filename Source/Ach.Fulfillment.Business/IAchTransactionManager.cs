using Ach.Fulfillment.Data;

namespace Ach.Fulfillment.Business
{
    public interface IAchTransactionManager : IManager<AchTransactionEntity>
    {
        string Generate();
    }
}
