using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace tcs_service.Repos.Base
{
    public interface IRepo<T>
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>> function);

        IEnumerable<T> GetAll();

        Task<bool> Exist(Expression<Func<T, bool>> function);

        Task<T> Find(Expression<Func<T, bool>> function);

        Task<T> Create(T t);
        Task<int> Create(IEnumerable<T> t);

        Task<T> Update(T t);

        Task<T> CreateOrUpdate(Expression<Func<T, bool>> func, T t);

        Task<T> Remove(Expression<Func<T, bool>> function);

        Task<IEnumerable<T>> RemoveAll(Expression<Func<T, bool>> function);
    }
}