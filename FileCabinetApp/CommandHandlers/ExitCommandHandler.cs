namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle exit command.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private Action<bool> exit;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="exit">Exit function.</param>
        public ExitCommandHandler(Action<bool> exit)
        {
            this.exit = exit;
        }

        /// <summary>
        /// Handle exit command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.Equals(request.Command, "exit", StringComparison.OrdinalIgnoreCase))
            {
                if (!(request.Parameters is null))
                {
                    if (request.Parameters.Length != 0)
                    {
                        throw new ArgumentException("Exit command should not contain any parameters.");
                    }
                }

                Console.WriteLine("Exiting an application...");
                this.exit.Invoke(false);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}
