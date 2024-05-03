namespace ExcelCableGeneratorApp.App.DataIngest;

internal abstract class ProcessStep<TIn,TOut>
{
    public string Name { get; init; }
    public TIn Data { get; private set; }
    private TOut _processedData;

    public ProcessStep(string name)
    {
        Name = name;
    }

    public abstract void SetData(TIn data);

    public abstract TOut Process();

}
