using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handle export command.
    /// </summary>
    public class ExportCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handle export command.
        /// </summary>
        /// <param name="request">request with command and param.</param>
        public override void Handle(AppCommandRequest request)
        {
            if (string.Equals(request.Command, "export", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(request.Parameters))
                {
                    Console.WriteLine("Command 'export' should contain type and path of file to export.");
                }
                else
                {
                    string[] input = request.Parameters.Split(' ', 2);
                    if (input.Length != 2)
                    {
                        Console.WriteLine("Please check you input.");
                    }
                    else
                    {
                        string typeOfFile = input[0].ToUpperInvariant();
                        string filePath = input[1];
                        switch (typeOfFile)
                        {
                            case "CSV":
                                if (!filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                                {
                                    filePath = string.Concat(filePath, ".csv");
                                }

                                if (File.Exists(filePath))
                                {
                                    bool notEnd = true;
                                    do
                                    {
                                        Console.Write($"File is exist - rewrite {filePath} [Y/n]");
                                        var answer = Console.ReadLine();
                                        if (string.IsNullOrEmpty(answer))
                                        {
                                            break;
                                        }

                                        if (string.Equals(answer, "y", StringComparison.OrdinalIgnoreCase))
                                        {
                                            File.Delete(filePath);
                                            notEnd = false;
                                        }
                                        else if (string.Equals(answer, "n", StringComparison.OrdinalIgnoreCase))
                                        {
                                            break;
                                        }
                                    }
                                    while (notEnd);
                                }

                                try
                                {
                                    FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
                                    StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
                                    var snapshot = Program.fileCabinetService.MakeSnapshot();
                                    snapshot.SaveToCsv(streamWriter);
                                    Console.WriteLine($"All records are exported to file {filePath}");
                                    streamWriter.Close();
                                    fileStream.Close();
                                }
                                catch (FileNotFoundException)
                                {
                                    Console.WriteLine($"Export failed: can't open file {filePath}");
                                }

                                break;
                            case "XML":
                                if (!filePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                                {
                                    filePath = string.Concat(filePath, ".xml");
                                }

                                if (File.Exists(filePath))
                                {
                                    bool notEnd = true;
                                    do
                                    {
                                        Console.Write($"File is exist - rewrite {filePath} [Y/n]");
                                        var answer = Console.ReadLine();
                                        if (string.IsNullOrEmpty(answer))
                                        {
                                            break;
                                        }

                                        if (string.Equals(answer, "y", StringComparison.OrdinalIgnoreCase))
                                        {
                                            File.Delete(filePath);
                                            notEnd = false;
                                        }
                                        else if (string.Equals(answer, "n", StringComparison.OrdinalIgnoreCase))
                                        {
                                            break;
                                        }
                                    }
                                    while (notEnd);
                                }

                                try
                                {
                                    FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
                                    StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
                                    var snapshot = Program.fileCabinetService.MakeSnapshot();
                                    snapshot.SaveToXml(streamWriter);
                                    Console.WriteLine($"All records are exported to file {filePath}");
                                    streamWriter.Close();
                                    fileStream.Close();
                                }
                                catch (FileNotFoundException)
                                {
                                    Console.WriteLine($"Export failed: can't open file {filePath}");
                                }

                                break;
                            default:
                                Console.WriteLine($"Unknown or unsupported type of file - {typeOfFile}");
                                break;
                        }
                    }
                }
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(request);
            }
        }
    }
}
