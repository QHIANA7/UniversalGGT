using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SignalRGGT.Services
{
    public class DatabaseService
    {
        private SqlConnection Connection { get; set; } = null;

        public String ConnectionString { get => Connection?.ConnectionString; set => Connection = new SqlConnection(value); }

        public Boolean IsOpen { get => Connection?.State == ConnectionState.Open; }

        public async void OpenAsync()
        {
           await Connection.OpenAsync();
        }

        public void Close()
        {
            Connection.Close();
        }

    }
}