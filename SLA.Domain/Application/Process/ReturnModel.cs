using SLA.Domain.Application.Enumerators;

namespace SLA.Domain.Application.Process
{
    public class ReturnModel<T>
    {
        public ReturnEnum Type { get; private set; } = ReturnEnum.NoProccess;
        public string Message { get; private set; } = string.Empty;
        public ErrorModel Erro { get; private set; } = new ErrorModel();
        public T Return { get; private set; }

        public ReturnModel() { }

        public bool IsSuccessful() => Type == ReturnEnum.Success;

        public void SetSuccess(string Message, T Return)
        {
            this.Type = ReturnEnum.Success;
            this.Message = Message;
            this.Return = Return;
        }

        public void SetFail(string Message, ErrorModel? Erro = null)
        {
            this.Type = ReturnEnum.Fail;
            this.Message = Message;            
            if (Erro != null)
            {
                this.Erro = Erro;
                Console.WriteLine($"FALHA: {Erro.Message}. DETALHES: {Erro.Detail}. STACK: {Erro.Stack}");
            }
        }

        public void SetFail(string Message, T Retorno, ErrorModel? Erro = null)
        {
            this.Type = ReturnEnum.Fail;
            this.Message = Message;            
            this.Return = Retorno;
            if (Erro != null)
            {
                this.Erro = Erro;
                Console.WriteLine($"FALHA: {Erro.Message}. DETALHES: {Erro.Detail}. STACK: {Erro.Stack}");
            }
        }

        public void SetError(string Message, ErrorModel Erro)
        {
            this.Type = ReturnEnum.Error;
            this.Message = Message;
            this.Erro = Erro;
            Console.WriteLine($"ERRO: {Erro.Message}. DETALHES: {Erro.Detail}. STACK: {Erro.Stack}");
        }
    }
}
