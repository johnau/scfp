using FaceplateIdGenerator.Aggregates;
using System.Diagnostics;

namespace FaceplateIdGenerator
{
    public class PrefixedIdentifierGenerator : IIdentifierGenerator
    {
        private readonly Dictionary<IdentifierType, Identifier> identifiers = [];
        private readonly Dictionary<IdentifierType, List<string>> generatedIds = [];
        private readonly Dictionary<IdentifierType, string> lastIdParent = []; // store the panel id assosciated with the generated id to trigger EndIdBatch()

        public List<string> EndSequence(IdentifierType type)
        {
            if (!identifiers.ContainsKey(type))
            {
                throw new Exception("No sequence exists for the specified identifier type.");
            }
            identifiers.Remove(type);
            
            var generatedIdList = generatedIds[type];
            generatedIds.Remove(type);

            return generatedIdList;
        }

        public string NextId(IdentifierType type, string idOwner)
        {
            if (type == IdentifierType.NONE)
                throw new Exception("Should not provide non type");

            if (!identifiers.TryGetValue(type, out var identifier))
                throw new Exception("There is no Id sequence started");
            
            if (!lastIdParent.TryGetValue(type, out var lastOwner))
                throw new Exception("Expected sequence does not exist");

            if (lastOwner == string.Empty)
            {
                lastIdParent[type] = idOwner;
            }
            else if (idOwner != lastOwner && lastOwner != string.Empty)
            {
                EndIdBatch(type);
                lastIdParent[type] = idOwner;
            }

            var nextId = identifier.IncrementId();
            generatedIds[type].Add(nextId);

            return nextId;
        }

        /// <summary>
        /// Shifts the cursor to the end of the current batch
        /// </summary>
        /// <remarks>
        /// Call before calling NextId() when batch needs to be ended.
        /// </remarks>
        /// <param name="type"></param>
        /// <exception cref="Exception"></exception>
        public void EndIdBatch(IdentifierType type)
        {
            if (!identifiers.TryGetValue(type, out var identifier))
            {
                throw new Exception("There is no Id sequence started");
            }

            identifier.EndBatch();
        }

        public void StartAllSequences()
        {
            foreach (IdentifierType type in Enum.GetValues(typeof(IdentifierType)))
            {
                if (type == IdentifierType.NONE) continue;

                StartNewSequence(type);
            }
        }

        public void StartNewSequence(IdentifierType type, bool force = false)
        {
            if (type == IdentifierType.NONE)
            {
                Debug.WriteLine($"!! NONE Type System Identifier received, no action in Id Generator");
                return;
            }

            // Dictionary to map IdentifierType enum values to identifier types
            Dictionary<IdentifierType, Func<Identifier>> identifierCreators = new()
            {
                { IdentifierType.AUDIO, () => new AudioIdentifier() },
                { IdentifierType.AUDIO_VISUAL, () => new AudioVisualIdentifier() },
                { IdentifierType.AV_CONTROL, () => new AvControlIdentifier() },
                { IdentifierType.DANTE_ETHERNET_AUDIO, () => new DanteEthernetAudioIdentifier() },
                { IdentifierType.DIGITAL_MEDIA, () => new DigitalMediaIdentifier() },
                { IdentifierType.DMX_LIGHTING_CONTROL, () => new DmxLightingControlIdentifier() },
                { IdentifierType.ESTOP, () => new EstopIdentifier() },
                { IdentifierType.HOIST_CONTROL, () => new HoistControlIdentifier() },
                { IdentifierType.HOUSE_CURTAIN_CONTROL, () => new HouseCurtainControlIdentifier() },
                { IdentifierType.MULTIMODE_FIBER, () => new MultimodeFiberIdentifier() },
                { IdentifierType.PAGING_SPEAKER, () => new PagingSpeakerIdentifier() },
                { IdentifierType.PAGING_STATION, () => new PagingStationIdentifier() },
                { IdentifierType.PERFORMANCE_LOUDSPEAKER, () => new PerformanceLoudSpeakerIdentifier() },
                { IdentifierType.STAGE_LIGHTING_OUTLET, () => new StageLightingOutletsIdentifier() },
                { IdentifierType.TALKBACK, () => new TalkbackIdentifier() },
                { IdentifierType.TECH_DATA, () => new TechDataIdentifier() },
                { IdentifierType.VIDEO_TIE_LINE, () => new VideoTieLineIdentifier() }
            };

            if (force)
                throw new NotImplementedException("");

            // Check if identifier type already exists
            if (!identifiers.TryAdd(type, identifierCreators[type].Invoke()))
            {
                throw new Exception("There was already an Id sequence in use");
            }

            // New empty list
            generatedIds[type] = [];
            lastIdParent[type] = "";
        }
    }
}
