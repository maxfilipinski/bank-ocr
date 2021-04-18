using BankOCR.Common;
using NUnit.Framework;
using System.Collections.Generic;

namespace BankOcrKata
{
    public class UserStory3Tests
    {
        [TestCase(@"
 _  _  _  _  _  _  _  _    
| || || || || || || ||_   |
|_||_||_||_||_||_||_| _|  |", "000000051")]
        [TestCase(@"
    _  _  _  _  _  _     _ 
|_||_|| || ||_   |  |  | _ 
  | _||_||_||_|  |  |  | _|", "49006771? ILL")]
        [TestCase(@"
    _  _     _  _  _  _  _ 
  | _| _||_| _ |_   ||_||_|
  ||_  _|  | _||_|  ||_| _ ", "1234?678? ILL")]
        public void Tests(string input, string expectedResult)
        {
            // Arrange
            List<string> expected = new List<string>()
            {
                expectedResult
            };

            // Act
            AccountNumberConverter converter = new AccountNumberConverter(input);
            AccountNumberValidator validator = new AccountNumberValidator();
            List<string> actualAccountNumbers = new List<string>();

            foreach (var item in converter.Converted)
            {
                string accountNumber = item;

                if (validator.CheckIsAccountNumeric(accountNumber))
                {
                    if (!validator.ValidateChecksum(accountNumber))
                    {
                        accountNumber = validator.MarkAccountNumberAsInvalid(accountNumber);
                    }
                }
                else
                {
                    accountNumber = validator.MarkAccountNumberAsIllegible(accountNumber);
                }

                actualAccountNumbers.Add(accountNumber);
            }

            // Assert
            Assert.AreEqual(expected, actualAccountNumbers);
        }
    }
}