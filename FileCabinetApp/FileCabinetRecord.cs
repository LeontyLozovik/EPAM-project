namespace FileCabinetApp
{
  public class FileCabinetRecord
    {
        // A constructor is needed to avoid CS 8618 warning in "FirstName" and "LastName" properties
        public FileCabinetRecord()
        {
            this.Id = 0;
            this.FirstName = string.Empty;
            this.LastName = string.Empty;
            this.DateOfBirth = default(DateTime);
            this.Children = 0;
            this.AverageSalary = 0;
            this.Sex = ' ';
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public short Children { get; set; }

        public decimal AverageSalary { get; set; }

        public char Sex { get; set; }
    }
}