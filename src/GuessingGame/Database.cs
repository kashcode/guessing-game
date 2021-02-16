using Microsoft.Data.Sqlite;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;

namespace GuessingGame
{
    public class Database : IDatabase
    {
        private readonly ILogger<Database> _logger;
        private readonly DatabaseSettings _databaseSettings;        

        public Database(ILogger<Database> logger, IOptions<DatabaseSettings> options)
        {
            _logger = logger;
            _databaseSettings = options.Value;
        }

        public TResult Execute<TResult>(Func<SqliteConnection, TResult> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var sqliteConnection = new SqliteConnection(_databaseSettings.DefaultConnection);

            bool closeConnection = false;
            try
            {
                closeConnection = sqliteConnection.State == ConnectionState.Closed || sqliteConnection.State == ConnectionState.Broken;

                if (closeConnection)
                {
                    sqliteConnection.Open();
                }                

                try
                {
                    return func(sqliteConnection);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);

                    throw;
                }
            }
            finally
            {
                if (closeConnection)
                {
                    sqliteConnection.Close();
                }
            }           
        }
    }
}
