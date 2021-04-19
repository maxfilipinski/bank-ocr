namespace BankOCR.Common
{
    public interface IRebuilder
    {
        public string ValidateAndTryRebuildAccountNumber(string input);
    }
}
