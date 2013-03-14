namespace Ach.Fulfillment.Migrations.Impl
{
    using System.Configuration;
    using System.Data;
    using System.Globalization;

    using FluentMigrator;

    [Migration(201303100001)]
    public class CheckFilestreamSupport : ForwardOnlyMigration
    {
        public override void Up()
        {
            this.Execute.WithConnection(PerformFilestreamSupport);
        }

        private static void PerformFilestreamSupport(IDbConnection connection, IDbTransaction transaction)
        {
            CheckFilestreamEnableForInstance(connection, transaction);
            CheckFilestreamEnableForDatabase(connection, transaction);
        }

        private static void CheckFilestreamEnableForInstance(IDbConnection connection, IDbTransaction transaction)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "select SERVERPROPERTY ('FilestreamEffectiveLevel')";
                var scalar = command.ExecuteScalar();
                var value = (int)scalar;
                if (value != 3)
                {
                    var message = string.Format(
                        CultureInfo.InvariantCulture, 
                        "Current sql instance should have enabled FILESTREAM for Transact-SQL with Win32 remote streaming access (Command {0} should return '3')", 
                        command.CommandText);
                    throw new ConfigurationErrorsException(message);
                }
            }
        }

        private static void CheckFilestreamEnableForDatabase(IDbConnection connection, IDbTransaction transaction)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "select count(*) from sys.database_files where type_desc = 'FILESTREAM'";
                var scalar = command.ExecuteScalar();
                var value = (int)scalar;
                if (value == 0)
                {
                    var message = string.Format(
                        CultureInfo.InvariantCulture, 
                        "Database '{0}' does not have default FILESTREAM file group. Please add FILESTREAM file group and restart migrator.", 
                        connection.Database);
                    throw new ConfigurationErrorsException(message);
                }
            }
        }
    }
}