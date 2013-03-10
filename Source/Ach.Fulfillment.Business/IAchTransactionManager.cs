namespace Ach.Fulfillment.Business
{
    using Data;

    public interface IAchTransactionManager
    {
        AchTransactionEntity Create(AchTransactionEntity instance);

        AchTransactionEntity Load(long id);
    }
}