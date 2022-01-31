using FileCabinetApp;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    public static class Program
    {
        private static GeneratorParams generator = new GeneratorParams();
        private static List<FileCabinetRecord> generatedRecords = new List<FileCabinetRecord>();
        public static void Main(string[] args)
        {
            ProcessInputParams(args);
            for (int i = 0; i < generator.RecordsAmount; i++)
            {
                FileCabinetRecord record = GenerateRecord(generator);
                generatedRecords.Add(record);
                generator.Id += 1;
            }
            Export(generatedRecords);
            Console.WriteLine($"{generator.RecordsAmount} records were written to {generator.Filename}.");
        }

        private static void Export(List<FileCabinetRecord> records)
        {
            string typeOfFile = generator.OutputType;
            string filePath = generator.Filename;
            switch (typeOfFile)
            {
                case "csv":
                    if (!filePath.EndsWith(".csv"))
                    {
                        filePath = string.Concat(filePath, ".csv");
                    }

                    try
                    {
                        FileStream fileStream;
                        if (File.Exists(filePath))
                        {
                            fileStream = new FileStream(filePath, FileMode.Truncate);
                        }
                        else
                        {
                            fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
                        }
                        using (StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.Default))
                        {
                            SaveToCsv(streamWriter, records);
                        }
                        fileStream.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    break;
                case "xml":
                    if (!filePath.EndsWith(".xml"))
                    {
                        filePath = string.Concat(filePath, ".xml");
                    }

                    try
                    {
                        FileStream fileStream;
                        if (File.Exists(filePath))
                        {
                            fileStream = new FileStream(filePath, FileMode.Truncate);
                        }
                        else
                        {
                            fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
                        }
                        FileStreamOptions fileStreamOptions = new FileStreamOptions();
                        using (StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.Default))
                        {
                            SaveToXml(streamWriter, generatedRecords);
                        }                       
                        fileStream.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    break;
                default:
                    Console.WriteLine($"Unknown or unsupported type of file - {typeOfFile}");
                    break;
            }
        }
        private static void SaveToCsv(StreamWriter streamWriter, List<FileCabinetRecord> records)
        {
            TextWriter textWriter = streamWriter;
            foreach (var record in records)
            {
                StringBuilder stringToWrite = new StringBuilder();
                object[] fildsOfRecord =
                {
                    record.Id, record.FirstName, record.LastName, record.DateOfBirth.ToString("dd/MM/yyyy"),
                    record.Children, record.AverageSalary, record.Sex,
                };
                stringToWrite.AppendJoin(',', fildsOfRecord);
                textWriter.WriteLine(stringToWrite);
            }
            textWriter.Close();
        }
        private static void SaveToXml(StreamWriter streamWriter, List<FileCabinetRecord> records)
        {
            TextWriter writer = streamWriter;
            XmlSerializer serializer = new XmlSerializer(typeof(List<FileCabinetRecord>));
            serializer.Serialize(writer, records);
        }

        private static void ProcessInputParams(string[] args)
        {
            if (args.Length != 4 && args.Length != 8)
            {
                Console.WriteLine("Please check your input");
            }
            else
            {
                switch (args.Length)
                {
                    case 4:
                        string[][] arguments = {
                            args[0].Split("=", 2),
                            args[1].Split("=", 2),
                            args[2].Split("=", 2),
                            args[3].Split("=", 2),
                        };
                        if (CheckArguments(arguments))
                        {
                            generator = CreateGenerator(arguments);
                        }
                        break;
                    case 8:
                        arguments = new string[4][];
                        arguments[0] = new string[] { args[0], args[1] };
                        arguments[1] = new string[] { args[2], args[3] };
                        arguments[2] = new string[] { args[4], args[5] };
                        arguments[3] = new string[] { args[6], args[7] };
                        if (CheckArguments(arguments))
                        {
                            generator = CreateGenerator(arguments);
                        }
                        break;
                }
            }
        }
        private static bool CheckArguments(string[][] arguments)
        {
            if (!string.Equals(arguments[0][0], "--output-type") && !string.Equals(arguments[0][0], "-t"))
            {
                return false;
            }
            if (!string.Equals(arguments[1][0], "--output") && !string.Equals(arguments[1][0], "-o"))
            {
                return false;
            }
            if (!string.Equals(arguments[2][0], "--records-amount") && !string.Equals(arguments[2][0], "-a"))
            {
                return false;
            }
            if (!string.Equals(arguments[3][0], "--start-id") && !string.Equals(arguments[3][0], "-i"))
            {
                return false;
            }

            return true;
        }
        private static GeneratorParams CreateGenerator(string[][] arguments)
        {
            string outputType = arguments[0][1].ToLowerInvariant(); 
            if (!string.Equals(outputType.ToLowerInvariant(), "csv") && !string.Equals(outputType.ToLowerInvariant(), "xml"))
            {
                throw new ArgumentException("Invalid output-type");
            }
            string filename = arguments[1][1];
            int amount;
            if (!int.TryParse(arguments[2][1], out amount))
            {
                throw new ArgumentException("Invalid records-amount");
            }
            int startId;
            if (!int.TryParse(arguments[3][1], out startId))
            {
                throw new ArgumentException("Invalid start-id");
            }
            GeneratorParams parameters = new GeneratorParams
            {
                OutputType = outputType,
                Filename = filename,
                RecordsAmount = amount,
                Id = startId,

            };

            return parameters;
        }
        private static FileCabinetRecord GenerateRecord(GeneratorParams generator)
        {
            Random random = new Random();
            bool isWomen = false;
            int position = random.Next(0, 49);
            if (position < 25)
            {
                isWomen = true;
            }
            string firstname = GenerateFirstname(position);
            position = random.Next(0, 24);
            string lastname = GenerateLastname(position);
            if (isWomen)
            {
                lastname = string.Concat(lastname, "a");
            }
            int year = random.Next(1950, DateTime.Now.Year);
            int month = random.Next(1,12);
            int day = GenerateDay(year, month);
            DateTime dateOfBirth = new DateTime(year, month, day);
            short children = (short)random.Next(0, 10);
            decimal salary = random.Next(0, 10000);
            char sex = isWomen ? 'w'  : 'm';

            FileCabinetRecord record = new FileCabinetRecord {
                Id = generator.Id,
                FirstName = firstname,
                LastName = lastname,
                DateOfBirth = dateOfBirth,
                Children = children,
                AverageSalary = salary,
                Sex = sex,
            };
            return record;
        }
        private static string GenerateFirstname(int position)
        {
            string[] names = { "Anastasia", "Anna", "Maria", "Elena", "Daria", "Alina", "Irina", "Ekaterina",
                "Arina", "Pauline", "Olga", "Julia", "Tatyana", "Natalia", "Victoria", "Elizabeth", "Kseniya",
                "Milana", "Veronica", "Alice", "Valeria", "Alexandra", "Ulyana", "Sofia", "Marina", "Alexander",
                "Dmitriy", "Maksim", "Sergei", "Andrey", "Alexei", "Artem", "Ilya",
                "Kirill", "Michael", "Nikita", "Matvey", "Arseniy", "Egor", "Ivan", "Denis", "Evgeniy",
                "Daniel", "Timothy", "Igor", "Vladimir", "Ruslan", "Mark", "Konstantin", "Oleg" };
            return names[position]; 
        }
        private static string GenerateLastname(int position)
        {
            string[] names = { "Ivanov", "Vasiliev", "Petrov", "Smirnov", "Mihailov", "Fedorov", "Sokolov", 
                "Yakovlev", "Popov", "Andreev", "Alekseev", "Aleksandrov", "Lebedev", "Grigoriev", "Stepanov", 
                "Semenov", "Pavlov", "Bogdanov", "Nikolaev", "Dmitriev", "Egorov", "Volkov", "Kuznicov", "Nikitin", 
                "Soloviev" };
            return names[position];
        }
        private static int GenerateDay(int year, int month)
        {
            Random random = new Random();
            if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
            {
                return random.Next(1, 31);
            }
            else if (month == 4 || month == 6 || month == 9 || month == 11)
            {
                return random.Next(1, 30);
            }
            else
            {
                if (year % 400 == 0)
                {
                    return random.Next(1, 29);
                }
                else if (year % 4 == 0)
                {
                    return random.Next(1, 29);
                }
                else
                {
                    return random.Next(1, 28);
                }
            }
        }
    }
}
