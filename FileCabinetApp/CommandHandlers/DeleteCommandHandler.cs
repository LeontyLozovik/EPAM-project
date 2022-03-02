using System.Collections.ObjectModel;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle delete command.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private static string[] keyWords = { "where", "and" };

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        public DeleteCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handle delete command.
        /// </summary>
        /// <param name="request">request with values to delete.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.Equals(request.Command, "delete", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(request.Parameters))
                {
                    throw new ArgumentNullException("Command 'delete' should contain keyword 'where' and 'fild'='value' expresion at least", nameof(request.Parameters));
                }

                string[] commandArgs = request.Parameters.Split(" ");
                if (!string.Equals(commandArgs[0], keyWords[0], StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Request should starts with keyword 'where'");
                }

                string[] splitedArguments = SplitArguments(commandArgs);
                int id = 0;
                int count = 1;
                var listOfIds = new List<int>();
                for (int i = 1; i < splitedArguments.Length; i++)
                {
                    if (!string.Equals(keyWords[1], splitedArguments[i], StringComparison.OrdinalIgnoreCase))
                    {
                        switch (splitedArguments[i])
                        {
                            case "ID":
                                if (!int.TryParse(splitedArguments[i + 1], out id))
                                {
                                    throw new ArgumentException("Incorrect Id.");
                                }

                                listOfIds.Add(id);
                                i++;
                                break;
                            case "FIRSTNAME":
                                try
                                {
                                    var withFirstnameRecords = service.FindByFirstName(splitedArguments[i + 1]);
                                    foreach (FileCabinetRecord record in withFirstnameRecords)
                                    {
                                        listOfIds.Add(record.Id);
                                    }

                                    i++;
                                }
                                catch (ArgumentNullException exeption)
                                {
                                    Console.WriteLine(exeption.Message);
                                }
                                catch (ArgumentException)
                                {
                                }

                                break;
                            case "LASTNAME":
                                try
                                {
                                    var withLastnameRecords = service.FindByLastName(splitedArguments[i + 1]);
                                    foreach (FileCabinetRecord record in withLastnameRecords)
                                    {
                                        listOfIds.Add(record.Id);
                                    }

                                    i++;
                                }
                                catch (ArgumentNullException exeption)
                                {
                                    Console.WriteLine(exeption.Message);
                                }
                                catch (ArgumentException)
                                {
                                }

                                break;
                            case "DATEOFBIRTH":
                                try
                                {
                                    var withDateOfBirthRecords = service.FindByBirthday(splitedArguments[i + 1]);
                                    foreach (FileCabinetRecord record in withDateOfBirthRecords)
                                    {
                                        listOfIds.Add(record.Id);
                                    }

                                    i++;
                                }
                                catch (ArgumentNullException exeption)
                                {
                                    Console.WriteLine(exeption.Message);
                                }
                                catch (ArgumentException)
                                {
                                }

                                break;
                            default:
                                Console.WriteLine($"Incorrect name of field: {splitedArguments[i]}");
                                return;
                        }
                    }
                    else
                    {
                        count++;
                    }
                }

                listOfIds.Sort();
                ReadOnlyCollection<int> result = GetIdOfRecordsToDelete(listOfIds, count);
                ReadOnlyCollection<int> collection = service.Delete(result);
                StringBuilder stringBuilder = new StringBuilder();
                if (collection.Count > 0)
                {
                    stringBuilder.Append("Record(s) ");
                    foreach (var item in collection)
                    {
                        stringBuilder.Append($"#{item}, ");
                    }

                    stringBuilder.Remove(stringBuilder.Length - 2, 1);
                }
                else
                {
                    stringBuilder.Append("Nothing ");
                }

                stringBuilder.Append("were deleted.");
                Console.WriteLine(stringBuilder);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }

        private static string[] SplitArguments(string[] arguments)
        {
            List<string> result = new List<string>();
            foreach (var arg in arguments)
            {
                if (!string.Equals(arg, "=", StringComparison.OrdinalIgnoreCase))
                {
                    string[] splitedArgs = arg.Split("=", 2);
                    foreach (var splitedArg in splitedArgs)
                    {
                        result.Add(splitedArg.Trim('\'').Trim().ToUpperInvariant());
                    }
                }
            }

            return result.ToArray();
        }

        private static ReadOnlyCollection<int> GetIdOfRecordsToDelete(List<int> listOfIds, int minCount)
        {
            var result = new List<int>();
            if (listOfIds.Count < minCount)
            {
                return new ReadOnlyCollection<int>(result);
            }

            if (listOfIds.Count == 1)
            {
                return new ReadOnlyCollection<int>(listOfIds);
            }

            int counter = 1;
            int maxCount = 0;
            for (int i = 0; i < listOfIds.Count; i++)
            {
                if (i + 1 == listOfIds.Count)
                {
                    if (listOfIds[i] == listOfIds[i - 1])
                    {
                        if (counter == maxCount)
                        {
                            if (counter >= minCount)
                            {
                                result.Add(listOfIds[i]);
                            }

                            counter = 1;
                        }
                        else if (counter > maxCount)
                        {
                            if (counter >= minCount)
                            {
                                result.Clear();
                                result.Add(listOfIds[i]);
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
                                result.Add(listOfIds[i]);
                            }

                            counter = 1;
                        }
                        else if (counter > maxCount)
                        {
                            if (counter >= minCount)
                            {
                                result.Clear();
                                result.Add(listOfIds[i]);
                            }

                            maxCount = counter;
                            counter = 1;
                        }
                    }

                    return new ReadOnlyCollection<int>(result);
                }

                if (listOfIds[i] == listOfIds[i + 1])
                {
                    counter++;
                }
                else
                {
                    if (counter == maxCount)
                    {
                        if (counter >= minCount)
                        {
                            result.Add(listOfIds[i]);
                        }

                        counter = 1;
                    }
                    else if (counter > maxCount)
                    {
                        if (counter >= minCount)
                        {
                            result.Clear();
                            result.Add(listOfIds[i]);
                        }

                        maxCount = counter;
                        counter = 1;
                    }
                }
            }

            return new ReadOnlyCollection<int>(result);
        }
    }
}
