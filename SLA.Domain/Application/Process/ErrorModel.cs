namespace SLA.Domain.Application.Process
{
    public class ErrorModel
    {
        public int Code { get; set; } = -1;
        public string Message { get; set; } = string.Empty;
        public string? Detail { get; set; } = string.Empty;
        public string? Stack { get; set; } = string.Empty;

        public ErrorModel() { }

        public ErrorModel(string Message, string Detail, string? Stack)
        {
            this.Message = Message;
            this.Detail = Detail;
            this.Stack = string.IsNullOrEmpty(Stack) ? "NO MAP" : Stack;
        }

        public ErrorModel(int Code, string Message, string Detail, string? Stack)
        {
            this.Code = Code;
            this.Message = Message;
            this.Detail = Detail;
            this.Stack = string.IsNullOrEmpty(Stack) ? "NO MAP" : Stack;
        }
    }
}
