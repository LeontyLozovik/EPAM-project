namespace FileCabinetApp
{
    /// <summary>
    /// Represent hendlers of app commonds.
    /// </summary>
    public class CommandHandlerBase : ICommandHandler
    {
        /// <summary>
        /// Next command handler.
        /// </summary>
        protected ICommandHandler? nextHandler;

        /// <summary>
        /// Next handler if this handler can't handle.
        /// </summary>
        /// <param name="commandHandler">Next handler.</param>
        public void SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler;
        }

        /// <summary>
        /// Handle request.
        /// </summary>
        /// <param name="request">request to handle.</param>
        public virtual void Handle(AppCommandRequest request)
        {
            throw new NotImplementedException();
        }
    }
}