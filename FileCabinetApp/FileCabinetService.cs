﻿namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth)
        {
            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
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
    }
}
