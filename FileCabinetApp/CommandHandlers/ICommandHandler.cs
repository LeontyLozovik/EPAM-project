namespace FileCabinetApp
{
    /// <summary>
    /// Represent hendlers of app commonds.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Next handler if this handler can't handle.
        /// </summary>
        /// <param name="commandHandler">Next handler.</param>
        public void SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Handle request.
        /// </summary>
        /// <param name="request">request to handle.</param>
        public void Handle(AppCommandRequest request);
    }
}