namespace SpendManagement.Identity.Application.Responses
{
    public class UserSignInResponse
    {
        public bool Success { get; }
        public List<string> Errors { get; }

        public UserSignInResponse() =>
            Errors = new List<string>();

        public UserSignInResponse(bool sucesso = true) : this() =>
            Success = sucesso;

        public void AddError(IEnumerable<string> erros) =>
            Errors.AddRange(erros);
    }
}
