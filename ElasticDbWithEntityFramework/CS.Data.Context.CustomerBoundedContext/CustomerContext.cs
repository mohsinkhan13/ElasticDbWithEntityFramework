namespace CS.Data.Context.CustomerBoundedContext
{
    using System.Data.Entity;
    using System.Data.Common;
    using System.Data.SqlClient;
    using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
    public partial class CustomerContext<T> : DbContext
    {
        public CustomerContext()
            : base("name=CustomerModel")
        {
        }
        public CustomerContext(ShardMap shardMap, T shardingKey, string connectionStr)
            : base(CreateDDRConnection(shardMap, shardingKey, connectionStr), true /* contextOwnsConnection */)
        {
        }

        // Only static methods are allowed in calls into base class c'tors
        private static DbConnection CreateDDRConnection(ShardMap shardMap, T shardingKey, string connectionStr)
        {
            // No initialization
            Database.SetInitializer<CustomerContext<T>>(null);

            // Ask shard map to broker a validated connection for the given key
            SqlConnection conn = shardMap.OpenConnectionForKey<T>(shardingKey, connectionStr, ConnectionOptions.Validate);
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
