using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace Flexpod.MVC.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext() : base("AuthConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class MigrationConfiguration : DbMigrationsConfiguration<UsersContext>
    {
        public MigrationConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(UsersContext context)
        {
            WebSecurity.InitializeDatabaseConnection("AuthConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
#if DEBUG
            if (false == WebSecurity.UserExists("admin"))
            {
                WebSecurity.CreateUserAndAccount("admin", "password", propertyValues: new
                {
                    EmailAddress = "info@flexpod.nl"
                });
            }
#endif
            // demo data
            if (false == WebSecurity.UserExists("John"))
            {
                WebSecurity.CreateUserAndAccount("John", "******", propertyValues: new
                {
                    EmailAddress = "john.do@flexpod.nl"
                });
                // set the PasswordChangedDate of John
                var johnId = WebSecurity.GetUserId("John");
                var threeMonthsBack = DateTime.Now.AddMonths(-3);
                var result = context.Database.ExecuteSqlCommand(
                    "UPDATE webpages_Membership SET PasswordChangedDate = @p0 WHERE UserId = @p1",
                    threeMonthsBack, johnId);
            }
            if (false == WebSecurity.UserExists("Jane"))
            {
                WebSecurity.CreateUserAndAccount("Jane", "******", propertyValues: new
                {
                    EmailAddress = "jane.do@flexpod.nl"
                });
            }
            // lock-out the account by logging in 10 times with wrong password
            for (int i = 0; i < 10; i++)
                WebSecurity.Login("Jane", "WrongPassword");
        }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class FlexpodUserModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "E-mail address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Last password changed")]
        public DateTime LastPasswordChangedDate { get; set; }
    }
}
