namespace SpendManagement.Identity.Application.Responses
{
    public class UserSignInResponse
    {
        public bool Sucesso { get; }
        public List<string> Erros { get; }

        public UserSignInResponse() =>
            Erros = new List<string>();

        public UserSignInResponse(bool sucesso = true) : this() =>
            Sucesso = sucesso;

        public void AdicionarErros(IEnumerable<string> erros) =>
            Erros.AddRange(erros);
    }
}
