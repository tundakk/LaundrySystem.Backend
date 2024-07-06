namespace LaundrySystem.DAL.Repos.Base
{
    using LaundrySystem.DAL.DataModel;
    using System.Linq;

    public abstract class BaseRepo<T> : IBaseRepo<T> where T : class, new()
    {
        /// <summary>
        /// Protected readonly property used as DI for classes. Doesnt feel right to have it public.
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
        public virtual T GetById(int id)
        {
            var entity = dataContext.Set<T>().Find(id);
            if (entity == null)
            {
                throw new ArgumentException($"Entity of type {typeof(T).Name} with ID {id} was not found.");
            }
            return entity;
        }

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>Returns all instances of a given entity.</returns>
        public virtual IQueryable<T> GetAll()
        {
            return dataContext.Set<T>();
        }

        /// <summary>
        /// Adds an entity to the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns the object that was created.</returns>
        public virtual T Insert(T entity)
        {
            var addedEntity = dataContext.Set<T>().Add(entity).Entity;
            Save();
            return addedEntity;
        }

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns the updated object of the entity.</returns>
        public virtual T Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Update - Entity must not be null");
            }

            dataContext.SetModified(entity);
            Save();
            return entity;
        }

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Returns the object that was removed.</returns>
        public virtual T Delete(T entity)
        {
            var removedEntity = dataContext.Set<T>().Remove(entity).Entity;
            Save();
            return removedEntity;
        }

        /// <summary>
        /// Saves changes to the repository.
        /// </summary>
        public void Save()
        {
            try
            {
                this.dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                var newEx = new Exception($"DAL Save - Could not be completed: {ex.Message}.", ex);
                throw newEx;
            }
        }
    }
}