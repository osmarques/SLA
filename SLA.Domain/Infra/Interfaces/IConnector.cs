using SLA.Domain.Application.Enumerators;
using SLA.Domain.Application.Process;
using System.Data;

namespace SLA.Domain.Infra.Interfaces
{
    public interface IConnector
    {
        #region Properties 
        public TypeModelEnum Type { get; set; }
        public string GetConnectionString();
        public bool Log { get; set; }
        #endregion

        #region Connection
        IDbTransaction? Transaction();
        void Roolback(IDbTransaction? Transaction);
        void Commit(IDbTransaction? Transaction);

        void Open();
        bool Active();
        void Close();
        #endregion

        #region Methods
        ReturnModel<bool> Execute(string Command);
        Task<ReturnModel<bool>> ExecuteAsync(string Command);
        ReturnModel<dynamic> ExecuteReturning(string Command);
        Task<ReturnModel<dynamic>> ExecuteReturningAsync(string Command);
        ReturnModel<long> Count(string Command);
        Task<ReturnModel<long>> CountAsync(string Command);
        ReturnModel<DataTable> Query(string Command);
        Task<ReturnModel<DataTable>> QueryAsync(string Command);
        #endregion
    }
}
