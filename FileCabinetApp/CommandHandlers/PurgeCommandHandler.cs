namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle purge command.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handle purge command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (string.Equals(request.Command, "purge", StringComparison.OrdinalIgnoreCase))
            {
                int numberOfDefragmentedRecords = Program.fileCabinetService.Defragment();
                Console.WriteLine($"Data file processing is completed: {numberOfDefragmentedRecords} of {Program.fileCabinetService.GetStat(false) + numberOfDefragmentedRecords} records were purged.");
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}
