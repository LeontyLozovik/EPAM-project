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
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}