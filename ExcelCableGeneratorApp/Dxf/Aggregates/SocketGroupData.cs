namespace ExcelCableGeneratorApp.Dxf.Aggregates;

/// <summary>
/// 
/// </summary>
/// <param name="SourceId">Used for label over socket group bracket</param>
/// <param name="SystemType">The system type for this socket group</param>
/// <param name="CableType">The cable type for this socket group</param>
internal record SocketGroupData(
    string SourceId,
    string SystemType,
    string CableType,
    List<SocketData> Sockets)
{ }
