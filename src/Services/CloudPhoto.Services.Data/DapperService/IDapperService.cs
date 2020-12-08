namespace CloudPhoto.Services.Data.DapperService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    using global::Dapper;

    public interface IDapperService : IDisposable
    {
        DbConnection GetDbconnection();

        T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);

        List<T> GetAll<T>(string sp, object parms, CommandType commandType = CommandType.StoredProcedure);
    }
}
