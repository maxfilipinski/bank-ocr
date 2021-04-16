using System.Collections.Generic;

namespace BankOCR.Common
{
    public class StringConverter
    {
        public List<string> Converted = new List<string>();
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

        public StringConverter(string input)
        {
            Convert(input);
        }

        private void Convert(string input)
        {
            // imitate File.ReadAllLines => returns string[]
            string[] entryLinesFromFile = new string[]
            {
                string.Join("", input.Split('\n', '\r')).Substring(0, 27),
                string.Join("", input.Split('\n', '\r')).Substring(27, 27),
                string.Join("", input.Split('\n', '\r')).Substring(54, 27),
                new string(' ', 27)
            };

            int index = -1;
            List<List<string>> entryLinesToConvert = new List<List<string>>();

            for (int i = 0; i < entryLinesFromFile.Length; i += 4)
            {
                index++;
                entryLinesToConvert.Add(new List<string>());

                for (int j = 0; j < 3; j++)
                {
                    entryLinesToConvert[index].Add(entryLinesFromFile[i + j]);
                }
            }

            for (int i = 0; i < entryLinesToConvert.Count; i++)
            {
                ConvertEntryLinesToSeparateNumbersArray(entryLinesToConvert[i].ToArray());
            }
        }

        private void ConvertEntryLinesToSeparateNumbersArray(string[] entryLinesArray)
        {
            int nextNumberStartPosition = 0;
            string[] separateNumbersArray = new string[9];

            for (int i = 0; i < 9; i++)
            {
                if (i != 0)
                {
                    nextNumberStartPosition = i * 3;
                }
                separateNumbersArray[i] += entryLinesArray[0].Substring(nextNumberStartPosition, 3);
                separateNumbersArray[i] += entryLinesArray[1].Substring(nextNumberStartPosition, 3);
                separateNumbersArray[i] += entryLinesArray[2].Substring(nextNumberStartPosition, 3);
            }

            ConvertNumbersArrayToNumber(separateNumbersArray);
        }

        private void ConvertNumbersArrayToNumber(string[] numbersArray)
        {
            string result = "";
            
            for (int i = 0; i < numbersArray.Length; i++)
            {
                result += NumberEquivalents[numbersArray[i]];
            }

            Converted.Add(result);
        }
    }
}
