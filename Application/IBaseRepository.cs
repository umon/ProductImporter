using Application.Models;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<T> GetAsync(Expression<Func<T, bool>> expression = null);
        Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> expression = null);
        Task InsertAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Expression<Func<T, bool>> expression);
        Task<BulkWriteResultModel> BulkWriteAsync(IEnumerable<T> inserts = null, IEnumerable<T> updates = null, IEnumerable<T> deletes = null);
        Task<bool> ExistsAsync(Expression<Func<T,bool>> expression);
    }
}
