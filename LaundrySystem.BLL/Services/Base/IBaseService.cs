namespace LaundrySystem.BLL.Infrastructure.Interfaces
{
    using LaundrySystem.Domain.Model.Responses;
    using System.Collections.Generic;

    public interface IBaseService<TModel>
    {
        ServiceResponse<IEnumerable<TModel>> GetAll();

        ServiceResponse<TModel> GetById(int id);

        ServiceResponse<TModel> Insert(TModel model);

        ServiceResponse<TModel> Update(TModel model);

        ServiceResponse<TModel> Delete(int id);
    }
}