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

            ValidateFirstName(record.FirstName);
            ValidateLastName(record.LastName);
            ValidateDateOfBirth(record.DateOfBirth);
            ValidateNumberOfChildren(record.Children);
            ValidateAverageSalary(record.AverageSalary);
            ValidateSex(record.Sex);
        }

        private static void ValidateFirstName(string parameter)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(parameter, "First name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(parameter) || parameter.Length < 2 || parameter.Length > 60)
            {
                throw new ArgumentException("Incorrect first name! First name should be grater then 2, less then 60 and can't be white space.", parameter);
            }
        }

        private static void ValidateLastName(string parameter)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(parameter, "Last name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(parameter) || parameter.Length < 2 || parameter.Length > 60)
            {
                throw new ArgumentException("Incorrect last name! Last name should be grater then 2, less then 60 and can't be white space.", parameter);
            }
        }

        private static void ValidateDateOfBirth(DateTime parameter)
        {
            DateTime oldest = new DateTime(1950, 1, 1);
            DateTime now = DateTime.Now;
            if (parameter < oldest || parameter > now)
            {
                throw new ArgumentException("Sorry but minimal date of birth - 01-Jan-1950 and maxsimum - current date");
            }
        }

        private static void ValidateNumberOfChildren(short parameter)
        {
            if (parameter < 0)
            {
                throw new ArgumentException("Number of children can't be less then 0.");
            }
        }

        private static void ValidateAverageSalary(decimal parameter)
        {
            if (parameter < 0 || parameter > 1000000000)
            {
                throw new ArgumentException("Average salary can't be less then 0 or grater then 1 billion.");
            }
        }

        private static void ValidateSex(char parameter)
        {
            if (parameter != 'm' && parameter != 'w')
            {
                throw new ArgumentException("Sorry, but your sex can be m - men or w - women only.");
            }
        }
    }
}
