using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public static class Connection
    {
        public static  string ConnectionString { get; set; }

        public static void InitiaizeDataAccessLayer(string Conn)
        {
            ConnectionString = Conn;

        }

    }
}
