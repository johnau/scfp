namespace ExcelCableGeneratorApp.Persistence.Entity;

internal class Job
{
    public int JobId { get; set; }

    public string Name { get; set; }
    public List<Cable> Cables { get; set; }

    public Job()
    {
        Name = "";
        Cables = [];
    }
}
