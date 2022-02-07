namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle list command.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> print;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        /// <param name="print">print function.</param>
        public ListCommandHandler(IFileCabinetService service, Action<IEnumerable<FileCabinetRecord>> print)
            : base(service)
        {
            this.print = print;
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
                    this.print.Invoke(listOfRecords);
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
