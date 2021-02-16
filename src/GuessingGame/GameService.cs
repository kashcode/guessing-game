using GuessingGame.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuessingGame
{
    public class GameService : IGameService
    {
        private readonly ILogger<GameService> _logger;
        private readonly IDatabase _database;

        public GameService(ILogger<GameService> logger, IDatabase database)
        {
            _logger = logger;
            _database = database;
        }

        public Game CreateGame(Player player)
        {
            var game = new Game
            {
                Uuid = Guid.NewGuid().ToString()
            };

            var insert = _database.Execute(connection => {

                using var cmd = new SqliteCommand(
                    @"insert into games(uuid, secret, user_id) 
                    values($uuid, $secret, $user_id)", connection);
                cmd.Parameters.AddWithValue("$uuid", game.Uuid.ToUpper());
                cmd.Parameters.AddWithValue("$secret", GetSecretNumber(4));
                cmd.Parameters.AddWithValue("$user_id", player.Id);

                return cmd.ExecuteNonQuery();
            });

            return game;
        }

        public void MakeGuess(string gameUuid, int[] secret, int[] input)
        {
            // check input agains secret
            var m = 0;
            var p = 0;
            for (int i = 0; i < secret.Length; i++)
            {
                for (int j = 0; j < input.Length; j++)
                {
                    if(secret[i] == input[j])
                    {
                        m++;

                        if(i == j)
                        {
                            p++;
                        }
                    }
                }
            }

            _ = _database.Execute(connection => {
                using (var cmd = new SqliteCommand(
                    @"insert into guesses(game_uuid, input, m , p) 
                    values ($game_uuid, $input, $m, $p)", connection))
                {
                    cmd.Parameters.AddWithValue("$game_uuid", gameUuid.ToUpper());
                    cmd.Parameters.AddWithValue("$input", string.Join("", input));
                    cmd.Parameters.AddWithValue("$m", m);
                    cmd.Parameters.AddWithValue("$p", p);

                    return cmd.ExecuteNonQuery();
                }            
            });

            _ = _database.Execute(connection => {
                using (var cmd = new SqliteCommand("update games set guesses_count = guesses_count + 1 where uuid = $uuid;", connection))
                {
                    cmd.Parameters.AddWithValue("$uuid", gameUuid.ToUpper());

                    return cmd.ExecuteNonQuery();
                }
            });            

            if (secret.SequenceEqual(input))
            {
                _ = _database.Execute(connection => {
                    using (var cmd = new SqliteCommand("update games set secret_guessed = 1 where uuid = $uuid;", connection))
                    {
                        cmd.Parameters.AddWithValue("$uuid", gameUuid.ToUpper());

                        return cmd.ExecuteNonQuery();
                    }
                });
            }            
        }

        private static string GetSecretNumber(int count)
        {
            var randomNumbers = new HashSet<int>();

            for (int i = 0; i < count; i++)
                while (!randomNumbers.Add(new Random().Next(0, 9)));

            return string.Join("", randomNumbers);
        }

        public bool InputIsInRenge(int[] input)
        {
            int number = input.Select((t, i) => t * Convert.ToInt32(Math.Pow(10, input.Length - i - 1))).Sum();

            return number >= 0 && number <= 9999;
        }

        private IEnumerable<Guess> GetLogs(string gameUuid)
        {
            var result = _database.Execute(connection => {                
                var result = new List<Guess>();

                using (var cmd = new SqliteCommand("select * from guesses where game_uuid = $game_uuid order by 1 desc", connection))
                {
                    cmd.Parameters.AddWithValue("$game_uuid", gameUuid.ToUpper());

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var guess = new Guess
                            {
                                Input = (string)reader["input"],
                                MatchingDigits = Convert.ToInt32(reader["m"]),
                                MatchingDigitsPlaces = Convert.ToInt32(reader["p"])
                            };

                            result.Add(guess);
                        }
                    }
                }

                return result;                
            });

            return result;
        }

        public Game GetGame(string uuid)
        {
            var game = _database.Execute(connection => {
                var result = new List<Guess>();

                using (var cmd = new SqliteCommand("select * from games where uuid = $uuid;", connection))
                {
                    cmd.Parameters.AddWithValue("$uuid", uuid.ToUpper());

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Game()
                            {
                                Uuid = (string)reader["uuid"],
                                GuessesCount = Convert.ToInt32(reader["guesses_count"]),
                                Secret = reader["secret"].ToString().Select(x => int.Parse(x.ToString())).ToArray(),
                                Logs = GetLogs(uuid),
                                SecretGuessed = Convert.ToInt32(reader["secret_guessed"]) == 1
                            };
                        }
                    }
                }

                return null;
            });

            return game;
        }

        public bool RegisterPlayer(Player player)
        {
            var insert = _database.Execute(connection => {

                using var cmd = new SqliteCommand(
                    @"insert into players (id, name) 
                    values($id, $name) on conflict(id) do update set name = $name", connection);
                cmd.Parameters.AddWithValue("id", player.Id);
                cmd.Parameters.AddWithValue("name", player.Name);                

                return cmd.ExecuteNonQuery();
            });

            return insert > 0;
        }

        public void RegisterStats(Game game)
        {
            var user_id = _database.Execute(connection => {
                using (var cmd = new SqliteCommand("select user_id from games where uuid = $uuid;", connection))
                {
                    cmd.Parameters.AddWithValue("$uuid", game.Uuid.ToUpper());

                    return cmd.ExecuteScalar().ToString();
                }
            });

            if (!string.IsNullOrEmpty(user_id))
            {
                _ = _database.Execute(connection => {
                    using (var cmd = new SqliteCommand(@"update players 
                        set games_played = games_played + 1, 
                            correct_guesses = correct_guesses + $correct_guesses,
                            total_guesses = total_guesses + $guesses_count
                        where id = id;", connection))
                    {
                        cmd.Parameters.AddWithValue("$id", user_id);
                        cmd.Parameters.AddWithValue("$correct_guesses", game.SecretGuessed ? 1 : 0);
                        cmd.Parameters.AddWithValue("$guesses_count", game.GuessesCount);

                        return cmd.ExecuteNonQuery();
                    }
                });
            }
        }

        public IEnumerable<Player> GetStats()
        {
            return GetStats(0);
        }

        public IEnumerable<Player> GetStats(int gameCount)
        {
            var result = _database.Execute(connection => {
                var result = new List<Player>();

                var sql = @"select cast(correct_guesses as float) / nullif(games_played, 0) as rank, name, total_guesses, games_played from players where games_played > 0";
                if (gameCount > 0)
                {
                    sql += " and games_played = $count";
                }
                sql += " order by rank, total_guesses";

                using (var cmd = new SqliteCommand(sql, connection))
                {
                    if (gameCount > 0)
                    {
                        cmd.Parameters.AddWithValue("$count", gameCount);
                    }                    

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var guess = new Player
                            {
                                Rank = Convert.ToDecimal(reader["rank"]),
                                Name = (string)reader["name"],
                                TotalGuesses = Convert.ToInt32(reader["total_guesses"]),
                                GamesPlayed = Convert.ToInt32(reader["games_played"])
                            };

                            result.Add(guess);
                        }
                    }
                }

                return result;
            });

            return result;
        }
    }
}
