namespace CloudPhoto.Services.Data.DapperService
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;

    using global::Dapper;
    using Microsoft.Extensions.Configuration;

    public class DapperService : IDapperService
    {
        private readonly IConfiguration config;
        private readonly string connectionstring = "DefaultConnection";

        public DapperService(IConfiguration config)
        {
            this.config = config;
        }

        public void Dispose()
        {
        }

        public T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = new SqlConnection(this.config.GetConnectionString(this.connectionstring));
            return db.Query<T>(sp, parms, commandType: commandType).FirstOrDefault();
        }

        public List<T> GetAll<T>(string sp, object parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(this.config.GetConnectionString(this.connectionstring));
            return db.Query<T>(sp, parms, commandType: commandType).ToList();
        }

        public DbConnection GetDbconnection()
        {
            return new SqlConnection(this.config.GetConnectionString(this.connectionstring));
        }
    }
}
