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

            // Act
            AccountNumberValidator validator = new AccountNumberValidator();

            // Assert
            Assert.AreEqual(expectedResult, validator.ValidateAccountNumber(input));
        }
    }
}