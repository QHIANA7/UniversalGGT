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
        public SqlConnection Connection { get; private set; } = null;

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

        #region TB_USERINFO에 대한 테이블 쿼리

        public String GetUserName(String id, String pw)
        {
            String result = String.Empty;
            try
            {
                String query = String.Format($"SELECT * FROM TB_USERINFO WHERE USER_ID = {id} AND USER_PASSWORD = {pw}");
                using (SqlCommand command = new SqlCommand(query, Connection))
                {
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            result = dataReader["USER_NAME"].ToString();
                        }
                    }
                }
                return result;
            }
            catch (InvalidOperationException ex)
            {
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        #endregion
    }
}