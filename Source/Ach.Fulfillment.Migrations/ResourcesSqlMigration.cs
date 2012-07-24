namespace Ach.Fulfillment.Migrations
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    using ECM7.Migrator.Framework;

    public abstract class ResourcesSqlMigration : Migration
    {
        #region Properties

        protected virtual string DefaultBatchSeparator
        {
            get { return ";\r\n"; }
        }

        protected virtual int DefaultCommandTimeout
        {
            get { return 60; }
        }

        protected virtual string SqlResourcesNamePattern
        {
            get
            {
                return GetDefaultResourcesNamePattern(this.GetType());
            }
        }

        protected virtual string ResourceName
        {
            get
            {
                var resourceName = string.Format(
                    CultureInfo.InvariantCulture, 
                    this.SqlResourcesNamePattern, 
                    this.GetMigrationVersion());
                return resourceName;
            }
        }

        protected virtual string Query
        {
            get
            {
                var resourceName = this.ResourceName;
                var assembly = this.GetType().Assembly;
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    Contract.Assert(stream != null);
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    return Encoding.Default.GetString(buffer);
                }
            }
        }

        #endregion

        #region Methods

        public static string GetDefaultResourcesNamePattern(Type type)
        {
            Contract.Assert(type != null);
            return GetDefaultResourcesNamePattern(type.Assembly.GetName().Name);
        }

        public static string GetDefaultResourcesNamePattern(string prefix = null)
        {
            return
                (prefix == null ? string.Empty : prefix + ".") + "Resources.{0}.sql";
        }

        public override void Apply()
        {
            var queryDefinition = this.Query;
            Contract.Assert(!string.IsNullOrEmpty(queryDefinition));
            var batchSeparator = this.GetBatchSeparator(queryDefinition);
            var timeout = this.GetTimeout(queryDefinition);
            var query = this.GetCoreQuery(queryDefinition);
            var lines = query.Split(new[] { batchSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var command in lines.Select(Prepare))
            {
                this.ExecuteNonQuery(command, timeout);
            }
        }

        private static string Prepare(string line)
        {
            Contract.Assert(line != null);
            return line.Replace("\r", string.Empty);
        }

        private long GetMigrationVersion()
        {
            var attribute = this.GetType()
                .GetCustomAttributes(typeof(MigrationAttribute), true)
                .Cast<MigrationAttribute>()
                .FirstOrDefault();
            Contract.Assert(attribute != null);
            return attribute.Version;
        }

        private string GetCoreQuery(string queryDefinition)
        {
            // todo: get real query
            return queryDefinition;
        }

        // ReSharper disable UnusedParameter.Local
        private string GetBatchSeparator(string queryDefinition)
        // ReSharper restore UnusedParameter.Local
        {
            // todo: read batch separator
            return this.DefaultBatchSeparator;
        }

        // ReSharper disable UnusedParameter.Local
        private int GetTimeout(string queryDefinition)
        // ReSharper restore UnusedParameter.Local
        {
            // todo: read timeout
            return this.DefaultCommandTimeout;
        }

        private void ExecuteNonQuery(string query, int timeout)
        {
            try
            {
                this.BuildCommand(query, timeout).ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "Migration Version - {0}, Exception - {1}, Query - '{2}')", this.GetMigrationVersion(), ex.Message, query));
            }
        }

        private IDbCommand BuildCommand(string query, int timeout)
        {
            Contract.Assert(!string.IsNullOrEmpty(query));
            var cmd = this.Database.GetCommand();
            cmd.CommandTimeout = timeout;
            cmd.CommandText = query;
            return cmd;
        }

        #endregion
    }
}
