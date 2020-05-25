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
    public class UserCreation : DbContext
    {
        public DbSet<UserAccountDetails> useraccoiuntDetails { get; set; }
        public UserCreation() : base()
        {

        }

        // Constructor to use on a DbConnection that is already opened
        public UserCreation(DbConnection existingConnection, bool contextOwnsConnection)
      : base(existingConnection, contextOwnsConnection)
        {

        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<UserAccountDetails>().MapToStoredProcedures();
        //}
    }
}