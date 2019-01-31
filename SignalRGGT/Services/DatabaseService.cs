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
                String query = String.Format($"SELECT USER_NAME FROM TB_USERINFO WHERE USER_ID = @UserID AND USER_PASSWORD = @UserPassword");
                OpenAsync();
                using (SqlCommand command = new SqlCommand(query, Connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserID", id));
                    command.Parameters.Add(new SqlParameter("@UserPassword", pw));
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            result = dataReader["USER_NAME"].ToString();
                        }
                    }
                }
                Close();
                return result;
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // 로그인 여부 확인
        public String GetUserStatus(String id, String pw)
        {
            String result = String.Empty;
            try
            {
                String query = String.Format($"SELECT USER_STATUS FROM TB_USERINFO WHERE USER_ID = @UserID AND USER_PASSWORD = @UserPassword");
                OpenAsync();
                using (SqlCommand command = new SqlCommand(query, Connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserID", id));
                    command.Parameters.Add(new SqlParameter("@UserPassword", pw));
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            result = dataReader["USER_STATUS"].ToString();
                        }
                    }
                }
                Close();
                return result;
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion
    }
}