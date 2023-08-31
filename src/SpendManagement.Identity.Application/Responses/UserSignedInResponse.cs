namespace SpendManagement.Identity.Application.Responses
{
    public class UserSignedInResponse
    {
        public bool Success { get; }
        public List<string> Errors { get; }

        public UserSignedInResponse() =>
            Errors = new List<string>();

        public UserSignedInResponse(bool sucesso = true) : this() =>
            Success = sucesso;

        public void AddError(IEnumerable<string> erros) =>
            Errors.AddRange(erros);
    }
}
