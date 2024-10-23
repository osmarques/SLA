using SLA.Domain.Application.Process;
using SLA.Domain.Infra.Data;

namespace SLA.Domain.Infra.Interfaces
{
    public interface IDataSet<T> where T : DataModel
    {
        #region Sincrono        
        ReturnModel<long> Count();
        ReturnModel<List<T>> ReadList();
        ReturnModel<List<T>> ReadList(IDataFilterCollection Filters);        
        ReturnModel<T> Read(long Id);        
        ReturnModel<T> Create(T Data);        
        ReturnModel<bool> Update(T Data);        
        ReturnModel<bool> Delete(T Data);        
        #endregion

        #region Assincrono        
        Task<ReturnModel<long>> CountAsync();
        Task<ReturnModel<List<T>>> ReadListAsync();
        Task<ReturnModel<List<T>>> ReadListAsync(IDataFilterCollection Filters);        
        Task<ReturnModel<T>> ReadAsync(long Id);        
        Task<ReturnModel<T>> CreateAsync(T Data);        
        Task<ReturnModel<bool>> UpdateAsync(T Data);        
        Task<ReturnModel<bool>> DeleteAsync(T Data);
        #endregion
    }
}
