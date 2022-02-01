namespace FileCabinetGenerator
{
    public class GeneratorParams
    {
        public GeneratorParams()
        {
            OutputType = string.Empty;
            Filename = string.Empty;
            RecordsAmount = 0;
            Id = 0;
        }
        public string OutputType { get; set; }
        public string Filename { get; set; }
        public int RecordsAmount { get; set; }
        public int Id { get; set; }
    }
}
