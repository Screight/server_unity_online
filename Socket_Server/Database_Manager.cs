using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Socket_Server
{
    class Database_Manager
    {
        public static Database_Manager _DATABASE_MANAGER;

        const string connectionString = "Server=db4free.net;Port=3306;database=unity_online_db;Uid=screight;password=qwerty123;SSL Mode=None;connect timeout=3600;default command timeout = 3600;";

        const string PLAYER_TABLE_NAME = "Players";
        const string PLAYER_NICKNAME_FIELD_NAME = "nickname";
        const string PLAYER_PASSWORD_FIELD_NAME = "passw0rd";
        const string PLAYER_ID_RACE_FIELD_NAME = "id_race";

        const string RACES_TABLE_NAME = "Races";
        const string RACES_ID_FIELD_NAME = "id_races";
        const string RACES_NAME_FIELD_NAME = "name";
        const string RACES_HEALTH_FIELD_NAME = "health";
        const string RACES_DAMAGE_FIELD_NAME = "damage";
        const string RACES_SPEED_FIELD_NAME = "speed";
        const string RACES_JUMP_FORCE_FIELD_NAME = "jump_force";
        const string RACES_FIRE_RATE_FIELD_NAME = "fire_rate";

        MySqlConnection connection;

        public Database_Manager()
        {
            connection = new MySqlConnection(connectionString);
        }

        public void OpenConnection()
        {
            //Try to open connection
            try
            {
                connection.Open();
                if (_DATABASE_MANAGER == null) { _DATABASE_MANAGER = this; }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        public void CloseConnection()
        {
            //Close connection
            connection.Close();
        }

        public string TryRegisteringUser(string p_name, string p_pass, string p_raceID)
        {
            //Creo el reader para leer los datos
            MySqlDataReader reader;

            //Creo la instruccion que quiero ejecutar de SQL (clase)
            MySqlCommand command = connection.CreateCommand();

            //Aado en el atributo de la clase la query a realizar
            command.CommandText = "Select * FROM " + PLAYER_TABLE_NAME + " WHERE " + PLAYER_NICKNAME_FIELD_NAME + " = '" + p_name + "'";

            int numberOfUsers = 0;
            try
            {
                //Creo el reader para leer los datos
                reader = command.ExecuteReader();

                //Mientras haya datos voy leyendo
                while (reader.Read())
                {
                    numberOfUsers++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            reader.Close();
            if (numberOfUsers > 0) { return null; }

            string query = "INSERT INTO Players(nickname, passw0rd, id_race) VALUES(\"";
            query += p_name;
            query += "\", \"";
            query += p_pass;
            query += "\",";
            query += p_raceID.ToString();
            query += ");";

            command = connection.CreateCommand();
            command.CommandText = query;
            reader = command.ExecuteReader();

            reader.Close();

            return p_name;
        }

        public User? TryGettingUser(string p_name, string p_pass)
        {
            //Creo el reader para leer los datos
            MySqlDataReader reader = null;

            //Creo la instruccion que quiero ejecutar de SQL (clase)
            MySqlCommand command = connection.CreateCommand();

            //Aado en el atributo de la clase la query a realizar
            command.CommandText = "Select * FROM " + PLAYER_TABLE_NAME + " WHERE " + PLAYER_NICKNAME_FIELD_NAME + " = '" + p_name + "'";

            try
            {
                //Creo el reader para leer los datos
                reader = command.ExecuteReader();

                //Mientras haya datos voy leyendo
                while (reader.Read())
                {
                    string value = reader[PLAYER_PASSWORD_FIELD_NAME].ToString();
                    Console.WriteLine(p_pass.ToArray());
                    Console.WriteLine(value.ToArray());

                    char[] valueArray = value.ToArray();
                    char[] resultArray;
                    if (valueArray[valueArray.Length - 1] == 8203)
                    {
                        resultArray = new char[valueArray.Length - 1];
                        for (int i = 0; i < resultArray.Length; i++)
                        {
                            resultArray[i] = valueArray[i];
                        }
                    }
                    else { resultArray = valueArray; }

                    Console.WriteLine(p_pass.ToArray());

                    bool areEqual = true;

                    for(int i = 0; i < resultArray.Length && areEqual; i++)
                    { if (resultArray[i] != p_pass.ToArray()[i]) { areEqual = false; } }

                    if(areEqual) {
                        
                        User user = new User();
                        //Almaceno el dato
                        user.Name = reader[PLAYER_NICKNAME_FIELD_NAME].ToString();
                        user.RaceID = System.Int32.Parse(reader[PLAYER_ID_RACE_FIELD_NAME].ToString());
                        reader.Close();
                        return user;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                if(reader != null) { reader.Close(); }
                Console.WriteLine(ex.Message);
                return null;
            }
            return null;
        }

        public List<Race> GetClasses()
        {
            List<Race> raceList = new List<Race>();
            MySqlDataReader reader = null;

            //Creo la instruccion que quiero ejecutar de SQL (clase)
            MySqlCommand command = connection.CreateCommand();

            //Aado en el atributo de la clase la query a realizar
            command.CommandText = "Select * FROM " + RACES_TABLE_NAME + ";";

            try
            {
                //Creo el reader para leer los datos
                reader = command.ExecuteReader();

                //Mientras haya datos voy leyendo
                while (reader.Read())
                {
                    Race race = new Race();
                    //Almaceno el dato
                    race.Name = reader[RACES_NAME_FIELD_NAME].ToString();
                    race.ID = System.Int32.Parse(reader[RACES_ID_FIELD_NAME].ToString());
                    race.MaxHealth = System.Int32.Parse(reader[RACES_HEALTH_FIELD_NAME].ToString());
                    race.Damage = System.Int32.Parse(reader[RACES_DAMAGE_FIELD_NAME].ToString());
                    race.Speed = System.Int32.Parse(reader[RACES_SPEED_FIELD_NAME].ToString());
                    race.JumpForce = System.Int32.Parse(reader[RACES_JUMP_FORCE_FIELD_NAME].ToString());
                    race.FireRate = System.Int32.Parse(reader[RACES_FIRE_RATE_FIELD_NAME].ToString());

                    raceList.Add(race);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (reader != null) { reader.Close(); }
            return raceList;
        }

        //Funcion para eliminar datos "Hardcoded"
        void DeleteExample(MySqlConnection connection)
        {
            //Creo la instruccion que quiero ejecutar de SQL (clase)
            MySqlCommand command = connection.CreateCommand();

            //AÃ±ado en el atributo de la clase la query a realizar
            command.CommandText = "DELETE FROM Player Where Player.Id = 5;";

            try
            {
                //Ejecuto la instrucciÃ³n, el NonQuery() evita que el programa se interrumpa y espere respuesta de SQL
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        //Funcion optimizada de select, recibe por parametro la query de la consulta y que campo debe devolver
        List<string> OptimizedStringSelectExample(string commandString, string returnedRow, MySqlConnection connection)
        {
            MySqlDataReader reader;

            //Creo la instruccion que quiero ejecutar de SQL (clase)
            MySqlCommand command = connection.CreateCommand();

            //AÃ±ado en el atributo de la clase la query a realizar
            command.CommandText = commandString;

            //Variable para almacenar temporalmente los datos
            List<string> tmp = new List<string>();

            try
            {
                //Creo el reader para leer los datos
                reader = command.ExecuteReader();

                //Mientras haya datos voy leyendo
                while (reader.Read())
                {
                    //Almaceno el dato
                    tmp.Add(reader[returnedRow].ToString());
                }

                //Devuelvo los datos
                reader.Close();
                return tmp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return tmp;
            }
        }

        //Funcion de ejecutar consultas genericas asincronas, es decir, que no necesito esperar respuesta de la base de datos
        void OptimizedExecuteCommandExample(string commandString, MySqlConnection connection)
        {
            //Creo la instruccion que quiero ejecutar de SQL (clase)
            MySqlCommand command = connection.CreateCommand();

            //Almaceno la query recibida por parametro.
            command.CommandText = commandString;

            try
            {
                //Ejecuto la instrucciÃ³n, el NonQuery() evita que el programa se interrumpa y espere respuesta de SQL
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}