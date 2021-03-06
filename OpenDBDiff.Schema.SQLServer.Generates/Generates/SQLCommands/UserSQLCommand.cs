using OpenDBDiff.Schema.SQLServer.Generates.Model;
using System.Text;

namespace OpenDBDiff.Schema.SQLServer.Generates.Generates.SQLCommands
{
    internal static class UserSQLCommand
    {
        public static string Get(DatabaseInfo.SQLServerVersion version, DatabaseInfo.SQLServerEdition edition)
        {
            switch (version)
            {
                case DatabaseInfo.SQLServerVersion.SQLServer2000:
                case DatabaseInfo.SQLServerVersion.SQLServer2005:
                case DatabaseInfo.SQLServerVersion.SQLServer2008:
                case DatabaseInfo.SQLServerVersion.SQLServer2008R2:
                    return Get2008();

                case DatabaseInfo.SQLServerVersion.SQLServerAzure10:
                    return GetAzure();

                default:
                    if (edition == DatabaseInfo.SQLServerEdition.Azure)
                        return GetAzure();
                    else
                        return Get2008();
            }
        }

        private static string Get2008()
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT is_fixed_role, type, ISNULL(suser_sname(sid),'') AS Login,Name,principal_id, ISNULL(default_schema_name,'') AS default_schema_name ");
            sql.AppendLine("FROM sys.database_principals ");
            sql.AppendLine("WHERE type IN ('S','U','A','R') ");
            sql.AppendLine("ORDER BY Name");
            return sql.ToString();
        }

        private static string GetAzure()
        {
            var sql = new StringBuilder();
            //to get LoginName in Azure (asside for the current login) you would have to link to master and query sys.sysusers or sys.sql_users
            //the CASE test below will at least get you the Current login
            sql.AppendLine("SELECT is_fixed_role, type, CASE WHEN suser_sid()=sid THEN suser_sname() ELSE '' END  AS Login,Name,principal_id, ISNULL(default_schema_name,'') AS default_schema_name ");
            sql.AppendLine("FROM sys.database_principals ");
            sql.AppendLine("WHERE type IN ('S','U','A','R') ");
            sql.AppendLine("ORDER BY Name");
            return sql.ToString();
        }
    }
}
