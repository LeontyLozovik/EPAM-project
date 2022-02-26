namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle remove command.
    /// </summary>
    public class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        public RemoveCommandHandler(IFileCabinetService service)
             : base(service)
        {
        }

        /// <summary>
        /// Handle remove command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.Equals(request.Command, "remove", StringComparison.OrdinalIgnoreCase))
            {
                int enteredId;
                if (!int.TryParse(request.Parameters, out enteredId))
                {
                    Console.WriteLine("Error! Please check inputed Id.");
                }

                if (enteredId <= 0)
                {
                    Console.WriteLine("Id should be grater then 0");
                }
                else if (!service.IsIdExist(enteredId))
                {
                    Console.WriteLine($"#{enteredId} record is not exists.");
                }
                else
                {
                    try
                    {
                        service.Remove(enteredId);
                        Console.WriteLine($"Record #{enteredId} is removed.");
                    }
                    catch (ArgumentNullException)
                    {
                        Console.WriteLine($"#{enteredId} record is not exists.");
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
