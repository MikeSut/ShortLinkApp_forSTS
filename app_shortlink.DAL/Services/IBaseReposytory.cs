using System.Linq;
using System.Threading.Tasks;

namespace app_shortlink.DAL.Services;

public interface IBaseReposytory<T>
{
    Task Create(T entity);

    IQueryable<T> GetALL();

    Task Delete(T entity);

    Task<T> Update(T entity);
}