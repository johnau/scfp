namespace FaceplateDataExtractor.Data
{
    public class StringFileReader
    {
        private readonly string _filePath;

        public StringFileReader(string filePath)
        {
            _filePath = filePath;
        }

        public List<string> ReadData()
        {
            try
            {
                using var reader = new BinaryReader(File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read));
                List<string> strings = [];
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    strings.Add(reader.ReadString());
                }

                return strings;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                throw;
            }
        }
    }

    //public class StringJsonReader
    //{
    //    private readonly string _filePath;

    //    public StringJsonReader(string filePath)
    //    {
    //        _filePath = filePath;
    //    }

    //    public List<string> ReadData()
    //    {
    //        try
    //        {
    //            string jsonData = File.ReadAllText(_filePath);
    //            List<string> strings = JsonConvert.DeserializeObject<List<string>>(jsonData);
    //            return strings;
    //        }
    //        catch (IOException ex)
    //        {
    //            Console.WriteLine($"Error reading from file: {ex.Message}");
    //            throw;
    //        }
    //    }
    //}
}
