using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ExcelCableGeneratorApp.Identifier;

/// <summary>
/// Handles generating ids based on the owner id and 24 batch size
/// </summary>
public class IdentifierGenerator
{
    private static readonly int Id_Batch_Size = 24;

    private readonly Dictionary<string, int> _idSequencesLastValue;
    private readonly Dictionary<string, List<(string, int)>> _generatedIds;
    private readonly Dictionary<string, int> _lastIdParent; // store the panel id 

    public IdentifierGenerator()
    {
        _idSequencesLastValue = [];
        _generatedIds = [];
        _lastIdParent = [];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idPrefix"></param>
    /// <returns></returns>
    public (string, int) NextId(string idPrefix, string ownerId)
    {
        var _ownerId = ExtractFirstNumber(ownerId);

        if (!_idSequencesLastValue.ContainsKey(idPrefix))
            throw new Exception("Must start sequence before use!");

        var lastId = _idSequencesLastValue[idPrefix];
        var lastOwnerId = _lastIdParent[idPrefix];
        if (_ownerId != lastOwnerId && lastId > 0)
            lastId = EndIdBatch(idPrefix);
        
        var nextIdNumber = lastId + 1;
        var nextId = $"{idPrefix}{nextIdNumber,3}";

        _generatedIds[idPrefix].Add((nextId, nextIdNumber));
        _idSequencesLastValue[idPrefix] = nextIdNumber;
        _lastIdParent[idPrefix] = _ownerId;

        return (nextId, nextIdNumber);
    }

    /// <summary>
    /// Shifts the cursor to the end of the current batch
    /// </summary>
    /// <remarks>
    /// Call before calling NextId() when batch needs to be ended.
    /// </remarks>
    /// <param name="type"></param>
    /// <exception cref="Exception"></exception>
    public int EndIdBatch(string idPrefix)
    {
        var lastId = _idSequencesLastValue[idPrefix];
        var _adjustedLastId = Id_Batch_Size - (lastId % Id_Batch_Size) + lastId;
        _idSequencesLastValue[idPrefix] = _adjustedLastId;
        return _adjustedLastId;
    }

    /// <summary>
    /// Start a new id sequence
    /// </summary>
    /// <param name="idPrefix"></param>
    /// <param name="force"></param>
    public void StartNewSequence(string idPrefix, bool force = false)
    {
        if (!_idSequencesLastValue.TryAdd(idPrefix, 0))
        {
            throw new Exception($"There was already an ID sequence with prefix: {idPrefix}");
        }
        _generatedIds.Add(idPrefix, []);
        _lastIdParent.Add(idPrefix, -1);
    }

    private static int ExtractFirstNumber(string input)
    {
        string numberStr = Regex.Match(input, @"\d+").Value;
        if (int.TryParse(numberStr, out var intValue)) {
            return intValue;
        }

        return -1;
    }
}
