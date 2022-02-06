using System.Globalization;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp
{
    /// <summary>
    /// Get user requests and calls the desired hendler.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// true - pogramm running, false - not running.
        /// </summary>
        public static bool isRunning = true;
        private const string DeveloperName = "Leontiy Lozovik";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());

        /// <summary>
        /// Get user comand from cmd and call functions to process them.
        /// </summary>
        /// <param name="args">parameters of "Main" function.</param>
        public static void Main(string[] args)
        {
            if (!(args is null))
            {
                ReadCmdArgument(args, ref fileCabinetService);
            }

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                var inputs = line != null ? line.Split(' ', 2) : new string[] { string.Empty, string.Empty };
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                var commandHandler = CreateCommandHandlers();
                commandHandler.Handle(
                        new AppCommandRequest
                        {
                            Command = command,
                            Parameters = parameters,
                        });
            }
            while (isRunning);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpCommandHandler = new HelpCommandHandler();
            var exitCommandHandler = new ExitCommandHandler();
            var statCommandHandler = new StatCommandHandle(Program.fileCabinetService);
            var listCommandHandler = new ListCommandHandler(Program.fileCabinetService);
            var createCommandHandler = new CreateCommandHandler(Program.fileCabinetService);
            var editCommandHandler = new EditCommandHandler(Program.fileCabinetService);
            var findCommandHandler = new FindCommandHandler(Program.fileCabinetService);
            var importCommandHandler = new ImportCommandHandler(Program.fileCabinetService);
            var exportCommandHandler = new ExportCommandHandler(Program.fileCabinetService);
            var removeCommandHandler = new RemoveCommandHandler(Program.fileCabinetService);
            var purgeCommandHandler = new PurgeCommandHandler(Program.fileCabinetService);
            var missedCommandHandler = new MissedCommandHandler();
            listCommandHandler.SetNext(createCommandHandler);
            createCommandHandler.SetNext(importCommandHandler);
            importCommandHandler.SetNext(removeCommandHandler);
            removeCommandHandler.SetNext(editCommandHandler);
            editCommandHandler.SetNext(findCommandHandler);
            findCommandHandler.SetNext(purgeCommandHandler);
            purgeCommandHandler.SetNext(statCommandHandler);
            statCommandHandler.SetNext(exportCommandHandler);
            exportCommandHandler.SetNext(helpCommandHandler);
            helpCommandHandler.SetNext(exitCommandHandler);
            exitCommandHandler.SetNext(missedCommandHandler);
            return listCommandHandler;
        }

        private static IRecordValidator SwitchValidationRules(string argument)
        {
            switch (argument)
            {
                case "DEFAULT":
                    Console.WriteLine("Using default validation rules.");
                    return new DefaultValidator();
                case "CUSTOM":
                    Console.WriteLine("Using custom validation rules.");
                    return new CustomValidator();
                default:
                    Console.WriteLine("There is only default and custom validation. Default rules will be set.");
                    return new DefaultValidator();
            }
        }

        private static IFileCabinetService SwitchStorageVariant(string argument, IRecordValidator validator)
        {
            switch (argument)
            {
                case "MEMORY":
                    Console.WriteLine("Using memory service.");
                    return new FileCabinetMemoryService(validator);
                case "FILE":
                    FileMode mode;
                    string path = "C:\\Epam-project\\FileCabinetApp\\FileDataBase\\cabinet-records.db";
                    if (File.Exists(path))
                    {
                        mode = FileMode.Truncate;
                    }
                    else
                    {
                        mode = FileMode.Create;
                    }

                    FileStream fileStream = new FileStream(path, mode);
                    Console.WriteLine("Using file service.");
                    return new FileCabinetFilesystemService(fileStream, validator);
                default:
                    Console.WriteLine("There is only file or memory service. Memory service will be used.");
                    return new FileCabinetMemoryService(validator);
            }
        }

        private static void ReadCmdArgument(string[] args, ref IFileCabinetService fileCabinetService)
        {
            if (args.Length > 0)
            {
                switch (args.Length)
                {
                    case 1:
                        var inputLine = args[0] != null ? args[0].Split('=', 2) : new string[] { string.Empty, string.Empty };
                        string command = inputLine[0];

                        switch (command)
                        {
                            case "--validation-rules":
                                string argument = inputLine[1].ToUpperInvariant();
                                fileCabinetService = new FileCabinetMemoryService(SwitchValidationRules(argument));
                                break;
                            case "--storage":
                                argument = inputLine[1].ToUpperInvariant();
                                fileCabinetService = SwitchStorageVariant(argument, new DefaultValidator());
                                break;
                            default:
                                Console.WriteLine("Unknown comand. Default validation rules and memory servise will be used.");
                                break;
                        }

                        break;
                    case 2:
                        if (args[0].StartsWith("--", StringComparison.OrdinalIgnoreCase) && args[0].Contains('=', StringComparison.OrdinalIgnoreCase))
                        {
                            var firstAtribute = args[0] != null ? args[0].Split('=', 2) : new string[] { string.Empty, string.Empty };
                            var secondAtribute = args[1] != null ? args[1].Split('=', 2) : new string[] { string.Empty, string.Empty };
                            string commandFirst = firstAtribute[0];
                            string commandSecond = secondAtribute[0];
                            switch (commandFirst)
                            {
                                case "--validation-rules":
                                    switch (commandSecond)
                                    {
                                        case "--storage":
                                            string firstArgument = firstAtribute[1].ToUpperInvariant();
                                            string secondArgument = secondAtribute[1].ToUpperInvariant();
                                            fileCabinetService = SwitchStorageVariant(secondArgument, SwitchValidationRules(firstArgument));
                                            break;
                                        default:
                                            firstArgument = firstAtribute[1].ToUpperInvariant();
                                            fileCabinetService = new FileCabinetMemoryService(SwitchValidationRules(firstArgument));
                                            break;
                                    }

                                    break;
                                case "--storage":
                                    switch (commandSecond)
                                    {
                                        case "--validation-rules":
                                            string firstArgument = firstAtribute[1].ToUpperInvariant();
                                            string secondArgument = secondAtribute[1].ToUpperInvariant();
                                            fileCabinetService = SwitchStorageVariant(firstArgument, SwitchValidationRules(secondArgument));
                                            break;
                                        default:
                                            firstArgument = firstAtribute[1].ToUpperInvariant();
                                            fileCabinetService = SwitchStorageVariant(firstArgument, new DefaultValidator());
                                            break;
                                    }

                                    break;
                                default:
                                    Console.WriteLine("Unknown comand. Default validation rules and memory servise will be used.");
                                    break;
                            }
                        }
                        else
                        {
                            command = args[0];
                            switch (command)
                            {
                                case "-v":
                                    string argument = args[1].ToUpperInvariant();
                                    fileCabinetService = new FileCabinetMemoryService(SwitchValidationRules(argument));
                                    break;
                                case "-s":
                                    argument = args[1].ToUpperInvariant();
                                    fileCabinetService = SwitchStorageVariant(argument, new DefaultValidator());
                                    break;

                                default:
                                    Console.WriteLine("Unknown comand. Default validation rules and memory servise will be used.");
                                    break;
                            }
                        }

                        break;
                    case 4:
                        string firstCommand = args[0];
                        string secondCommand = args[2];
                        switch (firstCommand)
                        {
                            case "-v":
                                switch (secondCommand)
                                {
                                    case "-s":
                                        string firstArgument = args[1].ToUpperInvariant();
                                        string secondArgument = args[3].ToUpperInvariant();
                                        fileCabinetService = SwitchStorageVariant(secondArgument, SwitchValidationRules(firstArgument));
                                        break;
                                    default:
                                        firstArgument = args[1].ToUpperInvariant();
                                        fileCabinetService = new FileCabinetMemoryService(SwitchValidationRules(firstArgument));
                                        break;
                                }

                                break;
                            case "-s":
                                switch (secondCommand)
                                {
                                    case "-v":
                                        string firstArgument = args[1].ToUpperInvariant();
                                        string secondArgument = args[3].ToUpperInvariant();
                                        fileCabinetService = SwitchStorageVariant(firstArgument, SwitchValidationRules(secondArgument));
                                        break;
                                    default:
                                        firstArgument = args[1].ToUpperInvariant();
                                        fileCabinetService = SwitchStorageVariant(firstArgument, new DefaultValidator());
                                        break;
                                }

                                break;
                            default:
                                Console.WriteLine("Unknown comand. Default validation rules and memory servise will be used.");
                                break;
                        }

                        break;
                    default:
                        Console.WriteLine("Invalid parameters. Default validation rules and memory servise will be used.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Using default validation rules and memory service.");
            }
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
            char[] separators = { ' ', '.', ',', '\'', '\"' };
            string trimedString = toConvert.Trim(separators);
            short numberOfChildren;
            bool goodShort = true;
            if (!short.TryParse(toConvert, out numberOfChildren))
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
            char[] separators = { ' ', '.', ',', '\'', '\"' };
            string trimedString = toConvert.Trim(separators);
            decimal averageSalary;
            bool goodNumber = true;
            if (!decimal.TryParse(toConvert, out averageSalary))
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
            switch (Program.fileCabinetService.GetValidationType())
            {
                case DefaultValidator:
                    if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
                    {
                        validationSuccess = false;
                    }

                    break;
                case CustomValidator:
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
            switch (Program.fileCabinetService.GetValidationType())
            {
                case DefaultValidator:
                    if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
                    {
                        validationSuccess = false;
                    }

                    break;
                case CustomValidator:
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
            switch (Program.fileCabinetService.GetValidationType())
            {
                case DefaultValidator:
                    DateTime oldest = new DateTime(1950, 1, 1);
                    DateTime now = DateTime.Now;
                    if (birthday < oldest || birthday > now)
                    {
                        validationSuccess = false;
                    }

                    break;
                case CustomValidator:
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
            switch (Program.fileCabinetService.GetValidationType())
            {
                case DefaultValidator:
                    if (children < 0)
                    {
                        validationSuccess = false;
                    }

                    break;
                case CustomValidator:
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
            switch (Program.fileCabinetService.GetValidationType())
            {
                case DefaultValidator:
                    if (salary < 0 || salary > 1000000000)
                    {
                        validationSuccess = false;
                    }

                    break;
                case CustomValidator:
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