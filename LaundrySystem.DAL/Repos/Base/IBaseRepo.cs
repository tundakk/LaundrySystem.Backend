namespace LaundrySystem.DAL.Repos.Base
{
    /// <summary>
    /// The interface for the base repository class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseRepo<T> where T : class
    {
        /// <summary>
        ///  Get all entries in the repo.
        /// </summary>
        /// <returns>Returns all instances of a given entity.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Add and entity to repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns object that was created.</returns>
        Task<T> InsertAsync(T entity);

        /// <summary>
        /// Updates an exising entity in the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns an updated object of the entity.</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns an object that was removed.</returns>
        Task<T> DeleteAsync(T entity);

        /// <summary>
        /// saves changes to repository.
        /// </summary>
        Task SaveAsync();

        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <returns>Returns the entity that matches the ID.</returns>
        Task<T> GetByIdAsync(Guid id);
    }
}