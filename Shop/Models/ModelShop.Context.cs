﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shop.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ShopEntities : DbContext
    {
        public ShopEntities()
            : base("name=ShopEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ConcreteProduct> ConcreteProduct { get; set; }
        public virtual DbSet<ConcreteProductDetails> ConcreteProductDetails { get; set; }
        public virtual DbSet<ProductType> ProductType { get; set; }
        public virtual DbSet<Produts> Produts { get; set; }

        public System.Data.Entity.DbSet<Shop.Models.ProductWithAllDetails> ProductWithAllDetails { get; set; }
    }
}
