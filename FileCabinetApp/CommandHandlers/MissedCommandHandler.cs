namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle missed command.
    /// </summary>
    public class MissedCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handle missed command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            Console.WriteLine($"There is no '{request.Command}' command.");
            Console.WriteLine();
        }
    }
}