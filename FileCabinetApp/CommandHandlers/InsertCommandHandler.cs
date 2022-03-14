using System.Globalization;
using System.Text;
using FileCabinetApp.Services;

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
        /// Handle insert command.
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
                try
                {
                    var fildsAndValues = GetFildsAndValues(request);
                    if (fildsAndValues.Item1.Length != fildsAndValues.Item2.Length)
                    {
                        Console.WriteLine("Number of filds not equal number of values. Please check you input.\nIf you want set fractional salary please use '.' Example : salary='1111.11'");
                        return;
                    }

                    FileCabinetRecord record = CreateRecordToInsert(fildsAndValues.Item1, fildsAndValues.Item2);
                    if (service.Insert(record))
                    {
                        Console.WriteLine("Successfuly inserted!");
                    }
                    else
                    {
                        Console.WriteLine("Inserting error!");
                    }
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

        private static Tuple<string[], string[]> GetFildsAndValues(AppCommandRequest request)
        {
            if (string.IsNullOrEmpty(request.Parameters))
            {
                throw new ArgumentNullException("Command 'insert' should contain names of properties and values to insert.", nameof(request.Parameters));
            }

            int index = request.Parameters.IndexOf("values", StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                throw new ArgumentException(
                    "Insert command arguments should contain sequence of filds and after keyword 'values' " +
                    "sequence of values of it's filds.\nExample: insert(id, firstname, lastname, dateofbirth) values('1', 'John', 'Doe', '5/18/1986')");
            }

            StringBuilder filds = new StringBuilder();
            filds.Append(request.Parameters, 0, index);
            StringBuilder values = new StringBuilder();
            values.Append(request.Parameters, index + 6, request.Parameters.Length - index - 6);
            char[] separators = { '=', ',', ' ' };
            List<string> fildsList = new List<string>(filds.ToString().Trim().TrimStart('(').TrimEnd(')').Split(separators));
            for (int i = 0; i < fildsList.Count; i++)
            {
                fildsList[i] = fildsList[i].Trim().Trim('\'').ToUpperInvariant();
                if (fildsList[i].Length == 0)
                {
                    fildsList.Remove(fildsList[i]);
                    i--;
                }
            }

            List<string> valuesList = new List<string>(values.ToString().Trim().TrimStart('(').TrimEnd(')').Split(separators));
            for (int i = 0; i < valuesList.Count; i++)
            {
                valuesList[i] = valuesList[i].Trim().Trim('\'').Replace('.', ',');
                if (valuesList[i].Length == 0)
                {
                    valuesList.Remove(valuesList[i]);
                    i--;
                }
            }

            return new Tuple<string[], string[]>(fildsList.ToArray(), valuesList.ToArray());
        }

        private static FileCabinetRecord CreateRecordToInsert(string[] fildsArr, string[] valuesArr)
        {
            if (!(fildsArr.ToList().Contains("FIRSTNAME") && fildsArr.ToList().Contains("LASTNAME") && fildsArr.ToList().Contains("DATEOFBIRTH")))
            {
                throw new ArgumentException("Insert command should contain firstname, lastname and dateofbirth to insert at least.");
            }

            FileCabinetRecord record = new FileCabinetRecord();
            for (int i = 0; i < valuesArr.Length; i++)
            {
                switch (fildsArr[i])
                {
                    case "ID":
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
                    case "FIRSTNAME":
                        record.FirstName = valuesArr[i];
                        break;
                    case "LASTNAME":
                        record.LastName = valuesArr[i];
                        break;
                    case "DATEOFBIRTH":
                        DateTime birthday;
                        if (!DateTime.TryParse(valuesArr[i], CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out birthday))
                        {
                            throw new ArgumentException("Invalid Date");
                        }

                        record.DateOfBirth = birthday;
                        break;
                    case "CHILDREN":
                        short children;
                        if (!short.TryParse(valuesArr[i], out children))
                        {
                            throw new ArgumentException("Invalid number of children");
                        }

                        record.Children = children;
                        break;
                    case "SALARY":
                        decimal salary;
                        if (!decimal.TryParse(valuesArr[i], out salary))
                        {
                            throw new ArgumentException("Invalid salary");
                        }

                        record.AverageSalary = salary;
                        break;
                    case "SEX":
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
