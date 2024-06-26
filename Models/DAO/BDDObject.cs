using System.Dynamic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Linq.Expressions;

namespace Evaluation.Models
{
    public class BDDObject
    {
        public virtual string TableName()
        {
            return "";
        }

        public string UpperedName(string input)
        {
            char firstChar = input[0];
            string restOfString = input.Substring(1);
            return char.ToUpper(firstChar) + restOfString;
        }

        private String ScriptInsertColumns(List<string> columnNames, string automatic)
        {
            String ans = "";
            for (int i = 0; i < columnNames.Count; i++)
            {
                if (String.Compare(columnNames[i], automatic, StringComparison.OrdinalIgnoreCase) == 0) { }
                else
                {
                    ans += columnNames[i];
                    if (i != columnNames.Count - 1)
                    {
                        ans += ",";
                    }
                }
            }
            return ans;
        }

        private String ScriptUpdateColumns(string[] properties)
        {
            String ans = "";
            foreach (string col in properties)
            {
                // Invoke the method and return the result
                PropertyInfo prop = GetProperty(col);
                ans += $"{col} = {ToValueForInsert(prop.GetGetMethod().Invoke(this, null))}, ";
            }
            return ans.Substring(0, ans.Length - 2);
        }

        public virtual List<string> GetTableColumns(DbConnection connection)
        {
            List<string> columnNames = new List<string>();
            DataTable schema = connection.GetSchema("Columns", new string[] { null, null, TableName().Replace("\"", "") });
            foreach (DataRow row in schema.Rows)
            {
                columnNames.Add(row["COLUMN_NAME"].ToString());
            }
            return columnNames;
        }

        public PropertyInfo GetProperty(String argName)
        {
            Type type = this.GetType();
            //Console.WriteLine(argName);
            PropertyInfo propriete = type.GetProperty(UpperedName(argName), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return propriete;
        }

        public String ToValueForInsert(Object arg)
        {
            if (arg is null)
            {
                return "NULL";
            }
            String ans = "'" + arg.ToString().Replace("'", "''") + "'";
            if (arg is int || arg is double)
            {
                ans = arg.ToString().Replace(",", ".");
            }
            return ans;
        }

        public String ScriptInsertValues(DbConnection connection)
        {
            List<String> columnNames = this.GetTableColumns(connection);
            String ans = "";
            for (int i = 0; i < columnNames.Count; i++)
            {
                //Console.WriteLine(UpperedName(columnNames[i]));
                PropertyInfo prop = GetProperty(UpperedName(columnNames[i]));
                if (Attribute.GetCustomAttribute(prop, typeof(Identity)) == null)
                {
                    if (Attribute.GetCustomAttribute(prop, typeof(Default)) != null)
                    {
                        ans += "default";
                    }
                    else
                    {
                        if (prop.PropertyType == typeof(DateTime))
                        {
                            Console.WriteLine(((DateTime)prop.GetGetMethod().Invoke(this, null)).ToString("yyyy-MM-dd hh:mm:ss"));
                            ans += ToValueForInsert(((DateTime)prop.GetGetMethod().Invoke(this, null)).ToString("yyyy-MM-dd hh:mm:ss"));
                        }
                        else
                        {
                            ans += ToValueForInsert(prop.GetGetMethod().Invoke(this, null));
                        }
                    }
                    if (i != columnNames.Count - 1)
                    {
                        ans += ",";
                    }
                }
            }
            return ans;
        }

        public void Find(DbConnection con)
        {
            bool isNewConnexion = false;
            // Create a new connection if the provided connection is null
            if (con == null)
            {
                con = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                string query = "SELECT * FROM " + TableName() + " WHERE id = " + this.GetProperty("Id").GetGetMethod().Invoke(this, null);
                Console.WriteLine(query);
                using (var command = con.CreateCommand())//new NpgsqlCommand(query, con))
                {
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PropertyInfo prop = null;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string attributName = reader.GetName(i);
                                prop = GetProperty(UpperedName(attributName));
                                if (reader.IsDBNull(reader.GetOrdinal(attributName)) == false)
                                {
                                    prop.GetSetMethod().Invoke(this, new object[] { reader.GetValue(reader.GetOrdinal(attributName)) });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (isNewConnexion == true)
                    con.Close();
            }
        }

        public List<T> Find<T>(DbConnection connection, string filtre)
        {
            bool isNewConnexion = false;

            // Create a new connection if the provided connection is null
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            if (filtre == null)
            {
                filtre = "";
            }
            List<T> ans = new List<T>();
            string query = "SELECT * FROM " + TableName() + " " + filtre;
            Console.WriteLine(query);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T objectInstance = Activator.CreateInstance<T>();
                        PropertyInfo prop = null;
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string attributName = reader.GetName(i);
                            //prop = GetProperty(UpperedName(attributName));
                            Type type = typeof(T);
                            //Console.WriteLine(argName);
                            prop = type.GetProperty(UpperedName(attributName), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (reader.IsDBNull(reader.GetOrdinal(attributName)) == false)
                            {
                                prop.GetSetMethod().Invoke(objectInstance, new object[] { reader.GetValue(reader.GetOrdinal(attributName)) });
                            }
                        }
                        ans.Add(objectInstance);
                    }
                }
            }

            // Close the connection if it was created within this method
            if (isNewConnexion == true)
            {
                connection.Close();
            }

            return ans;
        }

