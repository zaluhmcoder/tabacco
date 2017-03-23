using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TeamTrack.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Framework.Configuration;
using Microsoft.AspNetCore.Identity;
using System;

namespace TeamTrack.Core.Data
{
    public class TeamTrackDbContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
        }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Card> Cards { get; set; }

        public DbSet<UserCard> UserCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(x =>
            {
                x.ToTable("User");
            });

            modelBuilder.Entity<Role>(x =>
            {
                x.ToTable("Role");
            });

            modelBuilder.Entity<IdentityUserRole<int>>(x =>
            {
                x.ToTable("UserRole");
                x.HasKey(y => new { y.UserId, y.RoleId });
            });

            modelBuilder.Entity<IdentityUserLogin<int>>(x =>
            {
                x.ToTable("UserLogin");
                x.HasKey(y => new { y.LoginProvider, y.ProviderKey, y.UserId });
            });

            modelBuilder.Entity<IdentityUserToken<int>>(x =>
            {
                x.ToTable("UserToken");
                x.HasKey(y => new { y.LoginProvider, y.Name, y.UserId });
            });

            modelBuilder.Entity<IdentityUserClaim<int>>(x =>
            {
                x.ToTable("UserClaim");
            });

            modelBuilder.Entity<IdentityRoleClaim<int>>(x =>
            {
                x.ToTable("RoleClaim");
            });

            modelBuilder.Entity<UserCard>().HasKey(x => new { x.UserId, x.CardId });
        }

        public async void EnsureSeedData(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            string[] roles = { "Administrator", "SuperUser", "User" };

            for (int i = 0; i < roles.Length; i++)
            {
                if (roleManager.FindByNameAsync(roles[i]).Result == null)
                {
                    await roleManager.CreateAsync(new Role() { Name = roles[i] });
                }
            }

            for (int i = 0; i < userData.Length; i++)
            {
                var userDataItem = userData[i].Split(',');
                var existingUser = await userManager.FindByNameAsync(userDataItem[0]);

                if (existingUser == null)
                {
                    var user = new User()
                    {
                        UserName = userDataItem[0],
                        FullName = userDataItem[1],
                        Email = userDataItem[2],
                        Title = userDataItem[3],
                        EmailConfirmed = true,
                        IsActive = true,
                        TwoFactorEnabled = false,
                        CreateDate = DateTime.Now
                    };

                    await userManager.CreateAsync(user, userDataItem[4]);
                    await userManager.SetLockoutEnabledAsync(user, false);

                    var isInRole = await userManager.IsInRoleAsync(user, userDataItem[5]);

                    if (!isInRole)
                    {
                        await userManager.AddToRoleAsync(user, userDataItem[5]);
                    }
                }
            }
        }

        private static string[] userData =
        {
            "egenel,Ersin Genel,ersingenel@gmail.con,Developer,142536,Administrator",
            "bsen,Başar Sen,basarsen@basarsen.com.tr,Developer,142536,SuperUser",
            "eozturk,Efkan Ozturk,efkanzoturk67@gmail.con,Developer,142536,User"
        };
    }
}
