namespace Ach.Fulfillment.Migrations.Impl
{
    using System.Configuration;
    using System.Data;
    using System.Globalization;

    using FluentMigrator;

    [Migration(201303100000)]
    public class CheckServiceBrokerEnabled : ForwardOnlyMigration 
    {
        public override void Up()
        {
            this.Execute.WithConnection(PerformServiceBrokerEnable);    
        }

        private static void PerformServiceBrokerEnable(IDbConnection connection, IDbTransaction transaction)
        {
            var isBrokerEnabled = IsBrokerEnabled(connection, transaction);
            if (!isBrokerEnabled)
            {
                isBrokerEnabled = TryEnabledBroker(connection, transaction);
                if (isBrokerEnabled)
                {
                    throw new ConfigurationErrorsException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Service Broker was not enabled for database '{0}', but was successfully enabled by migator. You should restart migrator to continue applying other operations.",
                            connection.Database));
                }

                throw new ConfigurationErrorsException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Service Broker is not enabled for database '{0}'. You should manually enable it on '{0}' database.",
                        connection.Database));
            }
        }

        private static bool IsBrokerEnabled(IDbConnection connection, IDbTransaction transaction)
        {
            bool isBrokerEnabled;
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "select is_broker_enabled from sys.databases where name = @name";
                var p = command.CreateParameter();
                p.ParameterName = "name";
                p.DbType = DbType.String;
                p.Value = connection.Database;
                command.Parameters.Add(p);
                var scalar = command.ExecuteScalar();
                isBrokerEnabled = (bool)scalar;
            }

            return isBrokerEnabled;
        }

        private static bool TryEnabledBroker(IDbConnection connection, IDbTransaction transaction)
        {
            transaction.Commit();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "alter database " + connection.Database + " set ENABLE_BROKER with rollback immediate";
                command.ExecuteNonQuery();
            }

            return IsBrokerEnabled(connection, transaction);
        }
    }
}
