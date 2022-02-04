namespace FileCabinetApp
{
    /// <summary>
    /// Custom validation parameters of record.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <summary>
        /// Validate incoming parameters of record.
        /// </summary>
        /// <param name="record">record whose parametrs should be validate.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Instance doesn't exist.");
            }

            if (record.FirstName is null)
            {
                throw new ArgumentNullException(record.FirstName, "First name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(record.FirstName) || record.FirstName.Length < 1 || record.FirstName.Length > 20)
            {
                throw new ArgumentException("Incorrect first name! First name should be grater then 1, less then 20 and can't be white space. ", record.FirstName);
            }

            if (record.LastName is null)
            {
                throw new ArgumentNullException(record.LastName, "Last name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(record.LastName) || record.LastName.Length < 1 || record.LastName.Length > 20)
            {
                throw new ArgumentException("Incorrect last name! Last name should be grater then 1, less then 20 and can't be white space.", record.LastName);
            }

            if (record.Children < 1)
            {
                throw new ArgumentException("Number of children can't be less then 0. You should have a least one child.");
            }

            if (record.AverageSalary < 500 || record.AverageSalary > 1000000)
            {
                throw new ArgumentException("Average salary can't be less then 500 or grater then 1 million.");
            }

            if (record.Sex != 'm' && record.Sex != 'w')
            {
                throw new ArgumentException("Sorry, but your sex can be m - men or w - women only.");
            }

            DateTime oldest = new DateTime(1900, 1, 1);
            DateTime now = DateTime.Now;
            if (record.DateOfBirth < oldest || record.DateOfBirth > now)
            {
                throw new ArgumentException("Sorry but minimal date of birth - 01-Jan-1950 and maxsimum - current date");
            }
        }
    }
}
