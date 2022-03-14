using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using FileCabinetApp.Services;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle update command.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        public UpdateCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handle update command.
        /// </summary>
        /// <param name="request">request with filds and values.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.Equals(request.Command, "update", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(request.Parameters))
                {
                    throw new ArgumentNullException("Command 'update' should contain keyword 'set', new 'fild'='value' and fild(s) to find record.", nameof(request.Parameters));
                }

                var parametersOfBothRecords = GetFildsAndValues(request);
                if (parametersOfBothRecords.Item2.Length % 2 != 0)
                {
                    Console.WriteLine("Number of filds not equal number of values. Please check you input.\nIf you want set fractional salary please use '.' Example : salary='1111.11'");
                }

                bool andKeyword = false;
                if (parametersOfBothRecords.Item1.Contains<string>("and"))
                {
                    andKeyword = true;
                }

                ReadOnlyCollection<FileCabinetRecord> listOfOldRecords = service.SelectCommand(parametersOfBothRecords.Item1, andKeyword);
                if (listOfOldRecords.Count == 0)
                {
                    Console.WriteLine("There isn't any records with this values.");
                    return;
                }

                var result = new List<FileCabinetRecord>();
                for (int i = 0; i < listOfOldRecords.Count; i++)
                {
                    result.Add(CreateNewRecord(listOfOldRecords[i], parametersOfBothRecords.Item2));
                }

                if (service.Update(new ReadOnlyCollection<FileCabinetRecord>(result)))
                {
                    Console.WriteLine("Successfully updated!");
                }
                else
                {
                    Console.WriteLine("Updating error!");
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
                throw new ArgumentNullException("Command 'update' should contain keyword 'set', new 'fild'='value' and fild(s) to find record.", nameof(request.Parameters));
            }

            int setIndex = request.Parameters.IndexOf("set", StringComparison.OrdinalIgnoreCase);
            if (setIndex == -1)
            {
                throw new ArgumentException("Update command arguments should begine with 'set' keyword.\n" +
                    "Example: update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith'");
            }

            int whereIndex = request.Parameters.IndexOf("where", StringComparison.OrdinalIgnoreCase);
            if (whereIndex == -1)
            {
                throw new ArgumentException("Update command arguments should contain 'fild'='value' parm after keyword 'where'\n" +
                    "Example: update set DateOfBirth = '5/18/1986' where FirstName='Stan' and LastName='Smith'");
            }

            StringBuilder newValues = new StringBuilder();
            newValues.Append(request.Parameters, setIndex + 4, whereIndex - 4);
            StringBuilder valuesOfOldRecord = new StringBuilder();
            valuesOfOldRecord.Append(request.Parameters, whereIndex + 6, request.Parameters.Length - whereIndex - 6);
            char[] separators = { '=', ',', ' ' };
            List<string> newRecord = new List<string>(newValues.ToString().Split(separators));
            for (int i = 0; i < newRecord.Count; i++)
            {
                newRecord[i] = newRecord[i].Trim().Trim('\'').Replace('.', ',');
                if (newRecord[i].Length == 0)
                {
                    newRecord.Remove(newRecord[i]);
                    i--;
                }
            }

            List<string> oldRecord = new List<string>(valuesOfOldRecord.ToString().Split(separators));
            for (int i = 0; i < oldRecord.Count; i++)
            {
                oldRecord[i] = oldRecord[i].Trim().Trim('\'');
                if (oldRecord[i].Length == 0)
                {
                    oldRecord.Remove(oldRecord[i]);
                    i--;
                }
            }

            return new Tuple<string[], string[]>(oldRecord.ToArray(), newRecord.ToArray());
        }

        private static FileCabinetRecord CreateNewRecord(FileCabinetRecord oldRecord, string[] fildsAndValues)
        {
            var record = (FileCabinetRecord)oldRecord.Clone();
            for (int i = 0; i < fildsAndValues.Length; i++)
            {
                switch (fildsAndValues[i].ToUpperInvariant())
                {
                    case "ID":
                        throw new ArgumentException("You can't update Id fild.");
                    case "FIRSTNAME":
                        record.FirstName = fildsAndValues[i + 1];
                        i++;
                        break;
                    case "LASTNAME":
                        record.LastName = fildsAndValues[i + 1];
                        i++;
                        break;
                    case "DATEOFBIRTH":
                        DateTime birthday;
                        if (!DateTime.TryParse(fildsAndValues[i + 1], CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out birthday))
                        {
                            throw new ArgumentException("Invalid Date");
                        }

                        record.DateOfBirth = birthday;
                        i++;
                        break;
                    case "CHILDREN":
                        short children;
                        if (!short.TryParse(fildsAndValues[i + 1], out children))
                        {
                            throw new ArgumentException("Invalid number of children");
                        }

                        record.Children = children;
                        i++;
                        break;
                    case "SALARY":
                        decimal salary;
                        if (!decimal.TryParse(fildsAndValues[i + 1], out salary))
                        {
                            throw new ArgumentException("Invalid salary");
                        }

                        record.AverageSalary = salary;
                        i++;
                        break;
                    case "SEX":
                        record.Sex = fildsAndValues[i + 1][0];
                        i++;
                        break;
                    case "AND":
                        break;
                    default:
                        throw new ArgumentException($"Incorrect fildname {fildsAndValues[i]}");
                }
            }

            return record;
        }
    }
}
