using System;

namespace BankOCR.Common
{
    public class AccountNumberValidator
    {
        public bool IsAccountValid;

        public AccountNumberValidator(string accountNumber)
        {
            IsAccountValid = Validate(accountNumber);
        }

        private bool Validate(string accountNumber)
        {
            int checkSum = 0;

            for (int i = 0; i < accountNumber.Length; i++)
            {
                checkSum += int.Parse($"{accountNumber[i]}") * (accountNumber.Length - i);
            }

            return checkSum % 11 == 0;
        }
    }
}
