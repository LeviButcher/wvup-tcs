using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace tcs_service.Repos.Base {
    ///<summary>Methods that return data for a table from a database</summary>
    public interface IRepo<T> {
        ///<summary>Return all the records that matches the provided function</summary>
        IEnumerable<T> GetAll (Expression<Func<T, bool>> function);

        ///<summary>Return all the records in the table</summary>
        IEnumerable<T> GetAll ();

        ///<summary>Check if a record exists that matches the provided function</summary>
        Task<bool> Exist (Expression<Func<T, bool>> function);

        ///<summary>Return the record that matches the provided function</summary>
        Task<T> Find (Expression<Func<T, bool>> function);

        ///<summary>Create a new record in the table</summary>
        Task<T> Create (T t);

        ///<summary>Create several new records in the table</summary>
        Task<int> Create (IEnumerable<T> t);

        ///<summary>Update a record in the table</summary>
        Task<T> Update (T t);

        ///<summary>Create or update a record in the table depending on if a record is found with the function provided</summary>
        Task<T> CreateOrUpdate (Expression<Func<T, bool>> func, T t);

        ///<summary>Remove a record from the table</summary>
        Task<T> Remove (Expression<Func<T, bool>> function);

        ///<summary>Remove all records that match the provided function from the table</summary>
        Task<IEnumerable<T>> RemoveAll (Expression<Func<T, bool>> function);
    }
}