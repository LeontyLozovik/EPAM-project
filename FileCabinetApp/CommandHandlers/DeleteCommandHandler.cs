using System.Collections.ObjectModel;
using System.Text;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle delete command.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        public DeleteCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handle delete command.
        /// </summary>
        /// <param name="request">request with values to delete.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.Equals(request.Command, "delete", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(request.Parameters))
                {
                    throw new ArgumentNullException("Command 'delete' should contain keyword 'where' and 'fild'='value' expresion at least", nameof(request.Parameters));
                }

                string[] commandArgs = request.Parameters.Split(" ");
                if (!string.Equals(commandArgs[0], "where", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Request should starts with keyword 'where'\nExample: delete where id = '1'");
                }

                string[] splitedArguments = SplitArguments(commandArgs[1..]);
                bool andKeyword = false;
                foreach (var argument in splitedArguments.ToList())
                {
                    if (string.Equals(argument, "and", StringComparison.OrdinalIgnoreCase))
                    {
                        andKeyword = true;
                    }
                }

                ReadOnlyCollection<FileCabinetRecord> recordsToDelete = service.SelectCommand(splitedArguments, andKeyword);
                List<int> idsOfRecordsToDelete = new List<int>();
                foreach (var record in recordsToDelete)
                {
                    idsOfRecordsToDelete.Add(record.Id);
                }

                ReadOnlyCollection<int> collection = service.Delete(new ReadOnlyCollection<int>(idsOfRecordsToDelete));
                StringBuilder stringBuilder = new ();
                if (collection.Count > 0)
                {
                    stringBuilder.Append("Record(s) ");
                    foreach (var item in collection)
                    {
                        stringBuilder.Append($"#{item}, ");
                    }

                    stringBuilder.Remove(stringBuilder.Length - 2, 1);
                }
                else
                {
                    stringBuilder.Append("Nothing ");
                }

                stringBuilder.Append("were deleted.");
                Console.WriteLine(stringBuilder);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }

        private static string[] SplitArguments(string[] arguments)
        {
            List<string> result = new ();
            foreach (var arg in arguments)
            {
                if (!string.Equals(arg, "=", StringComparison.OrdinalIgnoreCase))
                {
                    string[] splitedArgs = arg.Split("=", 2);
                    foreach (var splitedArg in splitedArgs)
                    {
                        if (splitedArg.Length != 0)
                        {
                            result.Add(splitedArg.Trim('\'').Trim().ToUpperInvariant());
                        }
                    }
                }
            }

            return result.ToArray();
        }
    }
}
