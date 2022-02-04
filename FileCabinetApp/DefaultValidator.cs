namespace FileCabinetApp
{
    /// <summary>
    /// Default validation parameters of record.
    /// </summary>
    public class DefaultValidator : IRecordValidator
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
            else if (string.IsNullOrWhiteSpace(record.FirstName) || record.FirstName.Length < 2 || record.FirstName.Length > 60)
            {
                throw new ArgumentException("Incorrect first name! First name should be grater then 2, less then 60 and can't be white space.", record.FirstName);
            }

            if (record.LastName is null)
            {
                throw new ArgumentNullException(record.LastName, "Last name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(record.LastName) || record.LastName.Length < 2 || record.LastName.Length > 60)
            {
                throw new ArgumentException("Incorrect last name! Last name should be grater then 2, less then 60 and can't be white space.", record.LastName);
            }

            if (record.Children < 0)
            {
                throw new ArgumentException("Number of children can't be less then 0.");
            }

            if (record.AverageSalary < 0 || record.AverageSalary > 1000000000)
            {
                throw new ArgumentException("Average salary can't be less then 0 or grater then 1 billion.");
            }

            if (record.Sex != 'm' && record.Sex != 'w')
            {
                throw new ArgumentException("Sorry, but your sex can be m - men or w - women only.");
            }

            DateTime oldest = new DateTime(1950, 1, 1);
            DateTime now = DateTime.Now;
            if (record.DateOfBirth < oldest || record.DateOfBirth > now)
            {
                throw new ArgumentException("Sorry but minimal date of birth - 01-Jan-1950 and maxsimum - current date");
            }
        }
    }
}
