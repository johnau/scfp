# Faceplate Data Extractor

## Excel Spreadsheet Data Extractor

- Certain aspects of the spreadsheet must be relied on as anchors.
  - e.g. "Panel ID" will most likely need to always be the first column of the table.  *Potentially also located on the last row of the header block.*

  - Labels of the System Type columns for **Quantity of Outlets** and **To/From** will also likely need to be kept very similar.

  - Currently, identification of column types occurs by matching a set of keywords that should be found in the column description/header.  If not all keywords are matched, the column is considered not a match.  This is done by a string[] property assosciated with the Enum values in the [SystemType Enum](xref:FaceplateDataExtractor.Model.SystemType), [CableType Enum](xref:FaceplateDataExtractor.Model.CableType), and [ColumnValueType Enum](xref:FaceplateDataExtractor.Model.Mapper.ColumnValueType).
    - It could be refactored so that this array contains different possible values to identify the column, and matching any is sufficient.  This may be a better implementation going forward - and eventually these values can be shifted to the database so they can be adjusted over time.
    - Further sanitization of the values from the data source would also be best before doing these comparisons.
