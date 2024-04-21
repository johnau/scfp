namespace ExcelCableGeneratorApp.Identifier.Aggregates;

internal class StageCraftCableId
{
    public string Prefix { get; }
    public int Number { get; }
    public int Digits { get; }
    public string IdFull => FullId();

    public StageCraftCableId(string prefix, int number, int digits = 3)
    {
        Prefix = prefix;
        Number = number;
        Digits = digits;
    }

    private string FullId()
    {
        var idString = Prefix;
        for (int i = 0; i < Digits - Number.ToString().Length; i++)
        {
            idString += "0";
        }
        idString += Number.ToString();
        return idString;
    }
}
