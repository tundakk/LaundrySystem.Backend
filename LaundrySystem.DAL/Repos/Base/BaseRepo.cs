using LaundrySystem.DAL.DataModel;
using Microsoft.EntityFrameworkCore;

namespace LaundrySystem.DAL.Repos.Base
{
    public abstract class BaseRepo<T> : IBaseRepo<T> where T : class, new()
    {
        /// <summary>
        /// Protected readonly property used as DI for classes.
        /// </summary>
        protected readonly DataContext dataContext;

        /// <summary>
        /// Default constructor for the BaseRepo class.
        /// </summary>
        /// <param name="dataContext"></param>
        protected BaseRepo(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <returns>Returns the entity that matches the ID.</returns>
        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            // Use 'await' with the asynchronous method 'FindAsync'
            var entity = await dataContext.Set<T>().FindAsync(id.ToString());
            if (entity == null)
            {
                throw new ArgumentException($"Entity of type {typeof(T).Name} with ID {id} was not found.");
            }
            return entity; // Now, 'entity' is returned correctly in an async method
        }

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>Returns all instances of a given entity.</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            // Use 'await' with 'ToListAsync' to get the result asynchronously
            return await dataContext.Set<T>().ToListAsync();
        }

        /// <summary>
        /// Adds an entity to the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns the object that was created.</returns>
        public virtual async Task<T> InsertAsync(T entity)
        {
            // Use 'await' with 'AddAsync'
            var addedEntity = (await dataContext.Set<T>().AddAsync(entity)).Entity;
            await SaveAsync(); // Call the asynchronous 'SaveAsync' method
            return addedEntity;
        }

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns the updated object of the entity.</returns>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Update - Entity must not be null");
            }

            // Use 'Update' (no async version available)
            dataContext.Set<T>().Update(entity);
            await SaveAsync();
            return entity;
        }

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns the object that was removed.</returns>
        public virtual async Task<T> DeleteAsync(T entity)
        {
            var removedEntity = dataContext.Set<T>().Remove(entity).Entity;
            await SaveAsync();
            return removedEntity;
        }

        /// <summary>
        /// Saves changes to the repository.
        /// </summary>
        public async Task SaveAsync()
        {
            try
            {
                await dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var newEx = new Exception($"DAL SaveAsync - Could not be completed: {ex.Message}.", ex);
                throw newEx;
            }
        }
    }
}