using SLA.Domain.Application.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SLA.Domain.Application.Process
{
    public class RequestModel<T>
    {
        public HttpStatusCode Status { get; set; } = HttpStatusCode.Continue;
        public ReturnEnum Type { get; set; } = ReturnEnum.NoProccess;
        public string Message { get; set; } = string.Empty;
        public ErrorModel Erro { get; set; } = new ErrorModel();
        public T Return { get; set; } = Activator.CreateInstance<T>();

        public RequestModel()
        {
            Type = ReturnEnum.NoProccess;
        }

        public bool Validate()
        {
            if (Type == ReturnEnum.Success && (Status == HttpStatusCode.OK || Status == HttpStatusCode.Created || Status == HttpStatusCode.Accepted)) return true;
            else return false;
        }

        public void Success(HttpStatusCode Status, string Message, T Return)
        {
            this.Status = Status;
            this.Type = ReturnEnum.Success;
            this.Message = Message;
            this.Return = Return;
        }

        public void Fail(HttpStatusCode Status, string Message, ErrorModel? Erro = null)
        {
            this.Status = Status;
            this.Type = ReturnEnum.Fail;
            this.Message = Message;
            if (Erro != null) this.Erro = Erro;
        }

        public void Fail(HttpStatusCode Status, string Message, T Return, ErrorModel? Erro = null)
        {
            this.Status = Status;
            this.Type = ReturnEnum.Fail;
            this.Message = Message;            
            this.Return = Return;
            if (Erro != null) this.Erro = Erro;
        }

        public void Error(HttpStatusCode Status, string Message, ErrorModel Erro)
        {
            this.Status = Status;
            this.Type = ReturnEnum.Error;
            this.Message = Message;
            this.Erro = Erro;
        }
    }
}
