using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PayToCardsSystem.DAL
{
    public class LoginContext : DbContext
    {
        public DbSet<UserAccountDetails> useraccoiuntDetails { get; set; }
        public LoginContext() : base()
        {

        }

        // Constructor to use on a DbConnection that is already opened
        public LoginContext(DbConnection existingConnection, bool contextOwnsConnection)
      : base(existingConnection, contextOwnsConnection)
        {

        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<UserAccountDetails>().MapToStoredProcedures();
        //}
    }

    [Table("user_account_details")]
    public class UserAccountDetails
    {
        [Key]
        public int _id { get; set; }

        public int UserId { get; set; }

        public string SwishUserId { get; set; }

        public string Password { get; set; }

        public string EntityId { get; set; }
    }
}