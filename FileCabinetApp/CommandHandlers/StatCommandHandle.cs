namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle stat command.
    /// </summary>
    public class StatCommandHandle : CommandHandlerBase
    {
        /// <summary>
        /// Handle stat command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (string.Equals(request.Command, "stat", StringComparison.OrdinalIgnoreCase))
            {
                var recordsCount = Program.fileCabinetService.GetStat();
                Console.WriteLine($"{recordsCount} record(s).");
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}
