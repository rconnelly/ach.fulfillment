namespace Ach.Fulfillment.Migrations.Impl
{
    using System.Data;
    using System.Data.SqlClient;

    using FluentMigrator;

    [Migration(201303081103)]
    public class EnableServiceBroker : ForwardOnlyMigration 
    {
        public override void Up()
        {
            this.Execute.WithConnection(PerformServiceBrokerEnable);    
        }

        private static void PerformServiceBrokerEnable(IDbConnection c, IDbTransaction t)
        {
            t.Commit();
            c.Close();
            try
            {
                using (var connection = new SqlConnection(c.ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "ALTER DATABASE " + c.Database + " SET ENABLE_BROKER";
                    command.ExecuteNonQuery();
                }
            }
            finally 
            {
                //c.Open();
            }
        }
    }
}
