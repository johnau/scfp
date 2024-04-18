using static FaceplateDataExtractor.Utility.EnumHelper;

namespace FaceplateDataExtractor.Model.Mapper
{
    /// <summary>
    /// Aids in mapping column types to the <see cref="ExtractedFaceplateData"/> object
    /// </summary>
    /// <remarks>
    /// Should really only be used by the <see cref="ExtractedFaceplateDataMapper"/> class.
    /// Note: *See the FaceplateDataExtractor.Utility.StringsHelper.Sanitize(string) method to 
    /// help write these match strings.*
    /// </remarks>
    public enum ColumnValueType
    {
        /// <summary>
        /// No type
        /// </summary>
        /// <remarks>
        /// The StringArrayValue provided for NONE should ensure it does not match any values that 
        /// might be in the spreadsheet headers.
        /// NONE must be located at the top to be assigned value of 0 to be used as default
        /// </remarks>
        [StringArrayValue(["<-None->"])] NONE, 
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["To/From"])] TO_FROM,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Send Quantity"])] QUANTITY_MALE,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Return Quantity"])] QUANTITY_FEMALE,
        /// <summary>
        /// 
        /// </summary>
        [StringArrayValue(["Quantity of Outlets"])] QUANTITY,
    }
}
