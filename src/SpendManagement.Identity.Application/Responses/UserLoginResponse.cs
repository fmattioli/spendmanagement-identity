using System.Text.Json.Serialization;

namespace SpendManagement.Identity.Application.Responses
{
    public class UserLoginResponse
    {
        public bool Sucesso => Erros?.Count == 0;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AccessToken { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RefreshToken { get;}

        public List<string>? Erros { get;}

        public UserLoginResponse() =>
            Erros = new List<string>();

        public UserLoginResponse(string accessToken, string refreshToken) : this()
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public void AddError(string erro) =>
            Erros?.Add(erro);

        public void AdicionarErros(IEnumerable<string> erros) =>
            Erros?.AddRange(erros);
    }
}
