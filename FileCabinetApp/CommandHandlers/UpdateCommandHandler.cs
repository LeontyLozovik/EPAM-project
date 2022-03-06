using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

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
                ReadOnlyCollection<FileCabinetRecord> listOfOldRecords = FindOldRecord(parametersOfBothRecords.Item1);
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
                    Console.WriteLine("Something went wrong!");
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
                throw new ArgumentException("Update command arguments should begine with 'set' keyword.");
            }

            int whereIndex = request.Parameters.IndexOf("where", StringComparison.OrdinalIgnoreCase);
            if (whereIndex == -1)
            {
                throw new ArgumentException("Update command arguments should contain 'fild'='value' parm after keyword 'where'.");
            }

            StringBuilder newValues = new StringBuilder();
            newValues.Append(request.Parameters, setIndex + 4, whereIndex - 4);
            StringBuilder valuesOfOldRecord = new StringBuilder();
            valuesOfOldRecord.Append(request.Parameters, whereIndex + 6, request.Parameters.Length - whereIndex - 6);
            char[] separators = { '=', ',', ' ' };
            List<string> newRecord = new List<string>(newValues.ToString().Split(separators));
            for (int i = 0; i < newRecord.Count; i++)
            {
                newRecord[i] = newRecord[i].Trim().Trim('\'');
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
                        int id;
                        if (!int.TryParse(fildsAndValues[i + 1], out id))
                        {
                            throw new ArgumentException("Invalid id");
                        }

                        if (id <= 0)
                        {
                            throw new ArgumentException("Id can't be less then 1");
                        }

                        record.Id = id;
                        i++;
                        break;
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

        private static ReadOnlyCollection<FileCabinetRecord> FindOldRecord(string[] oldValues)
        {
            int id = 0;
            int count = 1;
            var listOfRecords = new List<FileCabinetRecord>();
            for (int i = 0; i < oldValues.Length; i++)
            {
                if (!string.Equals("and", oldValues[i], StringComparison.OrdinalIgnoreCase))
                {
                    switch (oldValues[i].ToUpperInvariant())
                    {
                        case "ID":
                            if (!int.TryParse(oldValues[i + 1], out id))
                            {
                                throw new ArgumentException("Incorrect Id.");
                            }

                            FileCabinetRecord? recordById = service.GetRecordById(id);
                            if (!(recordById is null))
                            {
                                listOfRecords.Add(recordById);
                            }

                            i++;
                            break;
                        case "FIRSTNAME":
                            try
                            {
                                var withFirstnameRecords = service.FindByFirstName(oldValues[i + 1]);
                                foreach (FileCabinetRecord record in withFirstnameRecords)
                                {
                                    listOfRecords.Add(record);
                                }
                            }
                            catch (ArgumentNullException exeption)
                            {
                                Console.WriteLine(exeption.Message);
                            }
                            catch (ArgumentException)
                            {
                            }

                            i++;
                            break;
                        case "LASTNAME":
                            try
                            {
                                var withLastnameRecords = service.FindByLastName(oldValues[i + 1]);
                                foreach (FileCabinetRecord record in withLastnameRecords)
                                {
                                    listOfRecords.Add(record);
                                }
                            }
                            catch (ArgumentNullException exeption)
                            {
                                Console.WriteLine(exeption.Message);
                            }
                            catch (ArgumentException)
                            {
                            }

                            i++;
                            break;
                        case "DATEOFBIRTH":
                            try
                            {
                                var withDateOfBirthRecords = service.FindByBirthday(oldValues[i + 1]);
                                foreach (FileCabinetRecord record in withDateOfBirthRecords)
                                {
                                    listOfRecords.Add(record);
                                }
                            }
                            catch (ArgumentNullException exeption)
                            {
                                Console.WriteLine(exeption.Message);
                            }
                            catch (ArgumentException)
                            {
                            }

                            i++;
                            break;
                        default:
                            Console.WriteLine($"Incorrect name of field: {oldValues[i]}");
                            i++;
                            break;
                    }
                }
                else
                {
                    count++;
                }
            }

            listOfRecords.Sort((x, y) =>
            {
                return x.Id.CompareTo(y.Id);
            });
            return GetIdOfRecordsToUpdate(new ReadOnlyCollection<FileCabinetRecord>(listOfRecords), id, count);
        }

        private static ReadOnlyCollection<FileCabinetRecord> GetIdOfRecordsToUpdate(ReadOnlyCollection<FileCabinetRecord> listOfRecords, int id, int minCount)
        {
            var result = new List<FileCabinetRecord>();
            if (listOfRecords.Count < minCount)
            {
                return new ReadOnlyCollection<FileCabinetRecord>(result);
            }

            if (listOfRecords.Count == 1)
            {
                return listOfRecords;
            }

            int counter = 1;
            int maxCount = 0;
            for (int i = 0; i < listOfRecords.Count; i++)
            {
                if (i + 1 == listOfRecords.Count)
                {
                    if (listOfRecords[i].Id == listOfRecords[i - 1].Id)
                    {
                        if (counter == maxCount)
                        {
                            if (counter >= minCount)
                            {
                                result.Add(listOfRecords[i]);
                            }

                            counter = 1;
                        }
                        else if (counter > maxCount)
                        {
                            if (counter >= minCount)
                            {
                                result.Clear();
                                result.Add(listOfRecords[i]);
                            }

                            maxCount = counter;
                            counter = 1;
                        }
                    }
                    else
                    {
                        if (counter == maxCount)
                        {
                            if (counter >= minCount)
                            {
                                result.Add(listOfRecords[i]);
                            }

                            counter = 1;
                        }
                        else if (counter > maxCount)
                        {
                            if (counter >= minCount)
                            {
                                result.Clear();
                                result.Add(listOfRecords[i]);
                            }

                            maxCount = counter;
                            counter = 1;
                        }
                    }

                    return new ReadOnlyCollection<FileCabinetRecord>(result);
                }

                if (listOfRecords[i].Id == listOfRecords[i + 1].Id)
                {
                    counter++;
                }
                else
                {
                    if (counter == maxCount)
                    {
                        if (counter >= minCount)
                        {
                            result.Add(listOfRecords[i]);
                        }

                        counter = 1;
                    }
                    else if (counter > maxCount)
                    {
                        if (counter >= minCount)
                        {
                            result.Clear();
                            result.Add(listOfRecords[i]);
                        }

                        maxCount = counter;
                        counter = 1;
                    }
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(result);
        }
    }
}
