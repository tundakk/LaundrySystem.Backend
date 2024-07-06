namespace LaundrySystem.BLL.Infrastructure.Services
{
    using LaundrySystem.BLL.Infrastructure.Interfaces;
    using LaundrySystem.DAL.Repos.Base;
    using LaundrySystem.Domain.Model.Responses;
    using Mapster;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class BaseService<TModel, TEntity, TRepo> : IBaseService<TModel>
        where TModel : class
        where TEntity : class
        where TRepo : IBaseRepo<TEntity>
    {
        protected readonly TRepo Repository;
        protected readonly ILogger<BaseService<TModel, TEntity, TRepo>> Logger;

        protected BaseService(TRepo repository, ILogger<BaseService<TModel, TEntity, TRepo>> logger)
        {
            Repository = repository;
            Logger = logger;
        }

        public virtual ServiceResponse<IEnumerable<TModel>> GetAll()
        {
            try
            {
                var entities = Repository.GetAll().ToList();
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

        public virtual ServiceResponse<TModel> GetById(int id)
        {
            try
            {
                var entity = Repository.GetById(id);
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

        public virtual ServiceResponse<TModel> Insert(TModel model)
        {
            try
            {
                var entity = model.Adapt<TEntity>();
                var insertedEntity = Repository.Insert(entity);
                Repository.Save();
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

        public virtual ServiceResponse<TModel> Update(TModel model)
        {
            try
            {
                var entity = model.Adapt<TEntity>();
                var updatedEntity = Repository.Update(entity);
                Repository.Save();
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

        public virtual ServiceResponse<TModel> Delete(int id)
        {
            try
            {
                var entity = Repository.GetById(id);
                if (entity == null)
                {
                    return new ServiceResponse<TModel>
                    {
                        Success = false,
                        Message = "Entity not found"
                    };
                }

                var deletedEntity = Repository.Delete(entity);
                Repository.Save();
                var deletedModel = deletedEntity.Adapt<TModel>();
                return new ServiceResponse<TModel>
                {
                    Data = deletedModel,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting entity");
                return new ServiceResponse<TModel>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}