using CS.Data.Context.CustomerBoundedContext;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        private static string s_server = "devcchneil01.database.windows.net";
        private static string s_shardmapmgrdb = "ElasticScaleStarterKit_ShardMapManagerDb";
        private static string s_shard1 = "ElasticScaleStarterKit_Shard0";
        private static string s_shard2 = "ElasticScaleStarterKit_Shard1";
        private static string s_userName = "devadmin";
        private static string s_password = "Afpftcb1td";
        private static string s_applicationName = "ElasticDbWithEntityFramework";
        static void Main(string[] args)
        {

            SqlConnectionStringBuilder connStrBldr = new SqlConnectionStringBuilder
            {
                UserID = s_userName,
                Password = s_password,
                ApplicationName = s_applicationName,
                MultipleActiveResultSets = true
            };

            // Bootstrap the shard map manager, register shards, and store mappings of tenants to shards
            // Note that you can keep working with existing shard maps. There is no need to 
            // re-create and populate the shard map from scratch every time.
            //Console.WriteLine("Checking for existing shard map and creating new shard map if necessary.");

            Sharding sharding = new Sharding(s_server, s_shardmapmgrdb, connStrBldr.ConnectionString);
            //sharding.RegisterNewShard(s_server, s_shard1, connStrBldr.ConnectionString, s_tenantId1);
            //sharding.RegisterNewShard(s_server, s_shard2, connStrBldr.ConnectionString, s_tenantId2);

            
            SqlDatabaseUtils.SqlRetryPolicy.ExecuteAction(() =>
            {
                using (var db = new CustomerContext<int>(sharding.ShardMap, 19, connStrBldr.ConnectionString))
                {
                    var query = from b in db.Orders
                                where b.CustomerId == 19
                                orderby b.OrderId
                                select b;

                    Console.WriteLine("All orders for customer id {0}:", 19);
                    foreach (var item in query)
                    {
                        //Console.WriteLine($"{item.OrderId}\t{item.OrderDate}");
                        Console.WriteLine($"{item.OrderId}\t{item.Product.Name}\t{item.OrderDate}");
                    }
                }
            });

            Console.Read();
        }
    }
}
