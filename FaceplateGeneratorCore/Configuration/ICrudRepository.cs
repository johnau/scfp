namespace FaceplateGeneratorCore.Configuration
{
    /// <summary>
    /// Represents a generic CRUD repository interface.
    /// </summary>
    /// <typeparam name="T">The type of objects stored in the repository.</typeparam>
    /// <typeparam name="ID">The type of the ID used for identifying objects in the repository.</typeparam>
    /// <remarks>
    /// <para>
    /// This interface defines basic CRUD (Create, Read, Update, Delete) operations
    /// for interacting with a data repository.
    /// </para>
    /// <para>
    /// Implementations of this interface can be used both at the repository layer
    /// and the DAO (Data Access Object) layer.
    /// </para>
    /// </remarks>
    public interface ICrudRepository<T, ID> where T : class
    {
        /// <summary>
        /// Retrieves all objects stored in the repository.
        /// </summary>
        /// <returns>A list of <typeparamref name="T"/> objects, or an empty list if no objects are found.</returns>
        List<T> FindAll();

        /// <summary>
        /// Retrieves objects with the specified IDs from the repository.
        /// </summary>
        /// <param name="ids">A list of IDs to match.</param>
        /// <returns>A list of <typeparamref name="T"/> objects matching the provided IDs.</returns>
        List<T> FindByIds(List<ID> ids);

        /// <summary>
        /// Retrieves an object by its ID.
        /// </summary>
        /// <param name="id">The ID of the object to retrieve.</param>
        /// <returns>The <typeparamref name="T"/> object found with the specified ID, or <c>null</c> if not found.</returns>
        T? FindById(ID id);

        /// <summary>
        /// Saves the provided object and returns its ID.
        /// </summary>
        /// <param name="o">The object to save.</param>
        /// <returns>The ID of the saved object.</returns>
        ID Save(T o);

        /// <summary>
        /// Deletes the object with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the object to delete.</param>
        /// <returns><c>true</c> if the object was successfully deleted; otherwise, <c>false</c>.</returns>
        bool Delete(ID id);
    }
}
