namespace FileCabinetApp
{
    /// <summary>
    /// Service class for crating snapshots.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">records to initialize new istance of this class.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Save all records to csv file.
        /// </summary>
        /// <param name="streamWriter">Stream Writer to write records to file.</param>
        public void SaveToCsv(StreamWriter streamWriter)
        {
            FileCabinetRecordCsvWriter fileWriter = new FileCabinetRecordCsvWriter(streamWriter);
            fileWriter.WriteTemplate();
            foreach (var record in this.records)
            {
                fileWriter.Write(record);
            }
        }
    }
}
