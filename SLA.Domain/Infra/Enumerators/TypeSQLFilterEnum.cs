namespace SLA.Domain.Infra.Enumerators
{
    public enum TypeSQLFilterEnum
    {
        Equal = 0,          // ==
        Like = 1,           // PARECIDO %V%
        NotEqual = 2,       // DIFERENTE V
        Starts = 3,         // INICIA V%
        Ends = 4,           // TERMINA %V
        Bigger = 5,         // MAIOR QUE >
        BiggerEqual = 6,    // MAIOR IGUAL QUE >=
        Less = 7,           // MENOR QUE <
        LessEqual = 8,      // MENOR IGUAL QUE <=
        NotIn = 9,          // NÃO PRESENTE ()
        In = 10,            // PRESENTE ()
        Between = 11        // ENTRE V1 E V2
    }
}
