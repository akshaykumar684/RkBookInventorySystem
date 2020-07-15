using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class DataAccess<T>
    {
      
        public static List<T> GetAllData(string StoredProcedureParams)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.ConnectionString))
            {
                //var output = connection.Query<Person>($"select * from People where LastName = '{ lastName }'").ToList();
                var output = connection.Query<T>(StoredProcedureParams).ToList();
                return output;
            }
        }

        
        public async static Task InsertData(T obj, string StoredProcedureParams)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.ConnectionString))
            {
                //Person newPerson = new Person { FirstName = firstName, LastName = lastName, EmailAddress = emailAddress, PhoneNumber = phoneNumber };
                //List<Person> people = new List<Person>();

                //people.Add(new Person { FirstName = firstName, LastName = lastName, EmailAddress = emailAddress, PhoneNumber = phoneNumber });

                await connection.ExecuteAsync(StoredProcedureParams, obj);

            }
        }

        public async static Task UpdateData(T obj, string StoredProcedureParams)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.ConnectionString))
            {
                await connection.ExecuteAsync(StoredProcedureParams, obj);
            }
        }

        public async static Task DeleteData(T obj, string StoredProcedureParams)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.ConnectionString))
            {
                await connection.ExecuteAsync(StoredProcedureParams, obj);
            }
        }
    }


    public static class DataAccess<T,T2>
    {
        public static List<T> GetAllData(string StoredProcedureParams,T2 obj)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.ConnectionString))
            {
                //var output = connection.Query<Person>($"select * from People where LastName = '{ lastName }'").ToList();
                var output = connection.Query<T>(StoredProcedureParams,obj).ToList();
                return output;
            }
        }
    }
}
