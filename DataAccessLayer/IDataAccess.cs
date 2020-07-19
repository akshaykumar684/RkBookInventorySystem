using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public interface IDataAccess<T>
    {
        List<T> GetAllData(string StoredProcedureParams);



        Task InsertData(T obj, string StoredProcedureParams);


        Task UpdateData(T obj, string StoredProcedureParams);


        Task DeleteData(T obj, string StoredProcedureParams);


        Task CheckOutBook(T obj, string StoredProcedureParams);


        List<T> GetAllOrderData(string StoredProcedureParams);

    }
}
