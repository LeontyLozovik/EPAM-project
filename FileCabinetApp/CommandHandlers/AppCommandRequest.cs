namespace FileCabinetApp
{
    /// <summary>
    /// Represent requests of FileCabinetApp.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Gets or sets get or set app command.
        /// </summary>
        /// <value>App command.</value>
        public string? Command { get; set; }

        /// <summary>
        /// Gets or sets get or set app parameters.
        /// </summary>
        /// <value>App parametrs.</value>
        public string? Parameters { get; set; }
    }
}