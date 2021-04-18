namespace BankOCR.Common
{
    public class AccountNumberValidator
    {
        public bool IsAccountValid;

        public AccountNumberValidator() { }

        public AccountNumberValidator(string accountNumber)
        {
            IsAccountValid = ValidateChecksum(accountNumber);
        }

        public bool ValidateChecksum(string accountNumber)
        {
            int checkSum = 0;

            for (int i = 0; i < accountNumber.Length; i++)
            {
                checkSum += int.Parse($"{accountNumber[i]}") * (accountNumber.Length - i);
            }

            return checkSum % 11 == 0;
        }

        public bool CheckIsAccountNumeric(string accountNumber)
        {
            return int.TryParse(accountNumber, out _);
        }

        public string MarkAccountNumberAsIllegible(string accountNumber)
        {
            return $"{accountNumber} ILL";
        }

        public string MarkAccountNumberAsInvalid(string accountNumber)
        {
            return $"{accountNumber} ERR";
        }
    }
}
