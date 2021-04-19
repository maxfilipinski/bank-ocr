using System;
using System.Collections.Generic;
using System.Linq;

namespace BankOCR.Common
{
    public class AccountNumberRebuilder : IRebuilder
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
                : TryRebuildIllegibleAccountNumber(accountNumber);
        }
        
        private string TryRebuildIllegibleAccountNumber(string illegibleAccountNumber)
        {
            List<string> accountNumberSubstitutions = new List<string>();
            int illDigitIndex = illegibleAccountNumber.IndexOf('?');
            List<char> accountNumber = illegibleAccountNumber.ToCharArray().ToList();
            var digitSubstitutes = NumberEquivalents.Select(x => x.Value).ToList();

            foreach (var digitSubstitute in digitSubstitutes)
            {
                accountNumber[illDigitIndex] = digitSubstitute;

                string accountNumberString = string.Join(string.Empty, accountNumber);

                if (_validator.ValidateChecksum(accountNumberString))
                {
                    accountNumberSubstitutions.Add(accountNumberString);
                }
            }

            if (!accountNumberSubstitutions.Count.Equals(1))
            {
                throw new Exception("Error in rebuilding ill account number!");
            }

            return string.Join(string.Empty, accountNumberSubstitutions.First());
        }

        private string TryRebuildInvalidAccountNumber(string invalidAccountNumber)
        {
            List<string> accountNumberSubstitutions = new List<string>();
            List<char> accountNumber = invalidAccountNumber.ToCharArray().ToList();

            for (int i = 0; i < accountNumber.Count; i++)
            {
                char digitToReplace = accountNumber[i];
                var digitSubstitutes = GetSimilarDigits(digitToReplace);

                foreach (var digitSubstitute in digitSubstitutes)
                {
                    accountNumber[i] = digitSubstitute;
                    string accountNumberString = string.Join(string.Empty, accountNumber);

                    if (_validator.ValidateChecksum(accountNumberString))
                    {
                        accountNumberSubstitutions.Add(accountNumberString);
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
                    return $"{invalidAccountNumber} AMB ['{string.Join("', '", accountNumberSubstitutions.OrderBy(s => s))}']";
            }
        }

        private IEnumerable<char> GetSimilarDigits(char digit)
        {
            var rootDigit = NumberEquivalents.Where(x => x.Value == digit).FirstOrDefault();

            foreach (var item in NumberEquivalents)
            {
                if (GetHammingDistance(rootDigit.Key, item.Key).Equals(1))
                {
                    yield return item.Value;
                }
            }
        }

        /// <summary>
        /// Returns number of positions at which the corresponding strings are different.
        /// </summary>
        /// <param name="string1"></param>
        /// <param name="string2"></param>
        /// <returns></returns>
        private int GetHammingDistance(string string1, string string2)
        {
            if (string1.Length != string2.Length)
            {
                throw new Exception("Strings must be equal length!");
            }

            int distance = string1
                .ToCharArray()
                .Zip(string2.ToCharArray(), (c1, c2) => new { c1, c2 })
                .Count(m => m.c1 != m.c2);

            return distance;
        }
    }
}
