using System.Globalization;

namespace FileCabinetApp
{
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
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints records statistics", "The 'stat' command prints records statistics." },
            new string[] { "create", "create record whith information about you", "The 'create' command create record whith information about you." },
            new string[] { "list", "prints all existing records", "The 'list' command prints all existing records" },
            new string[] { "edit", "edit your record by Id", "The 'edit' command edit your record by Id" },
            new string[] { "find", "find records by one parameter", "The 'find' command find records by one parameter" },
        };

        private static FileCabinetService fileCabinetService = new FileCabinetService();

        public static void Main(string[] args)
        {
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

        private static void PrintListOfRecords(FileCabinetRecord[] recordToPrint)
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
                var firstName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
                {
                    Console.WriteLine("Incorrect first name! First name should be grater then 2, less then 60 and can't be null or white space.");
                    continue;
                }

                Console.Write("Last name: ");
                var lastName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
                {
                    Console.WriteLine("Incorrect last name! Last name should be grater then 2, less then 60 and can't be null or white space.");
                    continue;
                }

                Console.Write("Date of birth: ");
                DateTime birthday;
                if (!DateTime.TryParse(Console.ReadLine(), CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out birthday))
                {
                    Console.WriteLine("Error! Please check your date of birth input.");
                    continue;
                }

                DateTime oldest = new DateTime(1950, 1, 1);
                DateTime now = DateTime.Now;
                if (birthday < oldest || birthday > now)
                {
                    Console.WriteLine("Sorry but minimal date of birth - 01-Jan-1950 and maxsimum - current date");
                    continue;
                }

                Console.Write("Number of children: ");
                short children;
                if (!short.TryParse(Console.ReadLine(), out children))
                {
                    Console.WriteLine("Error! Please check your number of children input");
                    continue;
                }
                else if (children < 0)
                {
                    Console.WriteLine("Number of children can't be less then 0.");
                    continue;
                }

                Console.Write("Averege salary: ");
                decimal salary;
                if (!decimal.TryParse(Console.ReadLine(), out salary))
                {
                    Console.WriteLine("Error! Please check your salary input");
                    continue;
                }
                else if (salary < 0 || salary > 1000000000)
                {
                    Console.WriteLine("Average salary can't be less then 0 or grater then 1 billion.");
                    continue;
                }

                Console.Write("Sex (m - men, w - women): ");
                char sex = (char)Console.Read();
                if (sex != 'm' && sex != 'w')
                {
                    Console.WriteLine("Sorry, but your sex can be m - men or w - women only.");
                    continue;
                }

                var id = Program.fileCabinetService.CreateRecord(firstName, lastName, birthday, children, salary, sex);
                Console.WriteLine($"Record #{id} is created.");
                flagNotEnd = false;
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

            var recordsCount = Program.fileCabinetService.GetStat();
            if (enteredId <= 0)
            {
                Console.WriteLine("Id should be grater then 0");
            }
            else if (recordsCount < enteredId)
            {
                Console.WriteLine($"#{enteredId} record is not found.");
            }
            else
            {
                bool flagNotEnd = true;
                while (flagNotEnd)
                {
                    Console.Write("First name: ");
                    var firstName = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
                    {
                        Console.WriteLine("Incorrect first name! First name should be grater then 2, less then 60 and can't be null or white space.");
                        continue;
                    }

                    Console.Write("Last name: ");
                    var lastName = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
                    {
                        Console.WriteLine("Incorrect last name! Last name should be grater then 2, less then 60 and can't be null or white space.");
                        continue;
                    }

                    Console.Write("Date of birth: ");
                    DateTime birthday;
                    if (!DateTime.TryParse(Console.ReadLine(), CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out birthday))
                    {
                        Console.WriteLine("Error! Please check your date of birth input.");
                        continue;
                    }

                    DateTime oldest = new DateTime(1950, 1, 1);
                    DateTime now = DateTime.Now;
                    if (birthday < oldest || birthday > now)
                    {
                        Console.WriteLine("Sorry but minimal date of birth - 01-Jan-1950 and maxsimum - current date");
                        continue;
                    }

                    Console.Write("Number of children: ");
                    short children;
                    if (!short.TryParse(Console.ReadLine(), out children))
                    {
                        Console.WriteLine("Error! Please check your number of children input");
                        continue;
                    }
                    else if (children < 0)
                    {
                        Console.WriteLine("Number of children can't be less then 0.");
                        continue;
                    }

                    Console.Write("Averege salary: ");
                    decimal salary;
                    if (!decimal.TryParse(Console.ReadLine(), out salary))
                    {
                        Console.WriteLine("Error! Please check your salary input");
                        continue;
                    }
                    else if (salary < 0 || salary > 1000000000)
                    {
                        Console.WriteLine("Average salary can't be less then 0 or grater then 1 billion.");
                        continue;
                    }

                    Console.Write("Sex (m - men, w - women): ");
                    char sex = (char)Console.Read();
                    if (sex != 'm' && sex != 'w')
                    {
                        Console.WriteLine("Sorry, but your sex can be m - men or w - women only.");
                        continue;
                    }

                    try
                    {
                        Program.fileCabinetService.EditRecord(enteredId, firstName, lastName, birthday, children, salary, sex);
                    }
                    catch (ArgumentException exeption)
                    {
                        Console.WriteLine(exeption.Message);
                    }
                    finally
                    {
                        Console.WriteLine($"Record #{enteredId} is updated.");
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
                string propName = input[0].ToLowerInvariant();
                if (input.Length != 2)
                {
                    Console.WriteLine("Please check you input.");
                }
                else
                {
                    string textToFind = input[1].Trim('\"');
                    switch (propName)
                    {
                        case "firstname":
                            var firstNameReturnedRecords = fileCabinetService.FindByFirstName(textToFind);
                            PrintListOfRecords(firstNameReturnedRecords);
                            break;
                        case "lastname":
                            var lastNameReturnedRecords = fileCabinetService.FindByLastName(textToFind);
                            PrintListOfRecords(lastNameReturnedRecords);
                            break;
                        case "dateofbirth":
                            var birthdayReturnedRecords = fileCabinetService.FindByBirthday(textToFind);
                            PrintListOfRecords(birthdayReturnedRecords);
                            break;
                        default:
                            Console.WriteLine($"Unknown property - {propName}");
                            break;
                    }
                }
            }
        }
    }
}