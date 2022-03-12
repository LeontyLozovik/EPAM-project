namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle help command.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints records statistics", "The 'stat' command prints records statistics." },
            new string[] { "create", "create record with information about you", "The 'create' command create record with information about you." },
            new string[] { "export", "exporting service records to files of a certain type", "The 'export' command exporting service records to files of a certain type" },
            new string[] { "import", "importing service records from files of a certain type", "The 'import' command importing service records from files of a certain type" },
            new string[] { "purge", "defragments the data file", "The 'purge' command defragments the data file" },
            new string[] { "insert", "insert records with given filds and values", "The 'insert' command insert records with given filds and values" },
            new string[] { "delete", "delete records with given criteries", "The 'delete' command delete records with given criteries" },
            new string[] { "update", "update records with given criteries", "The 'update' command delete records with given criteries" },
            new string[] { "select", "displays required filds that satisfy the search criteria", "The 'select' command displays required filds that satisfy the search criteria" },
        };

        /// <summary>
        /// Handle help command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.Equals(request.Command, "help", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(request.Parameters))
                {
                    var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], request.Parameters, StringComparison.OrdinalIgnoreCase));
                    if (index >= 0)
                    {
                        Console.WriteLine(helpMessages[index][ExplanationHelpIndex]);
                    }
                    else
                    {
                        Console.WriteLine($"There is no explanation for '{request.Parameters}' command.");
                    }
                }
                else
                {
                    Console.WriteLine("Available commands:");

                    foreach (var helpMessage in helpMessages)
                    {
                        Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                    }
                }

                Console.WriteLine();
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}
