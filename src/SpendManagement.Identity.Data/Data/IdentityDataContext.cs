using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SpendManagement.Identity.Data.Data
{
    public class IdentityDataContext(DbContextOptions<IdentityDataContext> options) : IdentityDbContext(options)
    {
    }
}
