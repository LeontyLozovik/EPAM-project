namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle stat command.
    /// </summary>
    public class StatCommandHandle : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandle"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        public StatCommandHandle(IFileCabinetService service)
             : base(service)
        {
        }

        /// <summary>
        /// Handle stat command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (string.Equals(request.Command, "stat", StringComparison.OrdinalIgnoreCase))
            {
                var recordsCount = this.service.GetStat();
                Console.WriteLine($"{recordsCount} record(s).");
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}
