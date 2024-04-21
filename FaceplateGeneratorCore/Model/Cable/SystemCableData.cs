namespace FaceplateGeneratorCore.Model.Cable;

public class SystemCableData
{

    public string SystemType { get; }
    public List<CableData> CablesInSystem { get; }

    public SystemCableData(string systemType, List<CableData> cablesInSystem)
    {
        SystemType = systemType;
        CablesInSystem = cablesInSystem;
    }
}
