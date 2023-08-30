using Microsoft.AspNetCore.Authorization;

namespace SpendManagement.Identity.Application.PolicyRequirements
{
    public class HorarioComercialRequirement : IAuthorizationRequirement
    {
        public HorarioComercialRequirement() { }
    }
}
