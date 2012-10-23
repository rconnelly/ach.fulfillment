namespace Ach.Fulfillment.Nacha.Enumeration
{
    public enum TransactionCode
    {
        CheckingAutomatedDeposit = 22,
        CheckingPrenoteCredit = 23,
        CheckingZeroDollarCredit = 24,
        CheckingAutomatedPayment = 27,
        CheckingPrenoteDebit = 28,
        CheckingZeroDollarDebit = 29,

        SavingAutomatedDeposit = 32,
        SavingPrenoteCredit = 33,
        SavingZeroDollarCredit = 34,
        SavingAutomatedPayment = 37,
        SavingPrenoteDebit = 38,
        SavingZeroDollarDebit = 39,
    }
}
