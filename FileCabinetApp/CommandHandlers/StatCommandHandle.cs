using FileCabinetApp.Services;

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
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.Equals(request.Command, "stat", StringComparison.OrdinalIgnoreCase))
            {
                if (!(request.Parameters is null))
                {
                    if (request.Parameters.Length != 0)
                    {
                        throw new ArgumentException("Stat command should not contain any parameters.");
                    }
                }

                var recordsCount = service.GetStat();
                Console.WriteLine($"{recordsCount} record(s).");
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}
