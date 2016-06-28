using CS.Data.Context.CustomerBoundedContext;
using System;
using System.Data.SqlClient;
using System.Linq;

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

            Sharding sharding = new Sharding(s_server, s_shardmapmgrdb, connStrBldr.ConnectionString);

            
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
                        Console.WriteLine($"{item.OrderId}\t{item.Product.Name}\t{item.OrderDate}");
                    }
                }
            });

            Console.Read();
        }
    }
}
