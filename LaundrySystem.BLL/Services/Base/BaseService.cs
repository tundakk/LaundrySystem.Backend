namespace LaundrySystem.BLL.Infrastructure.Services
{
    using LaundrySystem.BLL.Infrastructure.Interfaces;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.Domain.Model.Responses;
    using Mapster;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Base service class providing common CRUD operations.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TRepo">The type of the repository.</typeparam>
    public abstract class BaseService<TModel, TEntity, TRepo> : IBaseService<TModel>
        where TModel : class
        where TEntity : class
        where TRepo : IBaseRepo<TEntity>
    {
        /// <summary>
        /// The repository instance.
        /// </summary>
        protected readonly TRepo Repository;

        /// <summary>
        /// The logger instance.
        /// </summary>
        protected readonly ILogger<BaseService<TModel, TEntity, TRepo>> Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{TModel, TEntity, TRepo}"/> class.
        /// </summary>
        /// <param name="repository">The repository instance.</param>
        /// <param name="logger">The logger instance.</param>
        protected BaseService(TRepo repository, ILogger<BaseService<TModel, TEntity, TRepo>> logger)
        {
            Repository = repository;
            Logger = logger;
        }

        /// <inheritdoc />
        public virtual async Task<ServiceResponse<IEnumerable<TModel>>> GetAllAsync()
        {
            try
            {
                var entities = await Repository.GetAllAsync();
                var models = entities.Adapt<IEnumerable<TModel>>();
                return new ServiceResponse<IEnumerable<TModel>>
                {
                    Data = models,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting all entities");
                return new ServiceResponse<IEnumerable<TModel>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <inheritdoc />
        public virtual async Task<ServiceResponse<TModel>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await Repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResponse<TModel>
                    {
                        Success = false,
                        Message = "Entity not found"
                    };
                }

                var model = entity.Adapt<TModel>();
                return new ServiceResponse<TModel>
                {
                    Data = model,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting entity by id");
                return new ServiceResponse<TModel>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <inheritdoc />
        public virtual async Task<ServiceResponse<TModel>> InsertAsync(TModel model)
        {
            try
            {
                var entity = model.Adapt<TEntity>();
                var insertedEntity = await Repository.InsertAsync(entity);
                var insertedModel = insertedEntity.Adapt<TModel>();
                return new ServiceResponse<TModel>
                {
                    Data = insertedModel,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error inserting entity");
                return new ServiceResponse<TModel>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <inheritdoc />
        public virtual async Task<ServiceResponse<TModel>> UpdateAsync(TModel model)
        {
            try
            {
                var entity = model.Adapt<TEntity>();
                var updatedEntity = await Repository.UpdateAsync(entity);
                var updatedModel = updatedEntity.Adapt<TModel>();
                return new ServiceResponse<TModel>
                {
                    Data = updatedModel,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating entity");
                return new ServiceResponse<TModel>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        /// <inheritdoc />
        public virtual async Task<ServiceResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await Repository.GetByIdAsync(id);
                if (entity == null)
                {
                    Logger.LogWarning("Entity with ID {Id} not found.", id);
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Entity not found",
                        Data = false
                    };
                }

                await Repository.DeleteAsync(entity); // Perform the delete
                return new ServiceResponse<bool>
                {
                    Success = true,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting entity with ID {Id}", id);
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                };
            }
        }
    }
}