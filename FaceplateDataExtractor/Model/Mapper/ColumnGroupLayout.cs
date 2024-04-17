namespace FaceplateDataExtractor.Model.Mapper
{
    /// <summary>
    /// Represents a group of columns with related data.
    /// </summary>
    /// <remarks>
    /// Typically this will be a pair of columns, and typically it will be Quantity + Destination
    /// We probably want to support columns the other way round (ie. Destination + Quantity)
    /// And we have to handle columns with groups of 3 for the Audio Column.
    /// </remarks>
    internal class ColumnGroupLayout
    {
        #region static factory methods
        public static ColumnGroupLayout System_TwoColumnLayout() => new ColumnGroupLayout([ColumnValueType.QUANTITY_GENDERLESS, ColumnValueType.DESTINATION]);
        public static ColumnGroupLayout System_ThreeColumnLayout() => new ColumnGroupLayout([ColumnValueType.QUANTITY_MALE, ColumnValueType.QUANTITY_FEMALE, ColumnValueType.DESTINATION]);
        public static List<ColumnGroupLayout> ColumnLayouts() => [System_TwoColumnLayout(), System_ThreeColumnLayout()];
        #endregion

        private List<ColumnValueType> _columnTypes;
        private Dictionary<int, ColumnValueType> _columnIndexes;
        private Dictionary<ColumnValueType, string> _columnValues;

        public int StartIndex { get; set; }
        public int EndIndex => StartIndex + ColumnCount - 1;

        public List<ColumnValueType> ColumnTypes => new List<ColumnValueType>(_columnTypes);
        public Dictionary<int, ColumnValueType> ColumnIndexes => new Dictionary<int, ColumnValueType>(_columnIndexes);
        public Dictionary<ColumnValueType, string> ColumnValues => new Dictionary<ColumnValueType, string>(_columnValues);

        public int ColumnCount => _columnIndexes.Count;

        public ColumnGroupLayout()
        {
            StartIndex = -1;
            _columnTypes = [];
            _columnIndexes = [];
            _columnValues = [];
        }

        public ColumnGroupLayout(List<ColumnValueType> columnTypes)
            : this()
        {
            _columnTypes = columnTypes;
        }

        public void SetIndexForColumnValueType(ColumnValueType type, int colIndex)
        {
            _columnIndexes[colIndex] = type;
        }

        public bool Contains(ColumnValueType type)
        {
            return _columnTypes.Contains(type);
        }
    }
}
