﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Atom
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class atomEntities : DbContext
    {
        public atomEntities()
            : base("name=atomEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ut_MenuField> ut_MenuField { get; set; }
        public virtual DbSet<ut_MenuPageView> ut_MenuPageView { get; set; }
        public virtual DbSet<ut_RoleField> ut_RoleField { get; set; }
        public virtual DbSet<ut_Roles> ut_Roles { get; set; }
    }
}
