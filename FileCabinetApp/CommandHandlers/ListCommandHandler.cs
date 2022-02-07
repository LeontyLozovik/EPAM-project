namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle list command.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private IRecordPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        /// <param name="printer">printer to work with.</param>
        public ListCommandHandler(IFileCabinetService service, IRecordPrinter printer)
            : base(service)
        {
            this.printer = printer;
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
                    var listOfRecords = service.GetRecords();
                    this.printer.Print(listOfRecords);
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
