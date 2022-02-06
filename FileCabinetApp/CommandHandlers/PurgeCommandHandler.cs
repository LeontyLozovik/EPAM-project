namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle purge command.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        public PurgeCommandHandler(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Handle purge command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (string.Equals(request.Command, "purge", StringComparison.OrdinalIgnoreCase))
            {
                int numberOfDefragmentedRecords = this.service.Defragment();
                Console.WriteLine($"Data file processing is completed: {numberOfDefragmentedRecords} of {this.service.GetStat(false) + numberOfDefragmentedRecords} records were purged.");
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}
