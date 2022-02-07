namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle find command.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> print;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        /// <param name="print">print function.</param>
        public FindCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> print)
            : base(service)
        {
            this.print = print;
        }

        /// <summary>
        /// Handle find command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (string.Equals(request.Command, "find", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(request.Parameters))
                {
                    Console.WriteLine("Command 'find' should contain name of property and text to find.");
                }
                else
                {
                    string[] input = request.Parameters.Split(' ', 2);
                    if (input.Length != 2)
                    {
                        Console.WriteLine("Please check you input.");
                    }
                    else
                    {
                        string propName = input[0].ToUpperInvariant();
                        string textToFind = input[1].Trim('\"');
                        switch (propName)
                        {
                            case "FIRSTNAME":
                                try
                                {
                                    var firstNameReturnedRecords = service.FindByFirstName(textToFind);
                                    this.print.Invoke(firstNameReturnedRecords);
                                }
                                catch (ArgumentNullException exeption)
                                {
                                    Console.WriteLine(exeption.Message);
                                }
                                catch (ArgumentException exeption)
                                {
                                    Console.WriteLine(exeption.Message);
                                }

                                break;
                            case "LASTNAME":
                                try
                                {
                                    var lastNameReturnedRecords = service.FindByLastName(textToFind);
                                    this.print.Invoke(lastNameReturnedRecords);
                                }
                                catch (ArgumentNullException exeption)
                                {
                                    Console.WriteLine(exeption.Message);
                                }
                                catch (ArgumentException exeption)
                                {
                                    Console.WriteLine(exeption.Message);
                                }

                                break;
                            case "DATEOFBIRTH":
                                try
                                {
                                    var birthdayReturnedRecords = service.FindByBirthday(textToFind);
                                    this.print.Invoke(birthdayReturnedRecords);
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
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}
