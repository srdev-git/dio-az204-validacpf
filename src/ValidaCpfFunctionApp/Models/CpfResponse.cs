namespace ValidaCpfFunctionApp.Models
{
    public class CpfResponse
    {
        public string Cpf { get; set; }
        public bool Valido { get; set; }
        public string Mensagem { get; set; }
    }
}