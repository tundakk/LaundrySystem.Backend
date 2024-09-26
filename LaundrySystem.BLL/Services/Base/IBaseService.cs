namespace LaundrySystem.BLL.Infrastructure.Interfaces
{
    using LaundrySystem.Domain.Model.Responses;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for base service providing common CRUD operations.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public interface IBaseService<TModel>
    {
        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>A service response containing a list of all models.</returns>
        Task<ServiceResponse<IEnumerable<TModel>>> GetAllAsync();

        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        /// <returns>A service response containing the model.</returns>
        Task<ServiceResponse<TModel>> GetByIdAsync(Guid id);

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="model">The model to insert.</param>
        /// <returns>A service response containing the inserted model.</returns>
        Task<ServiceResponse<TModel>> InsertAsync(TModel model);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="model">The model to update.</param>
        /// <returns>A service response containing the updated model.</returns>
        Task<ServiceResponse<TModel>> UpdateAsync(TModel model);

        /// <summary>
        /// Deletes an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to delete.</param>
        /// <returns>A service response indicating the success or failure of the operation.</returns>
        Task<ServiceResponse<bool>> DeleteAsync(Guid id);
    }
}