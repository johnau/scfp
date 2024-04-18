namespace FaceplateDataExtractor.Data
{
    public class FileWriter
    {
        private readonly string _filePath;

        public FileWriter(string filePath)
        {
            _filePath = filePath;
        }

        public bool WriteData(List<string> strings)
        {
            try
            {
                using var writer = new BinaryWriter(File.Open(_filePath, FileMode.OpenOrCreate));
                writer.Write(strings.Count);
                foreach (string str in strings)
                {
                    writer.Write(str);
                }
                return true;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error writing file: {ex.Message}");
                return false;
            }
        }
    }

    //public class StringJsonWriter
    //{
    //    private readonly string _filePath;

    //    public StringJsonWriter(string filePath)
    //    {
    //        _filePath = filePath;
    //    }

    //    public bool WriteData(List<string> strings)
    //    {
    //        try
    //        {
    //            string jsonData = JsonConvert.SerializeObject(strings);
    //            File.WriteAllText(_filePath, jsonData);
    //            return true;
    //        }
    //        catch (IOException ex)
    //        {
    //            Console.WriteLine($"Error writing to file: {ex.Message}");
    //            return false;
    //        }
    //    }
    //}

}
