namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle edit command.
    /// </summary>
    public class EditCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        public EditCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handle edit command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (string.Equals(request.Command, "edit", StringComparison.OrdinalIgnoreCase))
            {
                int enteredId;
                if (!int.TryParse(request.Parameters, out enteredId))
                {
                    Console.WriteLine("Error! Please check inputed Id.");
                }

                if (enteredId <= 0)
                {
                    Console.WriteLine("Id should be grater then 0");
                }
                else if (!service.IsIdExist(enteredId))
                {
                    Console.WriteLine($"#{enteredId} record is not found.");
                }
                else
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

                        var newRecord = new FileCabinetRecord
                        {
                            Id = enteredId,
                            FirstName = firstName,
                            LastName = lastName,
                            DateOfBirth = birthday,
                            Children = children,
                            AverageSalary = salary,
                            Sex = sex,
                        };

                        try
                        {
                            service.EditRecord(newRecord);
                            Console.WriteLine($"Record #{enteredId} is updated.");
                            flagNotEnd = false;
                        }
                        catch (ArgumentNullException exeption)
                        {
                            Console.WriteLine(exeption.Message);
                            flagNotEnd = false;
                        }
                        catch (ArgumentException exeption)
                        {
                            Console.WriteLine(exeption.Message);
                            flagNotEnd = false;
                        }
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
