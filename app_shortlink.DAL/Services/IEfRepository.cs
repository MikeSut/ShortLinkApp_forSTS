using System.Collections.Generic;
using System.Threading.Tasks;
using app_shortlink.Domain.Entity;

namespace app_shortlink.DAL.Services;

public interface IEfRepository<T> where T: BaseEntity
{
    List<T> GetAll();
    T GetById(long id);
    Task<long> Add(T entity);
}