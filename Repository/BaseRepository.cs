using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace WebApplication1.Repository
{
    /// <summary>
    /// A base repository that has the Database connection information that all derived repositories use.
    /// </summary>
    public class BaseRepository
    {
        private string connectionString;
        protected BaseRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetValue<string>("DBInfo:ConnectionString");
        }

        protected IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(connectionString);
            }
        }
    }
}
