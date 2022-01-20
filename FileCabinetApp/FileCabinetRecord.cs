namespace FileCabinetApp
{
    /// <summary>
    /// Represents records.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// </summary>
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

        /// <summary>
        /// Gets or sets id of record.
        /// </summary>
        /// <value> Id of record.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets firstname in record.
        /// </summary>
        /// <value>Firstname in record.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets lastname in record.
        /// </summary>
        /// <value>Lastname in record.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets dateofbirth in record.
        /// </summary>
        /// <value>Dateofbirth in record.</value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets number of childrens in record.
        /// </summary>
        /// <value>Number of childrens in record.</value>
        public short Children { get; set; }

        /// <summary>
        /// Gets or sets average salary in record.
        /// </summary>
        /// <value>Average salary in record.</value>
        public decimal AverageSalary { get; set; }

        /// <summary>
        /// Gets or sets sex in record.
        /// </summary>
        /// <value>Sex in record.</value>
        public char Sex { get; set; }
    }
}