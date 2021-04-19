using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankOCR.Common
{
    public class AccountNumberRebuilder
    {
        private readonly IConverter _converter;
        private readonly IValidator _validator;

        private readonly Dictionary<string, char> NumberEquivalents = new Dictionary<string, char>()
        {
            { " _ " + "| |" + "|_|", '0' },
            { "   " + "  |" + "  |", '1' },
            { " _ " + " _|" + "|_ ", '2' },
            { " _ " + " _|" + " _|", '3' },
            { "   " + "|_|" + "  |", '4' },
            { " _ " + "|_ " + " _|", '5' },
            { " _ " + "|_ " + "|_|", '6' },
            { " _ " + "  |" + "  |", '7' },
            { " _ " + "|_|" + "|_|", '8' },
            { " _ " + "|_|" + " _|", '9' }
        };

        public AccountNumberRebuilder()
        {
            _converter = new AccountNumberConverter();
            _validator = new AccountNumberValidator();
        }

        public string ValidateAndTryRebuildAccountNumber(string input)
        {
            var accountNumber = _converter.Convert(input);

            if (_validator.CheckIsAccountNumeric(accountNumber) && _validator.ValidateChecksum(accountNumber))
            {
                return accountNumber;
            }

            return _validator.CheckIsAccountNumeric(accountNumber) 
                ? TryRebuildInvalidAccountNumber(accountNumber) 
                : TryRebuildIllAccountNumber(accountNumber);
        }
        
        private string TryRebuildIllAccountNumber(string illAccountNumber)
        {
            var accountNumberSubstitutions = new List<string>();
            int illDigitIndex = illAccountNumber.IndexOf('?');
            List<char> accountNumber = illAccountNumber.ToCharArray().ToList();
            var illDigit = accountNumber[illDigitIndex];
            var digitSubstitutes = NumberEquivalents.Select(x => x.Value).ToList();

            foreach (var digitSubstitute in digitSubstitutes)
            {
                accountNumber[illDigitIndex] = digitSubstitute;

                if (_validator.ValidateChecksum(string.Join(string.Empty, accountNumber)))
                {
                    accountNumberSubstitutions.Add(string.Join(string.Empty, accountNumber));
                }
            }

            if (accountNumberSubstitutions.Count != 1)
            {
                throw new Exception("Error in rebuilding ill account number!");
            }

            return string.Join(string.Empty, accountNumberSubstitutions.First());
        }

        private string TryRebuildInvalidAccountNumber(string invalidAccountNumber)
        {
            var accountNumberSubstitutions = new List<string>();
            List<char> accountNumber = invalidAccountNumber.ToCharArray().ToList();

            for (int i = 0; i < accountNumber.Count; i++)
            {
                var digitToReplace = accountNumber[i];
                var digitSubstitutes = GetSimilarDigits(digitToReplace);

                foreach (var digitSubstitute in digitSubstitutes)
                {
                    accountNumber[i] = digitSubstitute;

                    if (_validator.ValidateChecksum(string.Join(string.Empty, accountNumber)))
                    {
                        accountNumberSubstitutions.Add(string.Join(string.Empty, accountNumber));
                    }

                    accountNumber[i] = digitToReplace;
                }
            }

            switch (accountNumberSubstitutions.Count)
            {
                case 0:
                    throw new Exception("Error in rebuilding invalid account number!");
                case 1:
                    return string.Join(string.Empty, accountNumberSubstitutions.First());
                default:
                    return $"{invalidAccountNumber} AMB ['{string.Join("', '", accountNumberSubstitutions.OrderBy(str => str))}']";
            }
        }

        private IEnumerable<char> GetSimilarDigits(char digit)
        {
            var initialDigit = NumberEquivalents.Where(x => x.Value == digit).FirstOrDefault();

            foreach (var item in NumberEquivalents)
            {
                if (GetHammingDistance(initialDigit.Key, item.Key).Equals(1))
                {
                    yield return item.Value;
                }
            }
        }

        private int GetHammingDistance(string comparable, string compared)
        {
            if (comparable.Length != compared.Length)
            {
                throw new Exception("Strings must be equal length!");
            }

            int distance = comparable
                .ToCharArray()
                .Zip(compared.ToCharArray(), (c1, c2) => new { c1, c2 })
                .Count(m => m.c1 != m.c2);

            return distance;
        }
    }
}
