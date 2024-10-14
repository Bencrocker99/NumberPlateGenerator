using System.Data;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NumberPlateGeneratorTests")]
namespace NumberPlateGeneration
{
    public class NumberPlateGenerator
    {
        const int CUT_OFF_MONTH = 3; // Month that marks the start of a new year
        const int HALFWAY_CUT_OFF_MONTH = 9; // Month that marks halfway through the year
        const int LATE_YEAR_IDENTIFIER = 50; // Value to add to age identifier if numberplate is halfway through the year or further
        const int NUMBER_OF_RANDOM_LETTERS = 3; 
        const int MEMORY_TAG_LENGTH = 2;

        int NumberOfValidLetters { get { return _validLetters.Count; } }
        internal int MaximumCombinationOfRandomCharacters { get { return (int)Math.Pow((double) NumberOfValidLetters, (double) NUMBER_OF_RANDOM_LETTERS); } }

        readonly IReadOnlyList<char> _validLetters = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y' };
        readonly IReadOnlyList<char> _validDigits = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        Dictionary<string, int> _usedNumberplates = new Dictionary<string, int>(); 

        public string GenerateNumberPlate(string memoryTag, DateTime date)
        {
            if (!IsValidMemoryTag(memoryTag))
                throw new ArgumentException("Memory Tag is not valid for this format", nameof(memoryTag));

            string numberPlateStart = memoryTag + GetAgeIdentifier(date);

            return GenerateNumberPlateForIdentifier(numberPlateStart);
        }

        public bool IsNumberPlateValid(string numberPlate)
        {
            if (numberPlate.Length != 8)
                return false;

            return 
                IsValidLetter(numberPlate[0]) &&
                IsValidLetter(numberPlate[1]) &&
                IsValidDigit(numberPlate[2]) &&
                IsValidDigit(numberPlate[3]) &&
                numberPlate[4] == ' ' &&
                IsValidLetter(numberPlate[5]) &&
                IsValidLetter(numberPlate[6]) &&
                IsValidLetter(numberPlate[7]);
        }

        string GenerateNumberPlateForIdentifier(string identifier)
        {
            int numberPlateIndex = _usedNumberplates.GetValueOrDefault(identifier, 0);

            if (numberPlateIndex >= MaximumCombinationOfRandomCharacters)
                throw new ConstraintException($"Exceeded number of numberplates for: {identifier}");

            string numberPlateEnd = GetNextNumberplateEnding(NUMBER_OF_RANDOM_LETTERS, numberPlateIndex);

            _usedNumberplates[identifier] = numberPlateIndex + 1;

            return identifier + " " + numberPlateEnd;
        }

        string GetAgeIdentifier(DateTime date)
        {
            if (!IsValidDate(date))
                throw new ArgumentException("Date is not valid for this format", nameof(date));

            int year = int.Parse(date.ToString("yy")); // Take only the last 2 digits from the year

            if (ShouldUseEarlyYearIdentifier(date))
                return year.ToString("D2");
            else
            {
                if (date.Month < CUT_OFF_MONTH)
                    return (year + LATE_YEAR_IDENTIFIER - 1).ToString();
                else
                    return (year + LATE_YEAR_IDENTIFIER).ToString();
            } 
        }

        string GetNextNumberplateEnding(int numberOfLetters, int numberPlateIndex)
        {
            string letterSequence = "";
            int remainingUnitsToMap = numberPlateIndex;

            for (int i = 0; i < numberOfLetters; i++)
            {
                letterSequence = _validLetters[remainingUnitsToMap % NumberOfValidLetters] + letterSequence;
                remainingUnitsToMap /= NumberOfValidLetters;

                if (remainingUnitsToMap == 0)
                {
                    letterSequence = letterSequence.PadLeft(numberOfLetters, _validLetters[0]);

                    return letterSequence;
                }
            }

            return letterSequence;
        }

        bool IsValidLetter(char c)
        {
            return _validLetters.Contains(c);
        }        
        
        bool IsValidDigit(char c)
        {
            return _validDigits.Contains(c);
        }

        bool IsValidMemoryTag(string memoryTag)
        {
            if (memoryTag.Length != MEMORY_TAG_LENGTH)
                return false;

            foreach(char c in memoryTag)
            {
                if (!_validLetters.Contains(c))
                    return false;
            }

            return true;
        }

        bool IsValidDate(DateTime date)
        {
            DateTime earliestDate = new DateTime(2001, HALFWAY_CUT_OFF_MONTH, 01);
            DateTime latestDate = new DateTime(2051, CUT_OFF_MONTH, 01);

            return date >= earliestDate && date < latestDate;
        }

        bool ShouldUseEarlyYearIdentifier(DateTime date)
        {
            return date.Month is >= CUT_OFF_MONTH and < HALFWAY_CUT_OFF_MONTH;
        }
    }
}
