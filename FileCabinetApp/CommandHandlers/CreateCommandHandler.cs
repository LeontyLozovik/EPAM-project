namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle create command.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        public CreateCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handle create command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (string.Equals(request.Command, "create", StringComparison.OrdinalIgnoreCase))
            {
                bool flagNotEnd = true;
                while (flagNotEnd)
                {
                    Console.Write("First name: ");
                    var firstName = ReadInput(StringConverter, FirstNameValidator);

                    Console.Write("Last name: ");
                    var lastName = ReadInput(StringConverter, LastNameValidator);

                    Console.Write("Date of birth: ");
                    var birthday = ReadInput(DateConverter, DateOfBirthValidator);

                    Console.Write("Number of children: ");
                    short children = ReadInput(ShortConverter, NumberOfChildrenValidator);

                    Console.Write("Averege salary: ");
                    decimal salary = ReadInput(DecimalConverter, AverageSalaryValidator);

                    Console.Write("Sex (m - men, w - women): ");
                    char sex = ReadInput(CharConverter, SexValidator);

                    var record = new FileCabinetRecord
                    {
                        Id = 0,
                        FirstName = firstName,
                        LastName = lastName,
                        DateOfBirth = birthday,
                        Children = children,
                        AverageSalary = salary,
                        Sex = sex,
                    };

                    try
                    {
                        var id = service.CreateRecord(record);
                        Console.WriteLine($"Record #{id} is created.");
                        flagNotEnd = false;
                    }
                    catch (ArgumentNullException ex)
                    {
                        Console.WriteLine(ex.Message);
                        flagNotEnd = false;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        flagNotEnd = false;
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