namespace BankOCR.Common
{
    public interface IValidator
    {
        public string ValidateAccountNumber(string input);
        public bool CheckIsAccountNumberValid(string input);
        public bool CheckIsAccountNumeric(string input);
        public bool ValidateChecksum(string input);
    }
}
