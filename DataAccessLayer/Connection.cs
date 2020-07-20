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
