namespace CloudPhoto.Services.Data.DapperService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    using Dapper;

    public interface IDapperService : IDisposable
    {
        DbConnection GetDbConnection();

        T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);

        List<T> GetAll<T>(string sp, object parms, CommandType commandType = CommandType.StoredProcedure);
    }
}
