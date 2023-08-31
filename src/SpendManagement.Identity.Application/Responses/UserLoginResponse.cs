using System.Text.Json.Serialization;

namespace SpendManagement.Identity.Application.Responses
{
    public class UserLoginResponse
    {
        public bool Success => Errors?.Count == 0;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AccessToken { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RefreshToken { get;}

        public List<string>? Errors { get;}

        public UserLoginResponse() =>
            Errors = new List<string>();

        public UserLoginResponse(string accessToken, string refreshToken) : this()
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public UserLoginResponse AddErrors(string erro)
        {
            Errors?.Add(erro);
            return this;
        }

        public void AdicionarErros(IEnumerable<string> erros) =>
            Errors?.AddRange(erros);
    }
}
