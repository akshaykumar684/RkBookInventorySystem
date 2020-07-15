using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class CheckOutIn<T>
    {
        public async static Task CheckOutBook(T obj, string StoredProcedureParams)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.ConnectionString))
            {
                //Person newPerson = new Person { FirstName = firstName, LastName = lastName, EmailAddress = emailAddress, PhoneNumber = phoneNumber };
                //List<Person> people = new List<Person>();

                //people.Add(new Person { FirstName = firstName, LastName = lastName, EmailAddress = emailAddress, PhoneNumber = phoneNumber });

                await connection.ExecuteAsync(StoredProcedureParams, obj);

            }
        }

        public static List<T> GetAllOrderData(string StoredProcedureParams)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Connection.ConnectionString))
            {
                //var output = connection.Query<Person>($"select * from People where LastName = '{ lastName }'").ToList();
                var output = connection.Query<T>(StoredProcedureParams).ToList();
                return output;
            }
        }
    }
}
