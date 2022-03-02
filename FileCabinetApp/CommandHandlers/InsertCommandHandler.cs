using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle insert command.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        public InsertCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handle create command.
        /// </summary>
        /// <param name="request">request with filds and values.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.Equals(request.Command, "insert", StringComparison.OrdinalIgnoreCase))
            {
                string[] fildsArr, valuesArr;
                try
                {
                    GetFildsAndValues(request, out fildsArr, out valuesArr);
                    FileCabinetRecord record = CreateRecordToInsert(fildsArr, valuesArr);
                    service.Insert(record);
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }

        private static void GetFildsAndValues(AppCommandRequest request, out string[] fildsArr, out string[] valuesArr)
        {
            if (string.IsNullOrEmpty(request.Parameters))
            {
                throw new ArgumentNullException("Command 'insert' should contain names of properties and values to insert.", nameof(request.Parameters));
            }

            int index = request.Parameters.IndexOf("values", StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                throw new ArgumentException("Insert command arguments should contain sequence of filds and" +
                    " after keyword 'values' sequence of values of it's filds.");
            }

            StringBuilder filds = new StringBuilder();
            filds.Append(request.Parameters, 0, index);
            StringBuilder values = new StringBuilder();
            values.Append(request.Parameters, index + 6, request.Parameters.Length - index - 6);
            string stringFilds = filds.ToString().Trim().TrimStart('(').TrimEnd(')');
            string stringValues = values.ToString().Trim().TrimStart('(').TrimEnd(')');
            fildsArr = stringFilds.Split(',');
            for (int i = 0; i < fildsArr.Length; i++)
            {
                fildsArr[i] = fildsArr[i].Trim();
            }

            valuesArr = stringValues.Split(',');
            for (int i = 0; i < valuesArr.Length; i++)
            {
                valuesArr[i] = valuesArr[i].Trim().TrimStart('\'').TrimEnd('\'');
            }
        }

        private static FileCabinetRecord CreateRecordToInsert(string[] fildsArr, string[] valuesArr)
        {
            FileCabinetRecord record = new FileCabinetRecord();
            for (int i = 0; i < valuesArr.Length; i++)
            {
                switch (fildsArr[i])
                {
                    case "id":
                        int id;
                        if (!int.TryParse(valuesArr[i], out id))
                        {
                            throw new ArgumentException("Invalid id");
                        }

                        if (id <= 0)
                        {
                            throw new ArgumentException("Id can't be less then 1");
                        }

                        record.Id = id;
                        break;
                    case "firstname":
                        record.FirstName = valuesArr[i];
                        break;
                    case "lastname":
                        record.LastName = valuesArr[i];
                        break;
                    case "dateofbirth":
                        DateTime birthday;
                        if (!DateTime.TryParse(valuesArr[i], CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out birthday))
                        {
                            throw new ArgumentException("Invalid Date");
                        }

                        record.DateOfBirth = birthday;
                        break;
                    case "children":
                        short children;
                        if (!short.TryParse(valuesArr[i], out children))
                        {
                            throw new ArgumentException("Invalid number of children");
                        }

                        record.Children = children;
                        break;
                    case "salary":
                        decimal salary;
                        if (!decimal.TryParse(valuesArr[i], out salary))
                        {
                            throw new ArgumentException("Invalid salary");
                        }

                        record.AverageSalary = salary;
                        break;
                    case "sex":
                        record.Sex = valuesArr[i][0];
                        break;
                    default:
                        break;
                }
            }

            return record;
        }
    }
}
