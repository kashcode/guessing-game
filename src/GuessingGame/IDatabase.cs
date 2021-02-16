using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuessingGame
{
    public interface IDatabase
    {
        TResult Execute<TResult>(Func<SqliteConnection, TResult> func);
    }
}
