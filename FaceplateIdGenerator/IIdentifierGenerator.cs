using FaceplateIdGenerator.Aggregates;

namespace FaceplateIdGenerator;

public interface IIdentifierGenerator
{
    /// <summary>
    /// Start a new sequence
    /// </summary>
    void StartNewSequence(IdentifierType type, bool force = false);
    /// <summary>
    /// Returns the next Id in the sequence
    /// </summary>
    /// <returns></returns>
    string NextId(IdentifierType type, string idOwner);

    void EndIdBatch(IdentifierType type);

    /// <summary>
    /// Returns all ids generated;
    /// </summary>
    /// <returns></returns>
    List<string> EndSequence(IdentifierType type);

    void StartAllSequences();

}
