namespace CS.Data.Context.CustomerBoundedContext
{
    using System.Data.Entity;
    using System.Data.Common;
    using System.Data.SqlClient;
    using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
    using Microsoft.Azure.SqlDatabase.ElasticScale.Query;
    public partial class CustomerContext<T> : DbContext
    {
        public CustomerContext(ShardMap shardMap, T shardingKey, string connectionStr)
            : base(CreateDDRConnection(shardMap, shardingKey, connectionStr), true /* contextOwnsConnection */)
        {
        }

        //public CustomerContext(ShardMap shardMap, T shardingKey, string connectionStr)
        //   : base(CreateMultiShardConnection(shardMap, connectionStr), true /* contextOwnsConnection */)
        //{
        //}

        private static DbConnection CreateDDRConnection(ShardMap shardMap, T shardingKey, string connectionStr)
        {
            Database.SetInitializer<CustomerContext<T>>(null);

            SqlConnection conn = shardMap.OpenConnectionForKey<T>(shardingKey, connectionStr, ConnectionOptions.Validate);
            return conn;
        }

        private static MultiShardConnection CreateMultiShardConnection(ShardMap shardMap, string connectionStr)
        {
            Database.SetInitializer<CustomerContext<T>>(null);
            var shards = shardMap.GetShards();
            MultiShardConnection conn = new MultiShardConnection(shards, connectionStr);
            return conn;
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.Customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);
        }
    }

    
}
