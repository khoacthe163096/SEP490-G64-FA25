using DAL.vn.fpt.edu.entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.vn.fpt.edu.data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long, IdentityUserClaim<long>, IdentityUserRole<long>, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Map Identity to existing DB-First tables and columns
            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("user");
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).HasColumnName("id");
                b.Property(u => u.UserName).HasColumnName("username");
                b.Property(u => u.Email).HasColumnName("email");
                b.Property(u => u.PasswordHash).HasColumnName("password");
                b.Property(u => u.PhoneNumber).HasColumnName("phone");
                b.Property(u => u.RoleId).HasColumnName("role_id");
                b.Ignore(u => u.NormalizedUserName);
                b.Ignore(u => u.NormalizedEmail);
                b.Ignore(u => u.EmailConfirmed);
                b.Ignore(u => u.PhoneNumberConfirmed);
                b.Ignore(u => u.SecurityStamp);
                b.Ignore(u => u.ConcurrencyStamp);
                b.Ignore(u => u.LockoutEnabled);
                b.Ignore(u => u.LockoutEnd);
                b.Ignore(u => u.AccessFailedCount);
                b.Ignore(u => u.TwoFactorEnabled);
                // Lockout, SecurityStamp... map to hidden columns if available; otherwise keep defaults in memory
            });

            builder.Entity<ApplicationRole>(b =>
            {
                b.ToTable("role");
                b.HasKey(r => r.Id);
                b.Property(r => r.Id).HasColumnName("id");
                b.Property(r => r.Name).HasColumnName("name");
                b.Ignore(r => r.NormalizedName);
                b.Ignore(r => r.ConcurrencyStamp);
            });

            // If you have a join table, map it here; otherwise we will not use role assignments via IdentityUserRole
            // Not using UserRoles join table because schema stores role on user

            // Claims/Logins/Tokens tables are not used in current schema
            builder.Ignore<IdentityUserClaim<long>>();
            builder.Ignore<IdentityRoleClaim<long>>();
            builder.Ignore<IdentityUserLogin<long>>();
            builder.Ignore<IdentityUserToken<long>>();
        }
    }
}


