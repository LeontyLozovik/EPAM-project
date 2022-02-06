namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle exit command.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handle exit command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (string.Equals(request.Command, "exit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Exiting an application...");
                Program.isRunning = false;
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}
