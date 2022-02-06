namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle list command.
    /// </summary>
    public class ListCommandHandler : CommandHandlerBase
    {
        private IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        public ListCommandHandler(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Handle list command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (string.Equals(request.Command, "list", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var listOfRecords = this.service.GetRecords();
                    PrintListOfRecords(listOfRecords);
                }
                catch (ArgumentNullException)
                {
                    Console.WriteLine("Instance doesn't exist.");
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}
