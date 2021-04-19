namespace BankOCR.Common
{
    public interface IValidator
    {
        public string ValidateAccountNumber(string input);
        public bool CheckIsValid(string input);
        public bool CheckIsAccountNumeric(string input);
        public bool ValidateChecksum(string input);
    }

    public class AccountNumberValidator : IValidator
    {
        private readonly IConverter _converter;

        public AccountNumberValidator() 
        {
            _converter = new AccountNumberConverter();
        }

        public string ValidateAccountNumber(string input)
        {
            string accountNumber = _converter.Convert(input);

            if (CheckIsAccountNumeric(accountNumber))
            {
                if (ValidateChecksum(accountNumber))
                {
                    return accountNumber;
                }

                return MarkAccountNumberAsInvalid(accountNumber);
            }
            else
            {
                return MarkAccountNumberAsIllegible(accountNumber);
            }
        }

        public bool CheckIsValid(string input)
        {
            return ValidateChecksum(input);
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

        private string MarkAccountNumberAsIllegible(string accountNumber)
        {
            return $"{accountNumber} ILL";
        }

        private string MarkAccountNumberAsInvalid(string accountNumber)
        {
            return $"{accountNumber} ERR";
        }
    }
}
