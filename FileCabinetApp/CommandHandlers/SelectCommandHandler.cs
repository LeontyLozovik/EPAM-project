using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle select command.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="service">service to work with.</param>
        public SelectCommandHandler(IFileCabinetService service)
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

            if (string.Equals(request.Command, "select", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(request.Parameters))
                {
                    var allRecords = service.GetRecords();
                    var allFilds = new List<string>() { "id", "firstname", "lastname", "dateofbirth", "children", "salary", "sex" };
                    Write(allFilds, allRecords);
                    return;
                }

                var parametersOfBothRecords = GetFildsAndCriteries(request);
                bool andKeyword = false;
                bool orKeyword = false;
                foreach (var item in parametersOfBothRecords.Item2)
                {
                    if (string.Equals(item, "and", StringComparison.OrdinalIgnoreCase))
                    {
                        andKeyword = true;
                    }
                    else if (string.Equals(item, "or", StringComparison.OrdinalIgnoreCase))
                    {
                        orKeyword = true;
                    }
                }

                if (andKeyword && orKeyword)
                {
                    throw new ArgumentException("Sorry but can't use keyword 'and' at same time with keyword 'or'.");
                }
                else if (!andKeyword && !orKeyword)
                {
                    throw new ArgumentException("You should write at least 2 criteries.");
                }

                var selectedRecords = service.SelectCommand(parametersOfBothRecords.Item2, andKeyword);
                Write(new List<string>(parametersOfBothRecords.Item1), selectedRecords);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }

        private static Tuple<string[], string[]> GetFildsAndCriteries(AppCommandRequest request)
        {
            if (string.IsNullOrEmpty(request.Parameters))
            {
                throw new ArgumentNullException("Command 'select' should contain number of filds to display and at least 2 criteries to find after keyword 'where'.", nameof(request.Parameters));
            }

            int index = request.Parameters.IndexOf("where", StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                throw new ArgumentException("Select command arguments should contain 'fild'='value' parm after keyword 'where'.");
            }

            StringBuilder filds = new StringBuilder();
            filds.Append(request.Parameters, 0, index);
            StringBuilder criteriesToFind = new StringBuilder();
            criteriesToFind.Append(request.Parameters, index + 6, request.Parameters.Length - index - 6);
            char[] separators = { '=', ',', ' ' };

            List<string> fildsArr = new List<string>(filds.ToString().Trim().Split(separators));
            for (int i = 0; i < fildsArr.Count; i++)
            {
                fildsArr[i] = fildsArr[i].Trim();
                if (fildsArr[i].Length == 0)
                {
                    fildsArr.Remove(fildsArr[i]);
                    i--;
                }
            }

            List<string> criteriesArr = new List<string>(criteriesToFind.ToString().Trim().Split(separators));
            for (int i = 0; i < criteriesArr.Count; i++)
            {
                criteriesArr[i] = criteriesArr[i].Trim().Trim('\'');
                if (criteriesArr[i].Length == 0)
                {
                    criteriesArr.Remove(criteriesArr[i]);
                    i--;
                }
            }

            return new Tuple<string[], string[]>(fildsArr.ToArray(), criteriesArr.ToArray());
        }

        private static void Write(List<string> filds, ReadOnlyCollection<FileCabinetRecord> records)
        {
            if (records.Count == 0)
            {
                Console.WriteLine("No one record found.");
                return;
            }

            for (int i = 0; i < filds.Count; i++)
            {
                filds[i] = filds[i].ToUpperInvariant();
            }

            var lineAndHeader = CreateLineAndHeader(filds, records);
            StringBuilder line = lineAndHeader.Item1;
            StringBuilder header = lineAndHeader.Item2;
            Console.WriteLine(line);
            Console.WriteLine(header);
            foreach (var record in records)
            {
                Console.WriteLine(line);
                StringBuilder padding = CreatePadding(filds, record, lineAndHeader.Item3);
                Console.WriteLine(padding);
            }

            Console.WriteLine(line);
        }

        private static Tuple<StringBuilder, StringBuilder, List<int>> CreateLineAndHeader(List<string> filds, ReadOnlyCollection<FileCabinetRecord> records)
        {
            StringBuilder line = new StringBuilder("+");
            StringBuilder header = new StringBuilder("|");
            var offsets = new List<int>();
            int fullLenghts = 1;
            if (filds.Contains("ID"))
            {
                int maxLenghts = "Id".Length;
                foreach (var item in records)
                {
                    int lenght = item.Id.ToString(new CultureInfo("en-US")).Length;
                    if (maxLenghts < lenght)
                    {
                        maxLenghts = lenght;
                    }
                }

                maxLenghts += 2;
                offsets.Add(maxLenghts);
                line.Append(new string('-', maxLenghts));
                line.Append('+');

                header.Append(new string(' ', maxLenghts - "Id".Length));
                header.Insert(fullLenghts + ((maxLenghts / 2) - ("Id".Length / 2)), "Id");
                header.Append('|');
                fullLenghts += maxLenghts + 1;
            }

            if (filds.Contains("FIRSTNAME"))
            {
                int maxLenghts = "firstname".Length;
                foreach (var item in records)
                {
                    int lenght = item.FirstName.Length;
                    if (maxLenghts < lenght)
                    {
                        maxLenghts = lenght;
                    }
                }

                maxLenghts += 2;
                offsets.Add(maxLenghts);
                line.Append(new string('-', maxLenghts));
                line.Append('+');

                header.Append(new string(' ', maxLenghts - "Firstname".Length));
                header.Insert(fullLenghts + ((maxLenghts / 2) - ("Firstname".Length / 2)), "Firstname");
                header.Append('|');
                fullLenghts += maxLenghts + 1;
            }

            if (filds.Contains("LASTNAME"))
            {
                int maxLenghts = 0;
                foreach (var item in records)
                {
                    int lenght = item.LastName.Length;
                    if (maxLenghts < lenght)
                    {
                        maxLenghts = lenght;
                    }
                }

                maxLenghts += 2;
                offsets.Add(maxLenghts);
                line.Append(new string('-', maxLenghts));
                line.Append('+');

                header.Append(new string(' ', maxLenghts - "Lastname".Length));
                header.Insert(fullLenghts + ((maxLenghts / 2) - ("Lastname".Length / 2)), "Lastname");
                header.Append('|');
                fullLenghts += maxLenghts + 1;
            }

            if (filds.Contains("DATEOFBIRTH"))
            {
                int maxLenghts = "Date of Birth".Length + 2;
                offsets.Add(maxLenghts);
                line.Append(new string('-', maxLenghts));
                line.Append('+');

                header.Append(new string(' ', maxLenghts - "Date of Birth".Length));
                header.Insert(fullLenghts + ((maxLenghts / 2) - ("Date of Birth".Length / 2)), "Date of Birth");
                header.Append('|');
                fullLenghts += maxLenghts + 1;
            }

            if (filds.Contains("CHILDREN"))
            {
                int maxLenghts = "Children".Length + 2;
                offsets.Add(maxLenghts);
                line.Append(new string('-', maxLenghts));
                line.Append('+');

                header.Append(new string(' ', maxLenghts - "Children".Length));
                header.Insert(fullLenghts + ((maxLenghts / 2) - ("Children".Length / 2)), "Children");
                header.Append('|');
                fullLenghts += maxLenghts + 1;
            }

            if (filds.Contains("SALARY"))
            {
                int maxLenghts = "salary".Length;
                foreach (var item in records)
                {
                    int lenght = item.AverageSalary.ToString(new CultureInfo("en-US")).Length;
                    if (maxLenghts < lenght)
                    {
                        maxLenghts = lenght;
                    }
                }

                maxLenghts += 2;
                offsets.Add(maxLenghts);
                line.Append(new string('-', maxLenghts));
                line.Append('+');

                header.Append(new string(' ', maxLenghts - "Salary".Length));
                header.Insert(fullLenghts + ((maxLenghts / 2) - ("Salary".Length / 2)), "Salary");
                header.Append('|');
                fullLenghts += maxLenghts + 1;
            }

            if (filds.Contains("SEX"))
            {
                int maxLenghts = "sex".Length + 2;
                offsets.Add(maxLenghts);
                line.Append(new string('-', maxLenghts));
                line.Append('+');

                header.Append(new string(' ', maxLenghts - "Sex".Length));
                header.Insert(fullLenghts + ((maxLenghts / 2) - ("Sex".Length / 2)), "Sex");
                header.Append('|');
                fullLenghts += maxLenghts + 1;
            }

            return new Tuple<StringBuilder, StringBuilder, List<int>>(line, header, offsets);
        }

        private static StringBuilder CreatePadding(List<string> filds, FileCabinetRecord record, List<int> offsets)
        {
            StringBuilder padding = new StringBuilder("|");
            int counter = 0;
            int fullLenghts = 0;
            if (filds.Contains("ID"))
            {
                string id = record.Id.ToString(new CultureInfo("en-US"));
                padding.Append(new string(' ', offsets[counter] - id.Length));
                padding.Insert(fullLenghts + offsets[counter] - id.Length, id);
                padding.Append('|');
                fullLenghts += offsets[counter] + 1;
                counter++;
            }

            if (filds.Contains("FIRSTNAME"))
            {
                padding.Append(new string(' ', offsets[counter] - record.FirstName.Length));
                padding.Insert(fullLenghts + 2, record.FirstName);
                padding.Append('|');
                fullLenghts += offsets[counter] + 1;
                counter++;
            }

            if (filds.Contains("LASTNAME"))
            {
                padding.Append(new string(' ', offsets[counter] - record.LastName.Length));
                padding.Insert(fullLenghts + 2, record.LastName);
                padding.Append('|');
                fullLenghts += offsets[counter] + 1;
                counter++;
            }

            if (filds.Contains("DATEOFBIRTH"))
            {
                string birthday = record.DateOfBirth.ToString("yyyy-MMM-dd");
                padding.Append(new string(' ', offsets[counter] - birthday.Length));
                padding.Insert(fullLenghts + 2, birthday);
                padding.Append('|');
                fullLenghts += offsets[counter] + 1;
                counter++;
            }

            if (filds.Contains("CHILDREN"))
            {
                string children = record.Children.ToString(new CultureInfo("en-US"));
                padding.Append(new string(' ', offsets[counter] - children.Length));
                padding.Insert(fullLenghts + offsets[counter] - children.Length, children);
                padding.Append('|');
                fullLenghts += offsets[counter] + 1;
                counter++;
            }

            if (filds.Contains("SALARY"))
            {
                string salary = record.AverageSalary.ToString(new CultureInfo("en-US"));
                padding.Append(new string(' ', offsets[counter] - salary.Length));
                padding.Insert(fullLenghts + offsets[counter] - salary.Length, salary);
                padding.Append('|');
                fullLenghts += offsets[counter] + 1;
                counter++;
            }

            if (filds.Contains("SEX"))
            {
                padding.Append(new string(' ', offsets[counter] - 1));
                padding.Insert(fullLenghts + 3, record.Sex);
                padding.Append('|');
            }

            return padding;
        }
    }
}
