namespace FileCabinetGenerator
{
    public static class Program
    {
        private static GeneratorParams generator = new GeneratorParams();
        public static void Main(string[] args)
        {
            ProcessInputParams(args);
            Console.WriteLine($"{generator.RecordsAmount} records were written to {generator.Filename}.");
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
            string outputType = arguments[0][1]; 
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
                StartId = startId,

            };

            return parameters;
        }
    }
}
