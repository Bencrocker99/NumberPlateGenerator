using NumberPlateGeneration;
using System.Data;

namespace NumberPlateGeneratorTests
{
    public class NumberPlateGeneratorTests
    {
        [InlineData("AB10 DKM")]
        [InlineData("MV10 FRH")]
        [InlineData("YA51 YHL")]
        [Theory]
        public void IsNumberPlateValid_ValidNumberPlates_ReturnsTrue(string numberPlate)
        {
            NumberPlateGenerator numberPlateGenerator = new NumberPlateGenerator();

            bool isValid = numberPlateGenerator.IsNumberPlateValid(numberPlate);

            Assert.True(isValid);
        }

        [InlineData("AA10 AAA ")]
        [InlineData("AA10AAA ")]
        [InlineData("ABC0 ABC")]
        [InlineData("A012 ABC")]
        [InlineData("0A12 ABC")]
        [InlineData("AB12 0BC")]
        [InlineData("AB12 A0C")]
        [InlineData("AB12 AB0")]
        [InlineData("AB12 ABI")]
        [InlineData("AQ12 ABC")]
        [InlineData("ZB12 ABC")]
        [Theory]
        public void IsNumberPlateValid_InvalidNumberPlates_ReturnsFalse(string numberPlate)
        {
            NumberPlateGenerator numberPlateGenerator = new NumberPlateGenerator();

            bool isValid = numberPlateGenerator.IsNumberPlateValid(numberPlate);

            Assert.False(isValid);
        }

        [InlineData("AA", 2010, 1, 1)]
        [InlineData("RL", 2003, 8, 12)]
        [InlineData("JJ", 2019, 3, 24)]
        [InlineData("SA", 2017, 4, 30)]
        [InlineData("BP", 2011, 6, 28)]
        [Theory]
        public void GenerateNumberPlate_NumberPlateValidity_StartsWithMemoryTag(string memoryTag, int year, int month, int day)
        {
            NumberPlateGenerator numberPlateGenerator = new NumberPlateGenerator();

            string numberPlate = numberPlateGenerator.GenerateNumberPlate(memoryTag, new DateTime(year, month, day));

            bool startsWithMemoryTagAndYear = numberPlate.StartsWith(memoryTag);

            Assert.True(startsWithMemoryTagAndYear);
        }

        [InlineData("AA", 2010, 1, 1)]
        [InlineData("RL", 2003, 8, 12)]
        [InlineData("JJ", 2019, 3, 24)]
        [InlineData("SA", 2017, 4, 30)]
        [InlineData("BP", 2011, 6, 28)]
        [Theory]
        public void GenerateNumberPlate_NumberPlateValidity_IsValid(string memoryTag, int year, int month, int day)
        {
            NumberPlateGenerator numberPlateGenerator = new NumberPlateGenerator();

            string numberPlate = numberPlateGenerator.GenerateNumberPlate(memoryTag, new DateTime(year, month, day));

            bool isValid = numberPlateGenerator.IsNumberPlateValid(numberPlate);

            Assert.True(isValid);
        }

        [InlineData(2003, 3, 1, "03")]
        [InlineData(2003, 4, 1, "03")]
        [InlineData(2003, 5, 1, "03")]
        [InlineData(2003, 6, 1, "03")]
        [InlineData(2003, 7, 1, "03")]
        [InlineData(2003, 8, 1, "03")]
        [InlineData(2011, 5, 21, "11")]
        [InlineData(2039, 7, 6, "39")]
        [InlineData(2007, 8, 29, "07")]
        [Theory]
        public void GenerateNumberPlate_EarlyYear_EarlyYearCode (int year, int month, int day, string expected)
        {
            NumberPlateGenerator numberPlateGenerator = new NumberPlateGenerator();
            DateTime date = new DateTime(year, month, day);

            string numberPlate = numberPlateGenerator.GenerateNumberPlate("AA", date);

            string ageIdentifier = numberPlate.Substring(2, 2);

            Assert.Equal(expected, ageIdentifier);
        }

        [InlineData(2003, 9, 1, "53")]
        [InlineData(2003, 10, 1, "53")]
        [InlineData(2003, 11, 1, "53")]
        [InlineData(2003, 12, 1, "53")]
        [InlineData(2004, 1, 1, "53")]
        [InlineData(2004, 2, 1, "53")]
        [InlineData(2025, 11, 28, "75")]
        [InlineData(2014, 9, 11, "64")]
        [InlineData(2021, 2, 9, "70")]
        [Theory]
        public void GenerateNumberPlate_LateYear_EarlyYearCode(int year, int month, int day, string expected)
        {
            NumberPlateGenerator numberPlateGenerator = new NumberPlateGenerator();
            DateTime date = new DateTime(year, month, day);

            string numberPlate = numberPlateGenerator.GenerateNumberPlate("AA", date);

            string ageIdentifier = numberPlate.Substring(2, 2);

            Assert.Equal(expected, ageIdentifier);
        }

        [InlineData("aA")]
        [InlineData("11")]
        [InlineData("BBB")]
        [InlineData("AI")]
        [InlineData("AQ")]
        [InlineData("AZ")]
        [Theory]
        public void GenerateNumberPlate_InvalidMemoryTags_ThrowsArgumentException(string memoryTag)
        {
            NumberPlateGenerator numberPlateGenerator = new NumberPlateGenerator();

            DateTime date = new DateTime(2010, 3, 5);

            Assert.Throws<ArgumentException>(() => numberPlateGenerator.GenerateNumberPlate(memoryTag, date));
        }

        [InlineData(2001, 8, 1)]
        [InlineData(2001, 8, 31)]
        [InlineData(2051, 3, 1)]
        [Theory]
        public void GenerateNumberPlate_InvalidDates_ThrowsArgumentException(int year, int month, int day)
        {
            NumberPlateGenerator numberPlateGenerator = new NumberPlateGenerator();

            DateTime date = new DateTime(year, month, day);

            Assert.Throws<ArgumentException>(() => numberPlateGenerator.GenerateNumberPlate("AA", date));
        }

        [Fact]
        public void GenerateNumberPlate_ExceedingPossibleCombinations_ThrowsConstraintException()
        {
            NumberPlateGenerator numberPlateGenerator = new NumberPlateGenerator();

            for (int i = 0; i < numberPlateGenerator.MaximumCombinationOfRandomCharacters; i++)
            {
                numberPlateGenerator.GenerateNumberPlate("AA", new DateTime(2002, 3, 1));
            }

            Assert.Throws<ConstraintException>(() => numberPlateGenerator.GenerateNumberPlate("AA", new DateTime(2002, 3, 1)));

            for (int i = 0; i < numberPlateGenerator.MaximumCombinationOfRandomCharacters; i++)
            {
                numberPlateGenerator.GenerateNumberPlate("BB", new DateTime(2002, 3, 1));
            }

            Assert.Throws<ConstraintException>(() => numberPlateGenerator.GenerateNumberPlate("BB", new DateTime(2002, 3, 1)));
        }

        [Fact]
        public void GenerateNumerPlate_GeneratingAllCombinations_AllAreUnique()
        {
            NumberPlateGenerator numberPlateGenerator = new NumberPlateGenerator();

            HashSet<string> usedNumberplates = new HashSet<string>();

            for (int i = 0; i < numberPlateGenerator.MaximumCombinationOfRandomCharacters; i++)
            {
                string numberPlate = numberPlateGenerator.GenerateNumberPlate("DE", new DateTime(2012, 10, 1));

                bool isUnique = usedNumberplates.Add(numberPlate);

                Assert.True(isUnique);
            }
        }
    }
}