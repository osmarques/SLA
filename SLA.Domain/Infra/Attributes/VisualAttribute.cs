using System.Reflection;

namespace SLA.Domain.Infra.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class Visual : Attribute
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public bool Visivel { get; set; } = true;
        public int Sequencia { get; set; } = 3;

        public Visual() { }

        public Visual(string Titulo, string Descricao, Object Self)
        {
            this.Titulo = Titulo;
            this.Descricao = Descricao;
            this.Sequencia = GetFieldPosition(Self);
        }

        public Visual(string Titulo, string Descricao, bool Visivel, Object Self)
        {
            this.Titulo = Titulo;
            this.Descricao = Descricao;
            this.Visivel = Visivel;
            this.Sequencia = GetFieldPosition(Self);
        }

        public Visual(string Titulo, string Descricao, bool Visivel, int Sequencia)
        {
            this.Titulo = Titulo;
            this.Descricao = Descricao;
            this.Visivel = Visivel;
            this.Sequencia = Sequencia;
        }

        private static int GetFieldPosition(Object ObjectClass) 
        {
            int maxSequence = -1;
            var type = ObjectClass.GetType();
            var visualAttribute = type.GetCustomAttribute<Visual>();

            if (visualAttribute != null)
            {
                var visualAttributes = type.GetCustomAttributes<Visual>();                
                foreach (var att in visualAttributes)
                {
                    maxSequence = Math.Max(maxSequence, att.Sequencia);
                }

                visualAttribute.Sequencia = maxSequence + 1;
            }

            return maxSequence;
        }

        public Visual(List<CustomAttributeNamedArgument> attributes)
        {
            foreach (var att in attributes)
            {
                var value = att.TypedValue.Value;
                switch (att.MemberName)
                {
                    case "Titulo":
                        Titulo = Convert.ToString(value);
                        break;
                    case "Descricao":
                        Descricao = Convert.ToString(value);
                        break;
                    case "Visivel":
                        if (value is bool b)
                        {
                            Visivel = b;
                        }
                        break;
                    case "Sequencia":
                        if (value is int i)
                        {
                            Sequencia = i;
                        }
                        break;
                }
            }
        }
    }
}
