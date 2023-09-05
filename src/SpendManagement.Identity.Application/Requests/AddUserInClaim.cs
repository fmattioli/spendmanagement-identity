namespace SpendManagement.Identity.Application.Requests
{
    public class AddUserInClaim
    {
        public string? Email { get; set; }
        public ClaimType ClaimType { get; set; }
        public ClaimValue ClaimValue { get; set; }
    }

    public enum ClaimType
    {
        Category,
        Receipt
    }

    public enum ClaimValue
    {
        Read,
        Insert,
        Update,
        Delete
    }
}
