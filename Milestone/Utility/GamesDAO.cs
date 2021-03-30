using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

/**
 * Kacey Morris
 * Alex Vergara
 * March 28 2021
 * CST 247
 * Milestone 4 - Save, Load, and API
 * GamesDAO.cs
 * 
 * This class interacts with the database for game manipulation. 
 * 
 * This is our own work, as influenced by class time and video tutorials for previous projects.
 */

namespace Milestone.Utility
{
    public class GamesDAO
    {
        public String connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Minesweeper;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public bool saveGame(GameObject game)
        {
            bool success = false;

            String queryString = "INSERT INTO dbo.games (gameString, userID, datePlayed, level) VALUES (@gameString, @userID, @datePlayed, @level)";

            using(SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using(SqlCommand sqlCommand = new SqlCommand(queryString, sqlConnection))
                {
                    sqlCommand.Parameters.Add("@gameString", SqlDbType.Text).Value = game.JsonString;
                    sqlCommand.Parameters.Add("@userID", SqlDbType.Int, 100).Value = game.userID;
                    sqlCommand.Parameters.Add("@datePlayed", SqlDbType.DateTime).Value = game.datePlayed;
                    sqlCommand.Parameters.Add("@level", SqlDbType.NVarChar, 50).Value = game.level;

                    try
                    {
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                        sqlConnection.Close();
                        success = true;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Failure");
                        Debug.WriteLine(e.Message);
                    }
                
                }
            }
            return success;

        }

        /*
        public GameObject loadGame()
        {
            GameObject gameObject = new GameObject(1, "", 1, DateTime.Now, "easy");

            // gets the latest game saved
            // refactor to get a specific game
            String queryString = "SELECT TOP 1 * FROM dbo.games ORDER BY ID DESC";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(queryString, sqlConnection))
                {
                    try
                    {
                        sqlConnection.Open();
                        SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                        while (sqlDataReader.Read())
                        {
                            gameObject.id = sqlDataReader.GetInt32(0);
                            gameObject.JsonString = sqlDataReader.GetString(1);
                        }
                        sqlConnection.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failure");
                        Debug.WriteLine(e.Message);
                    }

                }
            }
            return gameObject;

        }
        */

        public List<GameObject> getAllGames()
        {
            List<GameObject> games = new List<GameObject>();

            // select all games from the database
            String queryString = "SELECT * FROM dbo.games";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(queryString, sqlConnection))
                {
                    try
                    {
                        // open connection
                        sqlConnection.Open();
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        // if results were returned
                        if (reader.HasRows)
                        {
                            // read through the results
                            while (reader.Read())
                            {
                                // create a new game from the information from the database
                                GameObject game = new GameObject((int)reader.GetValue(0), (string)reader.GetValue(1), (int)reader.GetValue(2), (DateTime)reader.GetValue(3), (string)reader.GetValue(4));
                                // add the game to the list
                                games.Add(game);
                            }
                        }
                        // close connection
                        sqlConnection.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failure");
                        Debug.WriteLine(e.Message);
                    }

                }
            }
            // return the list of games
            return games;
        }

        public List<GameObject> getGameByUser(int userID)
        {
            List<GameObject> games = new List<GameObject>();

            // select all games from the database
            String queryString = "SELECT * FROM dbo.games WHERE userID = @userID";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(queryString, sqlConnection))
                {
                    // bind parameters
                    sqlCommand.Parameters.Add("@userID", SqlDbType.Int, 100).Value = userID;
                    try
                    {
                        // open connection
                        sqlConnection.Open();
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        // if results were returned
                        if (reader.HasRows)
                        {
                            // read through the results
                            while (reader.Read())
                            {
                                // create a new game from the information from the database
                                GameObject game = new GameObject((int)reader.GetValue(0), (string)reader.GetValue(1), (int)reader.GetValue(2), (DateTime)reader.GetValue(3), (string)reader.GetValue(4));
                                // add the game to the list
                                games.Add(game);
                            }
                        }
                        // close connection
                        sqlConnection.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failure");
                        Debug.WriteLine(e.Message);
                    }

                }
            }
            // return the list of games
            return games;
        }

        public GameObject loadGameByID(int gameID)
        {
            GameObject game = new GameObject();
            // select all games from the database
            String queryString = "SELECT * FROM dbo.games WHERE Id = @gameID";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(queryString, sqlConnection))
                {
                    // bind parameters
                    sqlCommand.Parameters.Add("@gameID", SqlDbType.Int, 100).Value = gameID;
                    try
                    {
                        // open connection
                        sqlConnection.Open();
                        SqlDataReader reader = sqlCommand.ExecuteReader();
                        // if results were returned
                        if (reader.HasRows)
                        {
                            // read through the results
                            while (reader.Read())
                            {
                                // create a new game from the information from the database
                                game = new GameObject((int)reader.GetValue(0), (string)reader.GetValue(1), (int)reader.GetValue(2), (DateTime)reader.GetValue(3), (string)reader.GetValue(4));
                            }
                        }
                        // close connection
                        sqlConnection.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failure");
                        Debug.WriteLine(e.Message);
                    }

                }
            }
            // return the list of games
            return game;
        }

        public bool deleteGameByID(int gameID)
        {
            bool success = false;
            // select all games from the database
            String queryString = "DELETE FROM dbo.games WHERE Id = @gameID";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(queryString, sqlConnection))
                {
                    // bind parameters
                    sqlCommand.Parameters.Add("@gameID", SqlDbType.Int, 100).Value = gameID;
                    try
                    {
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                        sqlConnection.Close();
                        success = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failure");
                        Debug.WriteLine(e.Message);
                    }

                }
            }
            // return the list of games
            return success;
        }
    }
}
