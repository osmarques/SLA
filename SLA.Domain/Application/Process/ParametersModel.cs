namespace SLA.Domain.Application.Process
{
    public class ParametersModel<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }

        public ParametersModel (string Name, T Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }
}
