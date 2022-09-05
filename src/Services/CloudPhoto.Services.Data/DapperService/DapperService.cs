namespace CloudPhoto.Services.Data.DapperService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;

    using Dapper;
    using Microsoft.Extensions.Configuration;

    public class DapperService : IDapperService
    {
        private readonly IConfiguration config;
        private readonly string connectionString = "DefaultConnection";

        public DapperService(IConfiguration config)
        {
            this.config = config;
        }

        public T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString(connectionString));
            return db.Query<T>(sp, parms, commandType: commandType).FirstOrDefault();
        }

        public List<T> GetAll<T>(string sp, object parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString(connectionString));
            return db.Query<T>(sp, parms, commandType: commandType).ToList();
        }

        public DbConnection GetDbConnection()
        {
            return new SqlConnection(config.GetConnectionString(connectionString));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}