        public int ExecuteNonQuery(DbConnection connection, DbTransaction transaction, string query)
        {
            bool isNewConnexion = false;
            // Create a new connection if the provided connection is null
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }
                    int rowsAffected = 0;
                    command.CommandText = query;
                    Console.WriteLine(query);
                    rowsAffected += command.ExecuteNonQuery();
                    return rowsAffected;
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                // Close the connection if it was created within this method
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        // public List<T> FromSql<T>(DbConnection connection, string query)
        // {
        //     bool isNewConnexion = false;

        //     // Create a new connection if the provided connection is null
        //     if (connection == null)
        //     {
        //         connection = new Connect().getConnectPostgres();
        //         isNewConnexion = true;
        //     }

        //     List<T> ans = new List<T>();
        //     Console.WriteLine(query);
        //     using (var command = connection.CreateCommand())
        //     {
        //         command.CommandText = query;
        //         command.Connection = connection;
        //         using (var reader = command.ExecuteReader())
        //         {
        //             while (reader.Read())
        //             {
        //                 T objectInstance = Activator.CreateInstance<T>();
        //                 PropertyInfo prop = null;
        //                 for (int i = 0; i < reader.FieldCount; i++)
        //                 {
        //                     string attributName = reader.GetName(i);
        //                     prop = GetProperty(UpperedName(attributName));
        //                     if (reader.IsDBNull(reader.GetOrdinal(attributName)) == false)
        //                     {
        //                         prop.GetSetMethod().Invoke(objectInstance, new object[] { reader.GetValue(reader.GetOrdinal(attributName)) });
        //                     }
        //                 }
        //                 ans.Add(objectInstance);
        //             }
        //         }
        //     }

        //     // Close the connection if it was created within this method
        //     if (isNewConnexion == true)
        //     {
        //         connection.Close();
        //     }

        //     return ans;
        // }

        public DataTable ExecuteQuery(DbConnection connection, string query)
        {
            bool isNewConnexion = false;
            // Create a new connection if the provided connection is null
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                Console.WriteLine(query);
                List<String> columnNames = GetTableColumns(connection);
                String key = columnNames[0];
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    DbDataReader reader = command.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    return dataTable;
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                // Close the connection if it was created within this method
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }


        public int Update(DbConnection connection, DbTransaction transaction, string[] properties)
        {
            bool isNewConnexion = false;
            // Create a new connection if the provided connection is null
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                List<String> columnNames = GetTableColumns(connection);
                String key = columnNames[0];
                using (DbCommand command = connection.CreateCommand())
                {
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }
                    int rowsAffected = 0;
                    string updateQuery = $"UPDATE {TableName()} SET {ScriptUpdateColumns(properties)} WHERE {key} = {GetProperty(UpperedName(key)).GetGetMethod().Invoke(this, null)}";
                    command.CommandText = updateQuery;
                    Console.WriteLine(updateQuery);
                    rowsAffected += command.ExecuteNonQuery();
                    return rowsAffected;
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                // Close the connection if it was created within this method
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }

        public PropertyInfo GetIdentityProperty()
        {
            Type type = this.GetType();
            PropertyInfo[] proprietes = type.GetProperties();
            foreach (var item in proprietes)
            {
                if (Attribute.GetCustomAttribute(item, typeof(Identity)) != null)
                {
                    return item;
                }
            }
            return null;
        }

        public void Insert(DbConnection connection, DbTransaction transaction)
        {
            bool isNewConnexion = false;
            if (connection == null)
            {
                connection = new Connect().getConnectPostgres();
                isNewConnexion = true;
            }
            try
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    if (transaction != null)
                    {
                        command.Transaction = transaction;
                    }
                    int rowsAffected = 0;
                    string identity = null;
                    PropertyInfo prop = this.GetIdentityProperty();
                    if (prop != null)
                    {
                        identity = prop.Name;
                    }
                    // Invoke the method and return the result
                    String columns = ScriptInsertColumns(GetTableColumns(connection), identity);
                    String values = ScriptInsertValues(connection);
                    string query = $"insert into {TableName()} ({columns}) values ({values}) RETURNING {identity}";
                    Console.WriteLine(query);
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Access the returned value, for example, the id column
                            int insertedId = reader.GetInt32(0);
                            prop.GetSetMethod().Invoke(this, new object[] { insertedId });
                        }
                    }
                }
            }
            finally
            {
                if (isNewConnexion == true)
                {
                    connection.Close();
                }
            }
        }
    }
}
