﻿using FileCabinetApp.CommandHandlers;

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

        /// <summary>
        /// Type of service to use for handling records.
        /// </summary>
        public static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
        private const string DeveloperName = "Leontiy Lozovik";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";

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
            var statCommandHandler = new StatCommandHandle();
            var listCommandHandler = new ListCommandHandler();
            var createCommandHandler = new CreateCommandHandler();
            var editCommandHandler = new EditCommandHandler();
            var findCommandHandler = new FindCommandHandler();
            var importCommandHandler = new ImportCommandHandler();
            var exportCommandHandler = new ExportCommandHandler();
            var removeCommandHandler = new RemoveCommandHandler();
            var purgeCommandHandler = new PurgeCommandHandler();
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
    }
}