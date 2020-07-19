using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DataAccess<T,T2> : IDataAccess<T,T2>
    {
        public List<T> GetAllData(string StoredProcedureParams)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.ConnectionString))
            {
                //var output = connection.Query<Person>($"select * from People where LastName = '{ lastName }'").ToList();
                var output = connection.Query<T>(StoredProcedureParams).ToList();
                return output;
            }
        }


        public async Task InsertData(T obj, string StoredProcedureParams)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.ConnectionString))
            {
                //Person newPerson = new Person { FirstName = firstName, LastName = lastName, EmailAddress = emailAddress, PhoneNumber = phoneNumber };
                //List<Person> people = new List<Person>();

                //people.Add(new Person { FirstName = firstName, LastName = lastName, EmailAddress = emailAddress, PhoneNumber = phoneNumber });

                await connection.ExecuteAsync(StoredProcedureParams, obj);

            }
        }

        public async Task UpdateData(T obj, string StoredProcedureParams)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.ConnectionString))
            {
                await connection.ExecuteAsync(StoredProcedureParams, obj);
            }
        }


        public async Task DeleteData(T obj, string StoredProcedureParams)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.ConnectionString))
            {
                await connection.ExecuteAsync(StoredProcedureParams, obj);
            }
        }




        public async Task CheckOutBook(T obj, string StoredProcedureParams)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.ConnectionString))
            {
                await connection.ExecuteAsync(StoredProcedureParams, obj);
            }
        }

        public List<T> GetAllOrderData(string StoredProcedureParams)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.ConnectionString))
            {
                var output = connection.Query<T>(StoredProcedureParams).ToList();
                return output;
            }
        }


        public List<T2> GetAllData(string StoredProcedureParams, T obj)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.ConnectionString))
            {
                //var output = connection.Query<Person>($"select * from People where LastName = '{ lastName }'").ToList();
                var output = connection.Query<T2>(StoredProcedureParams, obj).ToList();
                return output;
            }
        }
    }
}
