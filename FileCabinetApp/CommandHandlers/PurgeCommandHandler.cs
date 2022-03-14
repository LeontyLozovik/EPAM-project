using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle purge command.
    /// </summary>
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        public PurgeCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handle purge command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.Equals(request.Command, "purge", StringComparison.OrdinalIgnoreCase))
            {
                if (!(request.Parameters is null))
                {
                    if (request.Parameters.Length != 0)
                    {
                        throw new ArgumentException("Purge command should not contain any parameters.");
                    }
                }

                int numberOfDefragmentedRecords = service.Defragment();
                Console.WriteLine($"Data file processing is completed: {numberOfDefragmentedRecords} of {service.GetStat(false) + numberOfDefragmentedRecords} records were purged.");
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}
