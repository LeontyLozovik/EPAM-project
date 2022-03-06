using FileCabinetApp.CommandHandlers;
using FileCabinetApp.RecordValidators;

namespace FileCabinetApp
{
    /// <summary>
    /// Get user requests and calls the desired hendler.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Leontiy Lozovik";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private static bool isRunning = true;
        private static ValidationType validationType;
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new ValidatorBuilder().CreateDefault(SetValidationType));

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
                try
                {
                    commandHandler.Handle(
                        new AppCommandRequest
                        {
                            Command = command,
                            Parameters = parameters,
                        });
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            while (isRunning);
        }

        /// <summary>
        /// Get currient validation type.
        /// </summary>
        /// <returns>Validation type.</returns>
        public static ValidationType GetValidationType()
        {
            return validationType;
        }

        private static void SetValidationType(ValidationType type)
        {
            validationType = type;
        }

        private static void ChangeRunning(bool setRunnig)
        {
            isRunning = setRunnig;
        }

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth:yyyy-MMM-dd}, {record.Children}, {record.AverageSalary}, {record.Sex}");
            }
        }

        private static void DefaultRecordPrint(FileCabinetRecord record)
        {
            Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth:yyyy-MMM-dd}, {record.Children}, {record.AverageSalary}, {record.Sex}");
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpCommandHandler = new HelpCommandHandler();
            var exitCommandHandler = new ExitCommandHandler(ChangeRunning);
            var statCommandHandler = new StatCommandHandle(Program.fileCabinetService);
            var listCommandHandler = new ListCommandHandler(Program.fileCabinetService, DefaultRecordPrint);
            var createCommandHandler = new CreateCommandHandler(Program.fileCabinetService);
            var findCommandHandler = new FindCommandHandler(Program.fileCabinetService, DefaultRecordPrint);
            var importCommandHandler = new ImportCommandHandler(Program.fileCabinetService);
            var exportCommandHandler = new ExportCommandHandler(Program.fileCabinetService);
            var purgeCommandHandler = new PurgeCommandHandler(Program.fileCabinetService);
            var insertCommandHandler = new InsertCommandHandler(Program.fileCabinetService);
            var deleteCommandHandler = new DeleteCommandHandler(Program.fileCabinetService);
            var updateCommandHandler = new UpdateCommandHandler(Program.fileCabinetService);
            var missedCommandHandler = new MissedCommandHandler();
            listCommandHandler.SetNext(createCommandHandler);
            createCommandHandler.SetNext(importCommandHandler);
            importCommandHandler.SetNext(findCommandHandler);
            findCommandHandler.SetNext(purgeCommandHandler);
            purgeCommandHandler.SetNext(statCommandHandler);
            statCommandHandler.SetNext(exportCommandHandler);
            exportCommandHandler.SetNext(helpCommandHandler);
            helpCommandHandler.SetNext(insertCommandHandler);
            insertCommandHandler.SetNext(deleteCommandHandler);
            deleteCommandHandler.SetNext(updateCommandHandler);
            updateCommandHandler.SetNext(exitCommandHandler);
            exitCommandHandler.SetNext(missedCommandHandler);
            return listCommandHandler;
        }

        private static IRecordValidator SwitchValidationRules(string argument)
        {
            switch (argument)
            {
                case "DEFAULT":
                    Console.WriteLine("Using default validation rules.");
                    return new ValidatorBuilder().CreateDefault(SetValidationType);
                case "CUSTOM":
                    Console.WriteLine("Using custom validation rules.");
                    return new ValidatorBuilder().CreateCustom(SetValidationType);
                default:
                    Console.WriteLine("There is only default and custom validation. Default rules will be set.");
                    return new ValidatorBuilder().CreateDefault(SetValidationType);
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

        private static void SetValidator(string inputLine)
        {
            string argument = inputLine.ToUpperInvariant();
            fileCabinetService = new FileCabinetMemoryService(SwitchValidationRules(argument));
        }

        private static void SetStorage(string inputLine)
        {
            string argument = inputLine.ToUpperInvariant();
            IRecordValidator validator = new ValidatorBuilder().CreateDefault(SetValidationType);
            if (GetValidationType() == ValidationType.Custom)
            {
                validator = new ValidatorBuilder().CreateCustom(SetValidationType);
            }

            fileCabinetService = SwitchStorageVariant(argument, validator);
        }

        private static void SetAddServices(string inputLine, ref bool priorityArguments)
        {
            switch (inputLine)
            {
                case "use-stopwatch":
                    fileCabinetService = new ServiceMeter(fileCabinetService);
                    priorityArguments = false;
                    Console.WriteLine("Using service meter.");
                    break;
                case "use-logger":
                    fileCabinetService = new ServiceLogger(fileCabinetService);
                    Console.WriteLine("Using logging");
                    priorityArguments = false;
                    break;
            }
        }

        private static void ReadCmdArgument(string[] args, ref IFileCabinetService fileCabinetService)
        {
            List<string> cmdComands = new List<string>() { "--validation-rules", "--storage", "-v", "-s", "use-stopwatch", "use-logger" };
            bool priorityArguments = true;
            for (int i = 0; i < args.Length; i++)
            {
                string[] inputLine = new string[2];
                if (args[i].Contains('=', StringComparison.OrdinalIgnoreCase))
                {
                    inputLine = args[i] != null ? args[i].Split('=', 2) : new string[] { string.Empty, string.Empty };
                }
                else
                {
                    inputLine[0] = args[i];
                }

                if (cmdComands.Contains(inputLine[0]) && priorityArguments)
                {
                    switch (inputLine[0])
                    {
                        case "--validation-rules":
                            SetValidator(inputLine[1]);
                            break;
                        case "--storage":
                            SetStorage(inputLine[1]);
                            break;
                        case "-v":
                            SetValidator(args[i + 1]);
                            break;
                        case "-s":
                            SetStorage(args[i + 1]);
                            break;
                        default:
                            SetAddServices(inputLine[0], ref priorityArguments);
                            break;
                    }
                }
                else if (cmdComands.Contains(inputLine[0]))
                {
                    SetAddServices(inputLine[0], ref priorityArguments);
                }
            }
        }
    }
}