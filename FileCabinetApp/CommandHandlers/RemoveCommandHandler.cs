namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle remove command.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handle remove command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (string.Equals(request.Command, "remove", StringComparison.OrdinalIgnoreCase))
            {
                int enteredId;
                if (!int.TryParse(request.Parameters, out enteredId))
                {
                    Console.WriteLine("Error! Please check inputed Id.");
                }

                var recordsCount = Program.fileCabinetService.GetStat(false);
                if (enteredId <= 0)
                {
                    Console.WriteLine("Id should be grater then 0");
                }
                else if (recordsCount < enteredId)
                {
                    Console.WriteLine($"#{enteredId} record is not exists.");
                }
                else
                {
                    try
                    {
                        Program.fileCabinetService.Remove(enteredId);
                        Console.WriteLine($"Record #{enteredId} is removed.");
                    }
                    catch (ArgumentNullException)
                    {
                        Console.WriteLine($"#{enteredId} record is not exists.");
                    }
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}
