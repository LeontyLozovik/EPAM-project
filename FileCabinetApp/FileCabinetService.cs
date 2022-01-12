using System.Linq;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short children, decimal salary, char sex)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(firstName, "First name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("Incorrect first name! First name should be grater then 2, less then 60 and can't be white space.", firstName);
            }

            if (lastName is null)
            {
                throw new ArgumentNullException(lastName, "Last name can't be null.");
            }
            else if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("Incorrect last name! Last name should be grater then 2, less then 60 and can't be white space.", lastName);
            }

            if (children < 0)
            {
                throw new ArgumentException("Number of children can't be less then 0.");
            }

            if (salary < 0 || salary > 1000000000)
            {
                throw new ArgumentException("Average salary can't be less then 0 or grater then 1 billion.");
            }

            if (sex != 'm' && sex != 'w')
            {
                throw new ArgumentException("Sorry, but your sex can be m - men or w - women only.");
            }

            DateTime oldest = new DateTime(1950, 1, 1);
            DateTime now = DateTime.Now;
            if (dateOfBirth < oldest || dateOfBirth > now)
            {
                throw new ArgumentException("Sorry but minimal date of birth - 01-Jan-1950 and maxsimum - current date");
            }

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Children = children,
                AverageSalary = salary,
                Sex = sex,
            };

            this.list.Add(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] results = Array.Empty<FileCabinetRecord>();
            foreach (var record in this.list)
            {
                Array.Resize<FileCabinetRecord>(ref results, results.Length + 1);
                results[results.Length - 1] = record;
            }

            return results;
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short children, decimal salary, char sex)
        {
            if (id > this.list.Count)
            {
                throw new ArgumentException("There is no record whith this Id.");
            }

            var newRecord = new FileCabinetRecord
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Children = children,
                AverageSalary = salary,
                Sex = sex,
            };

            this.list.RemoveAt(id - 1);
            this.list.Insert(id - 1, newRecord);
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            string nameToFind = firstName.ToLowerInvariant();
            var selectedRecords = this.list.FindAll(record => string.Equals(record.FirstName.ToLower(), nameToFind, StringComparison.Ordinal));
            FileCabinetRecord[] result = selectedRecords.ToArray();
            return result;
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            string nameToFind = lastName.ToLowerInvariant();
            var selectedRecords = this.list.FindAll(record => string.Equals(record.LastName.ToLower(), nameToFind, StringComparison.Ordinal));
            FileCabinetRecord[] result = selectedRecords.ToArray();
            return result;
        }
    }
}
