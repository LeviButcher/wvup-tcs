
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace tcs_service.Repos.Base
{
    public interface IRepo<T>
    {
        IEnumerable<T> GetAll(Expression<Func<T, Boolean>> function);

        IEnumerable<T> GetAll();

        Task<bool> Exist(Expression<Func<T, Boolean>> function);

        Task<T> Find(Expression<Func<T, Boolean>> function);

        Task<T> Create(T t);

        Task<T> Update(T t);

        Task<T> Remove(Expression<Func<T, Boolean>> function);

        Task<IEnumerable<T>> RemoveAll(Expression<Func<T, Boolean>> function);
    }
}