namespace LaundrySystem.DAL.Repos.Base
{
    using System.Linq;

    /// <summary>
    /// Theinterface for the base repository class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseRepo<T> where T : class
    {
        /// <summary>
        ///  Get all entries in the repo.
        /// </summary>
        /// <returns>Returns all instances of a given entity.</returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Add and entity to repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns object that was created.</returns>
        T Insert(T entity);

        /// <summary>
        /// Updates an exising entity in the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns an updated object of the entity.</returns>
        T Update(T entity);

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns an object that was removed.</returns>
        T Delete(T entity);

        /// <summary>
        /// saves changes to repository.
        /// </summary>
        void Save();

        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <returns>Returns the entity that matches the ID.</returns>
        T GetById(int id);

        //Explicit Load support

        //void Include(T entity, params Expression<Func<T, object>>[] joinedEntities);
        //void IncludeCollection(T entity, params Expression<Func<T, ICollection<object>>>[] joinedEntities);
        //IQueryable<T2> GetIncludeQuery<T2>(T entity, Expression<Func<T, T2>> join) where T2 : class;
        //IQueryable<T2> GetIncludeCollectionQuery<T2>(T entity, Expression<Func<T, ICollection<T2>>> join) where T2 : class;
    }
}