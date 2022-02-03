using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Get user requests and calls the desired hendler.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Leontiy Lozovik";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("purge", Purge),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints records statistics", "The 'stat' command prints records statistics." },
            new string[] { "create", "create record with information about you", "The 'create' command create record with information about you." },
            new string[] { "list", "prints all existing records", "The 'list' command prints all existing records" },
            new string[] { "edit", "edit your record by Id", "The 'edit' command edit your record by Id" },
            new string[] { "find", "find records by one parameter", "The 'find' command find records by one parameter" },
            new string[] { "export", "exporting service records to files of a certain type", "The 'export' command exporting service records to files of a certain type" },
            new string[] { "import", "importing service records from files of a certain type", "The 'import' command importing service records from files of a certain type" },
            new string[] { "remove", "remove records from service", "The 'remove' command remove records from service" },
            new string[] { "purge", "defragments the data file", "The 'purge' command defragments the data file" },
        };

        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());

        /// <summary>
        /// Get user comand from cmd and call functions to process them.
        /// </summary>
        /// <param name="args">parameters of "Main" function.</param>
        public static void Main(string[] args)
        {
            ReadCmdArgument(args, ref fileCabinetService);

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

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static IRecordValidator SwitchValidationRules(string argument)
        {
            switch (argument)
            {
                case "default":
                    Console.WriteLine("Using default validation rules.");
                    return new DefaultValidator();
                case "custom":
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
                case "memory":
                    Console.WriteLine("Using memory service.");
                    return new FileCabinetMemoryService(validator);
                case "file":
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
                                string argument = inputLine[1].ToLowerInvariant();
                                fileCabinetService = new FileCabinetMemoryService(SwitchValidationRules(argument));
                                break;
                            case "--storage":
                                argument = inputLine[1].ToLowerInvariant();
                                fileCabinetService = SwitchStorageVariant(argument, new DefaultValidator());
                                break;
                            default:
                                Console.WriteLine("Unknown comand. Default validation rules and memory servise will be used.");
                                break;
                        }

                        break;
                    case 2:
                        if (args[0].StartsWith("--") && args[0].Contains("="))
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
                                            string firstArgument = firstAtribute[1].ToLowerInvariant();
                                            string secondArgument = secondAtribute[1].ToLowerInvariant();
                                            fileCabinetService = SwitchStorageVariant(secondArgument, SwitchValidationRules(firstArgument));
                                            break;
                                        default:
                                            firstArgument = firstAtribute[1].ToLowerInvariant();
                                            fileCabinetService = new FileCabinetMemoryService(SwitchValidationRules(firstArgument));
                                            break;
                                    }

                                    break;
                                case "--storage":
                                    switch (commandSecond)
                                    {
                                        case "--validation-rules":
                                            string firstArgument = firstAtribute[1].ToLowerInvariant();
                                            string secondArgument = secondAtribute[1].ToLowerInvariant();
                                            fileCabinetService = SwitchStorageVariant(firstArgument, SwitchValidationRules(secondArgument));
                                            break;
                                        default:
                                            firstArgument = firstAtribute[1].ToLowerInvariant();
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
                                    string argument = args[1].ToLowerInvariant();
                                    fileCabinetService = new FileCabinetMemoryService(SwitchValidationRules(argument));
                                    break;
                                case "-s":
                                    argument = args[1].ToLowerInvariant();
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
                                        string firstArgument = args[1].ToLowerInvariant();
                                        string secondArgument = args[3].ToLowerInvariant();
                                        fileCabinetService = SwitchStorageVariant(secondArgument, SwitchValidationRules(firstArgument));
                                        break;
                                    default:
                                        firstArgument = args[1].ToLowerInvariant();
                                        fileCabinetService = new FileCabinetMemoryService(SwitchValidationRules(firstArgument));
                                        break;
                                }

                                break;
                            case "-s":
                                switch (secondCommand)
                                {
                                    case "-v":
                                        string firstArgument = args[1].ToLowerInvariant();
                                        string secondArgument = args[3].ToLowerInvariant();
                                        fileCabinetService = SwitchStorageVariant(firstArgument, SwitchValidationRules(secondArgument));
                                        break;
                                    default:
                                        firstArgument = args[1].ToLowerInvariant();
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

        private static void PrintListOfRecords(ReadOnlyCollection<FileCabinetRecord> recordToPrint)
        {
            foreach (var record in recordToPrint)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth:yyyy-MMM-dd}, {record.Children}, {record.AverageSalary}, {record.Sex}");
            }
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
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

        private static Tuple<bool, string, string> StringConverter(string toConvert)
        {
            char[] separators = { ' ', '.', ',', '\'', '\"' };
            string resultString = toConvert.Trim(separators);
            return new Tuple<bool, string, string>(true, toConvert, resultString);
        }

        private static Tuple<bool, string, DateTime> DateConverter(string toConvert)
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

        private static Tuple<bool, string, short> ShortConverter(string toConvert)
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

        private static Tuple<bool, string, decimal> DecimalConverter(string toConvert)
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

        private static Tuple<bool, string, char> CharConverter(string toConvert)
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

        private static Tuple<bool, string> FirstNameValidator(string firstName)
        {
            bool validationSuccess = true;
            switch (fileCabinetService.GetValidationType())
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

        private static Tuple<bool, string> LastNameValidator(string lastName)
        {
            bool validationSuccess = true;
            switch (fileCabinetService.GetValidationType())
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

        private static Tuple<bool, string> DateOfBirthValidator(DateTime birthday)
        {
            bool validationSuccess = true;
            switch (fileCabinetService.GetValidationType())
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

            return new Tuple<bool, string>(validationSuccess, birthday.ToString());
        }

        private static Tuple<bool, string> NumberOfChildrenValidator(short children)
        {
            bool validationSuccess = true;
            switch (fileCabinetService.GetValidationType())
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

            return new Tuple<bool, string>(validationSuccess, children.ToString());
        }

        private static Tuple<bool, string> AverageSalaryValidator(decimal salary)
        {
            bool validationSuccess = true;
            switch (fileCabinetService.GetValidationType())
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

            return new Tuple<bool, string>(validationSuccess, salary.ToString());
        }

        private static Tuple<bool, string> SexValidator(char sex)
        {
            bool validationSuccess = true;
            if (sex != 'm' && sex != 'w')
            {
                validationSuccess = false;
            }

            return new Tuple<bool, string>(validationSuccess, sex.ToString());
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            bool flagNotEnd = true;
            while (flagNotEnd)
            {
                Console.Write("First name: ");
                var firstName = ReadInput(StringConverter, FirstNameValidator);

                Console.Write("Last name: ");
                var lastName = ReadInput(StringConverter, LastNameValidator);

                Console.Write("Date of birth: ");
                var birthday = ReadInput(DateConverter, DateOfBirthValidator);

                Console.Write("Number of children: ");
                short children = ReadInput(ShortConverter, NumberOfChildrenValidator);

                Console.Write("Averege salary: ");
                decimal salary = ReadInput(DecimalConverter, AverageSalaryValidator);

                Console.Write("Sex (m - men, w - women): ");
                char sex = ReadInput(CharConverter, SexValidator);

                var record = new FileCabinetRecord
                {
                    Id = 0,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = birthday,
                    Children = children,
                    AverageSalary = salary,
                    Sex = sex,
                };

                try
                {
                    var id = Program.fileCabinetService.CreateRecord(record);
                    Console.WriteLine($"Record #{id} is created.");
                    flagNotEnd = false;
                }
                catch (ArgumentException exeption)
                {
                    Console.WriteLine(exeption.Message);
                    flagNotEnd = false;
                }
            }
        }

        private static void List(string parameters)
        {
            var listOfRecords = Program.fileCabinetService.GetRecords();
            PrintListOfRecords(listOfRecords);
        }

        private static void Edit(string parameters)
        {
            int enteredId;
            if (!int.TryParse(parameters, out enteredId))
            {
                Console.WriteLine("Error! Please check inputed Id.");
            }

            if (enteredId <= 0)
            {
                Console.WriteLine("Id should be grater then 0");
            }
            else if (!fileCabinetService.IsIdExist(enteredId))
            {
                Console.WriteLine($"#{enteredId} record is not found.");
            }
            else
            {
                bool flagNotEnd = true;
                while (flagNotEnd)
                {
                    Console.Write("First name: ");
                    var firstName = ReadInput(StringConverter, FirstNameValidator);

                    Console.Write("Last name: ");
                    var lastName = ReadInput(StringConverter, LastNameValidator);

                    Console.Write("Date of birth: ");
                    var birthday = ReadInput(DateConverter, DateOfBirthValidator);

                    Console.Write("Number of children: ");
                    short children = ReadInput(ShortConverter, NumberOfChildrenValidator);

                    Console.Write("Averege salary: ");
                    decimal salary = ReadInput(DecimalConverter, AverageSalaryValidator);

                    Console.Write("Sex (m - men, w - women): ");
                    char sex = ReadInput(CharConverter, SexValidator);

                    var newRecord = new FileCabinetRecord
                    {
                        Id = enteredId,
                        FirstName = firstName,
                        LastName = lastName,
                        DateOfBirth = birthday,
                        Children = children,
                        AverageSalary = salary,
                        Sex = sex,
                    };

                    try
                    {
                        Program.fileCabinetService.EditRecord(newRecord);
                        Console.WriteLine($"Record #{enteredId} is updated.");
                        flagNotEnd = false;
                    }
                    catch (ArgumentException exeption)
                    {
                        Console.WriteLine(exeption.Message);
                        flagNotEnd = false;
                    }
                }
            }
        }

        private static void Find(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Command 'find' should contain name of property and text to find.");
            }
            else
            {
                string[] input = parameters.Split(' ', 2);
                if (input.Length != 2)
                {
                    Console.WriteLine("Please check you input.");
                }
                else
                {
                    string propName = input[0].ToLowerInvariant();
                    string textToFind = input[1].Trim('\"');
                    switch (propName)
                    {
                        case "firstname":
                            try
                            {
                                var firstNameReturnedRecords = fileCabinetService.FindByFirstName(textToFind);
                                PrintListOfRecords(firstNameReturnedRecords);
                            }
                            catch (ArgumentException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            break;
                        case "lastname":
                            try
                            {
                                var lastNameReturnedRecords = fileCabinetService.FindByLastName(textToFind);
                                PrintListOfRecords(lastNameReturnedRecords);
                            }
                            catch (ArgumentException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            break;
                        case "dateofbirth":
                            try
                            {
                                var birthdayReturnedRecords = fileCabinetService.FindByBirthday(textToFind);
                                PrintListOfRecords(birthdayReturnedRecords);
                            }
                            catch (ArgumentException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            break;
                        default:
                            Console.WriteLine($"Unknown property - {propName}");
                            break;
                    }
                }
            }
        }

        private static void Export(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Command 'export' should contain type and path of file to export.");
            }
            else
            {
                string[] input = parameters.Split(' ', 2);
                if (input.Length != 2)
                {
                    Console.WriteLine("Please check you input.");
                }
                else
                {
                    string typeOfFile = input[0].ToLowerInvariant();
                    string filePath = input[1];
                    switch (typeOfFile)
                    {
                        case "csv":
                            if (!filePath.EndsWith(".csv"))
                            {
                                filePath = string.Concat(filePath, ".csv");
                            }

                            if (File.Exists(filePath))
                            {
                                bool notEnd = true;
                                do
                                {
                                    Console.Write($"File is exist - rewrite {filePath} [Y/n]");
                                    var answer = Console.ReadLine();
                                    if (string.IsNullOrEmpty(answer))
                                    {
                                        break;
                                    }

                                    if (string.Equals(answer.ToLowerInvariant(), "y"))
                                    {
                                        File.Delete(filePath);
                                        notEnd = false;
                                    }
                                    else if (answer.ToLowerInvariant() == "n")
                                    {
                                        break;
                                    }
                                }
                                while (notEnd);
                            }

                            try
                            {
                                FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
                                StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
                                var snapshot = fileCabinetService.MakeSnapshot();
                                snapshot.SaveToCsv(streamWriter);
                                Console.WriteLine($"All records are exported to file {filePath}");
                                streamWriter.Close();
                                fileStream.Close();
                            }
                            catch (Exception)
                            {
                                Console.WriteLine($"Export failed: can't open file {filePath}");
                            }

                            break;
                        case "xml":
                            if (!filePath.EndsWith(".xml"))
                            {
                                filePath = string.Concat(filePath, ".xml");
                            }

                            if (File.Exists(filePath))
                            {
                                bool notEnd = true;
                                do
                                {
                                    Console.Write($"File is exist - rewrite {filePath} [Y/n]");
                                    var answer = Console.ReadLine();
                                    if (string.IsNullOrEmpty(answer))
                                    {
                                        break;
                                    }

                                    if (string.Equals(answer.ToLowerInvariant(), "y"))
                                    {
                                        File.Delete(filePath);
                                        notEnd = false;
                                    }
                                    else if (answer.ToLowerInvariant() == "n")
                                    {
                                        break;
                                    }
                                }
                                while (notEnd);
                            }

                            try
                            {
                                FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
                                StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
                                var snapshot = fileCabinetService.MakeSnapshot();
                                snapshot.SaveToXml(streamWriter);
                                Console.WriteLine($"All records are exported to file {filePath}");
                                streamWriter.Close();
                                fileStream.Close();
                            }
                            catch (Exception)
                            {
                                Console.WriteLine($"Export failed: can't open file {filePath}");
                            }

                            break;
                        default:
                            Console.WriteLine($"Unknown or unsupported type of file - {typeOfFile}");
                            break;
                    }
                }
            }
        }

        private static void Import(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Command 'import' should contain type and path of file from witch read.");
            }
            else
            {
                string[] input = parameters.Split(' ', 2);
                if (input.Length != 2)
                {
                    Console.WriteLine("Please check you input.");
                }
                else
                {
                    string typeOfFile = input[0].ToLowerInvariant();
                    string filePath = input[1];
                    switch (typeOfFile)
                    {
                        case "csv":
                            if (!filePath.EndsWith(".csv"))
                            {
                                filePath = string.Concat(filePath, ".csv");
                            }

                            if (!File.Exists(filePath))
                            {
                                Console.WriteLine($"Import error: file {filePath} is not exist");
                                break;
                            }

                            try
                            {
                                FileStream fileStream = new FileStream(filePath, FileMode.Open);
                                StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
                                var snapshot = fileCabinetService.MakeSnapshot();
                                snapshot.LoadFromCsv(streamReader);
                                int amount = fileCabinetService.Restore(snapshot);
                                Console.WriteLine($"{amount} records were imported from file {filePath}");
                                fileStream.Close();
                                streamReader.Close();
                            }
                            catch (ArgumentNullException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            break;
                        case "xml":
                            if (!filePath.EndsWith(".xml"))
                            {
                                filePath = string.Concat(filePath, ".xml");
                            }

                            if (!File.Exists(filePath))
                            {
                                Console.WriteLine($"Import error: file {filePath} is not exist");
                                break;
                            }

                            try
                            {
                                FileStream fileStream = new FileStream(filePath, FileMode.Open);
                                StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
                                var snapshot = fileCabinetService.MakeSnapshot();
                                snapshot.LoadFromXml(streamReader);
                                int amount = fileCabinetService.Restore(snapshot);
                                Console.WriteLine($"{amount} records were imported from file {filePath}");
                                fileStream.Close();
                                streamReader.Close();
                            }
                            catch (ArgumentNullException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            break;
                        default:
                            Console.WriteLine($"Unknown or unsupported type of file - {typeOfFile}");
                            break;
                    }
                }
            }
        }

        private static void Remove(string parameters)
        {
            int enteredId;
            if (!int.TryParse(parameters, out enteredId))
            {
                Console.WriteLine("Error! Please check inputed Id.");
            }

            var recordsCount = Program.fileCabinetService.GetStat(false);
            if (enteredId <= 0)
            {
                Console.WriteLine("Id should be grater then 0");
            }
            else if (recordsCount < enteredId)
            {
                Console.WriteLine($"#{enteredId} record is not exists.");
            }
            else
            {
                try
                {
                    Program.fileCabinetService.Remove(enteredId);
                    Console.WriteLine($"Record #{enteredId} is removed.");
                }
                catch (ArgumentNullException)
                {
                    Console.WriteLine($"#{enteredId} record is not exists.");
                }
            }
        }

        private static void Purge(string parameters)
        {
            int numberOfDefragmentedRecords = fileCabinetService.Defragment();
            Console.WriteLine($"Data file processing is completed: {numberOfDefragmentedRecords} of {fileCabinetService.GetStat(false) + numberOfDefragmentedRecords} records were purged.");
        }
    }
}