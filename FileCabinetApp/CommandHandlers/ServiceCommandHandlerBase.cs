using System.Globalization;
using FileCabinetApp.RecordValidators;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for commands that uses IFileCabinetServise.
    /// </summary>
    public class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// Instance of service.
        /// </summary>
#pragma warning disable SA1401 // Fields should be private
        protected static IFileCabinetService service = new FileCabinetMemoryService(new ValidatorBuilder().CreateDefault());
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="cabinetService">using service.</param>
        public ServiceCommandHandlerBase(IFileCabinetService cabinetService)
        {
            service = cabinetService;
        }

        /// <summary>
        /// Read all filds of record.
        /// </summary>
        /// <typeparam name="T">returning type.</typeparam>
        /// <param name="converter">one of converters.</param>
        /// <param name="validator">one of validators.</param>
        /// <returns>converted and validate fild of record.</returns>
        public static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                if (converter is null)
                {
                    throw new ArgumentNullException(nameof(converter));
                }
                else if (validator is null)
                {
                    throw new ArgumentNullException(nameof(validator));
                }

                T value;

                var input = Console.ReadLine();
                if (input is null)
                {
                    input = string.Empty;
                }

                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        /// <summary>
        /// Convert input param to string.
        /// </summary>
        /// <param name="toConvert">string to convert.</param>
        /// <returns>converted string.</returns>
        public static Tuple<bool, string, string> StringConverter(string toConvert)
        {
            if (toConvert is null)
            {
                throw new ArgumentNullException(nameof(toConvert));
            }

            char[] separators = { ' ', '.', ',', '\'', '\"' };
            string resultString = toConvert.Trim(separators);
            return new Tuple<bool, string, string>(true, toConvert, resultString);
        }

        /// <summary>
        /// Convert input param to date.
        /// </summary>
        /// <param name="toConvert">string to convert.</param>
        /// <returns>converted date.</returns>
        public static Tuple<bool, string, DateTime> DateConverter(string toConvert)
        {
            if (toConvert is null)
            {
                throw new ArgumentNullException(nameof(toConvert));
            }

            char[] separators = { ' ', '.', ',', '\'', '\"' };
            string trimedString = toConvert.Trim(separators);
            DateTime birthday;
            bool goodDate = true;
            if (!DateTime.TryParse(trimedString, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out birthday))
            {
                goodDate = false;
            }

            return new Tuple<bool, string, DateTime>(goodDate, toConvert, birthday);
        }

        /// <summary>
        /// Convert input param to short.
        /// </summary>
        /// <param name="toConvert">string to convert.</param>
        /// <returns>converted short.</returns>
        public static Tuple<bool, string, short> ShortConverter(string toConvert)
        {
            if (toConvert is null)
            {
                throw new ArgumentNullException(nameof(toConvert));
            }

            char[] separators = { ' ', '.', ',', '\'', '\"' };
            string trimedString = toConvert.Trim(separators);
            short numberOfChildren;
            bool goodShort = true;
            if (!short.TryParse(trimedString, out numberOfChildren))
            {
                goodShort = false;
            }

            return new Tuple<bool, string, short>(goodShort, toConvert, numberOfChildren);
        }

        /// <summary>
        /// Convert input param to decimal.
        /// </summary>
        /// <param name="toConvert">string to convert.</param>
        /// <returns>converted decimal.</returns>
        public static Tuple<bool, string, decimal> DecimalConverter(string toConvert)
        {
            if (toConvert is null)
            {
                throw new ArgumentNullException(nameof(toConvert));
            }

            char[] separators = { ' ', '.', ',', '\'', '\"' };
            string trimedString = toConvert.Trim(separators);
            decimal averageSalary;
            bool goodNumber = true;
            if (!decimal.TryParse(trimedString, out averageSalary))
            {
                goodNumber = false;
            }

            return new Tuple<bool, string, decimal>(goodNumber, toConvert, averageSalary);
        }

        /// <summary>
        /// Convert input param to char.
        /// </summary>
        /// <param name="toConvert">string to convert.</param>
        /// <returns>converted char.</returns>
        public static Tuple<bool, string, char> CharConverter(string toConvert)
        {
            if (toConvert is null)
            {
                throw new ArgumentNullException(nameof(toConvert));
            }

            char[] separators = { ' ', '.', ',', '\'', '\"' };
            string trimedString = toConvert.Trim(separators);
            bool isChar = true;
            if (trimedString.Length > 1)
            {
                isChar = false;
            }

            return new Tuple<bool, string, char>(isChar, toConvert, trimedString[0]);
        }

        /// <summary>
        /// Validate firstname.
        /// </summary>
        /// <param name="firstName">string to validate.</param>
        /// <returns>true - correct, false - incorrect.</returns>
        public static Tuple<bool, string> FirstNameValidator(string firstName)
        {
            bool validationSuccess = true;
            switch (Program.GetValidationType())
            {
                case ValidationType.Default:
                    if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
                    {
                        validationSuccess = false;
                    }

                    break;
                case ValidationType.Custom:
                    if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 1 || firstName.Length > 20)
                    {
                        validationSuccess = false;
                    }

                    break;
            }

            return new Tuple<bool, string>(validationSuccess, firstName);
        }

        /// <summary>
        /// Validate lastname.
        /// </summary>
        /// <param name="lastName">string to validate.</param>
        /// <returns>true - correct, false - incorrect.</returns>
        public static Tuple<bool, string> LastNameValidator(string lastName)
        {
            bool validationSuccess = true;
            switch (Program.GetValidationType())
            {
                case ValidationType.Default:
                    if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
                    {
                        validationSuccess = false;
                    }

                    break;
                case ValidationType.Custom:
                    if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 1 || lastName.Length > 20)
                    {
                        validationSuccess = false;
                    }

                    break;
            }

            return new Tuple<bool, string>(validationSuccess, lastName);
        }

        /// <summary>
        /// Validate date.
        /// </summary>
        /// <param name="birthday">date to validate.</param>
        /// <returns>true - correct, false - incorrect.</returns>
        public static Tuple<bool, string> DateOfBirthValidator(DateTime birthday)
        {
            bool validationSuccess = true;
            switch (Program.GetValidationType())
            {
                case ValidationType.Default:
                    DateTime oldest = new DateTime(1950, 1, 1);
                    DateTime now = DateTime.Now;
                    if (birthday < oldest || birthday > now)
                    {
                        validationSuccess = false;
                    }

                    break;
                case ValidationType.Custom:
                    oldest = new DateTime(1900, 1, 1);
                    now = DateTime.Now;
                    if (birthday < oldest || birthday > now)
                    {
                        validationSuccess = false;
                    }

                    break;
            }

            return new Tuple<bool, string>(validationSuccess, birthday.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Validate number of childres.
        /// </summary>
        /// <param name="children">number of children to validate.</param>
        /// <returns>true - correct, false - incorrect.</returns>
        public static Tuple<bool, string> NumberOfChildrenValidator(short children)
        {
            bool validationSuccess = true;
            switch (Program.GetValidationType())
            {
                case ValidationType.Default:
                    if (children < 0)
                    {
                        validationSuccess = false;
                    }

                    break;
                case ValidationType.Custom:
                    if (children < 1)
                    {
                        validationSuccess = false;
                    }

                    break;
            }

            return new Tuple<bool, string>(validationSuccess, children.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Validate salary.
        /// </summary>
        /// <param name="salary">salary to validate.</param>
        /// <returns>true - correct, false - incorrect.</returns>
        public static Tuple<bool, string> AverageSalaryValidator(decimal salary)
        {
            bool validationSuccess = true;
            switch (Program.GetValidationType())
            {
                case ValidationType.Default:
                    if (salary < 0 || salary > 1000000000)
                    {
                        validationSuccess = false;
                    }

                    break;
                case ValidationType.Custom:
                    if (salary < 500 || salary > 1000000)
                    {
                        validationSuccess = false;
                    }

                    break;
            }

            return new Tuple<bool, string>(validationSuccess, salary.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Validate sex.
        /// </summary>
        /// <param name="sex">sex to validate.</param>
        /// <returns>true - correct, false - incorrect.</returns>
        public static Tuple<bool, string> SexValidator(char sex)
        {
            bool validationSuccess = true;
            if (sex != 'm' && sex != 'w')
            {
                validationSuccess = false;
            }

            return new Tuple<bool, string>(validationSuccess, sex.ToString());
        }
    }
}
