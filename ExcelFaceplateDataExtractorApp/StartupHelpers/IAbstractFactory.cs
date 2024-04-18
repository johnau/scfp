namespace ExcelFaceplateDataExtractorApp.StartupHelpers
{
    public interface IAbstractFactory<T>
    {
        T Create();
    }
}
