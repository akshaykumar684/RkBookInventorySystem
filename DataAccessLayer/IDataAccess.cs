using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public interface IDataAccess<T,T2>
    {
        List<T> GetAllData(string StoredProcedureParams);

        List<T2> GetAllData(string StoredProcedureParams, T obj);

        Task InsertData(T obj, string StoredProcedureParams);


        Task UpdateData(T obj, string StoredProcedureParams);


        Task DeleteData(T obj, string StoredProcedureParams);


        Task CheckOutBook(T obj, string StoredProcedureParams);


        List<T> GetAllOrderData(string StoredProcedureParams);

    }
}
