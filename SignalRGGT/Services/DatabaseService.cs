using SignalRGGT.Models;
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
            if (Connection.State == ConnectionState.Closed)
                await Connection.OpenAsync();
        }

        public void Close()
        {
            if (Connection.State != ConnectionState.Closed)
                Connection.Close();
        }

        #region TB_USERINFO에 대한 테이블 쿼리

        /// <summary>
        /// ID와 Password에 따른 사용자 이름을 조회
        /// </summary>
        /// <param name="id">사용자의 ID</param>
        /// <param name="pw">사용자의 Password</param>
        /// <returns></returns>
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

        /// <summary>
        /// ID에 따른 사용자 이름을 조회
        /// </summary>
        /// <param name="id">사용자의 ID</param>
        /// <returns></returns>
        public String GetUserName(String id)
        {
            String result = String.Empty;
            try
            {
                String query = String.Format($"SELECT USER_NAME FROM TB_USERINFO WHERE USER_ID = @UserID");
                OpenAsync();
                using (SqlCommand command = new SqlCommand(query, Connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserID", id));
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

        /// <summary>
        /// ID가 존재하는지 확인
        /// </summary>
        /// <param name="id">사용자의 ID</param>
        /// <returns></returns>
        public Boolean GetIdExist(String id)
        {
            Int32 EffectedRowCount = -1;
            try
            {
                String query = String.Format($"SELECT COUNT(*) FROM TB_USERINFO WHERE USER_ID = @UserID");
                OpenAsync();
                using (SqlCommand command = new SqlCommand(query, Connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserID", id));
                    EffectedRowCount = (Int32)command.ExecuteScalar();
                }
                Close();
                return EffectedRowCount == 0;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // 로그인 여부 확인
        public String GetUserStatus(String id)
        {
            String result = String.Empty;
            try
            {
                String query = String.Format($"SELECT USER_STATUS FROM TB_USERINFO WHERE USER_ID = @UserID");
                OpenAsync();
                using (SqlCommand command = new SqlCommand(query, Connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserID", id));
                    //command.Parameters.Add(new SqlParameter("@UserPassword", pw));
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

        public String GetUserCurrentLocation(String id, out Boolean is_exception)
        {
            String result = String.Empty;
            try
            {
                String query = String.Format($"SELECT CURRENT_LOCATION FROM TB_USERINFO WHERE USER_ID = @UserID");
                OpenAsync();
                using (SqlCommand command = new SqlCommand(query, Connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserID", id));
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            result = dataReader["CURRENT_LOCATION"].ToString();
                        }
                    }
                }
                Close();
                is_exception = false;
                return result;
            }
            catch (InvalidOperationException ex)
            {
                is_exception = true;
                return ex.Message;
            }
            catch (Exception ex)
            {
                is_exception = true;
                return ex.Message;
            }
        }

        public IEnumerable<UserInfo> GetUsersCurrentLocation(out Boolean is_exception)
        {
            List<UserInfo> list = new List<UserInfo>();
            try
            {
                String query = String.Format($"SELECT USER_NAME, CURRENT_LOCATION FROM TB_USERINFO WHERE USER_STATUS = 'X'");
                OpenAsync();
                using (SqlCommand command = new SqlCommand(query, Connection))
                {
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            CurrentLocation location = (CurrentLocation)Enum.Parse(typeof(CurrentLocation), dataReader["CURRENT_LOCATION"].ToString());
                            list.Add(new UserInfo() { UserName = dataReader["USER_NAME"].ToString(), Location = location });
                        }
                    }
                }
                Close();
                is_exception = false;
                return list;
            }
            catch (InvalidOperationException ex)
            {
                is_exception = true;
                return null;
            }
            catch (Exception ex)
            {
                is_exception = true;
                return null;
            }
        }

        /// <summary>
        /// 사용자를 추가합니다
        /// </summary>
        /// <param name="carNum">등록차량의 차량번호</param>
        /// <param name="owner">등록차량의 소유자</param>
        /// <param name="carType">등록차량의 차종</param>
        /// <returns>Insert문의 결과로 1개의 레코드가 영향을 받았을경우 true, 그렇지 않으면 false 입니다.</returns>
        public Boolean InsertUserInfo(String id, String pw, String name)
        {
            Int32 EffectedRowCount = 0;
            try
            {
                String query = String.Format($"INSERT INTO TB_USERINFO VALUES (@UserID, @UserPassword, @UserName, 'O', 'None', null)");
                OpenAsync();
                using (SqlCommand command = new SqlCommand(query, Connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserID", id));
                    command.Parameters.Add(new SqlParameter("@UserPassword", pw));
                    command.Parameters.Add(new SqlParameter("@UserName", name));

                    EffectedRowCount = command.ExecuteNonQuery(); //이 메소드는 영향을 미친 레코드의 수를 반환한다.
                }
                Close();

                if (EffectedRowCount == 1)
                    return true;
                else
                    return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 사용자의 연결ID를 갱신합니다.
        /// </summary>
        /// <param name="id">사용자의 ID</param>
        /// <param name="ConnectionID">사용자의 연결ID</param>
        /// <returns></returns>
        public Boolean UpdateConnectionID(String id, String connection_id)
        {
            Int32 EffectedRowCount = 0;
            try
            {
                String query = String.Format($"UPDATE TB_USERINFO SET CONNECTION_ID = @ConnectionID WHERE USER_ID = @UserID");
                OpenAsync();
                using (SqlCommand command = new SqlCommand(query, Connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserID", id));
                    command.Parameters.Add(new SqlParameter("@ConnectionID", connection_id));

                    EffectedRowCount = command.ExecuteNonQuery(); //이 메소드는 영향을 미친 레코드의 수를 반환한다.
                }
                Close();

                if (EffectedRowCount == 1)
                    return true;
                else
                    return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 사용자를 로그인 상태로 갱신합니다.
        /// </summary>
        /// <param name="id">사용자의 ID</param>
        /// <returns></returns>
        public Boolean UpdateUserLogin(String id)
        {
            Int32 EffectedRowCount = 0;
            try
            {
                String query = String.Format($"UPDATE TB_USERINFO SET USER_STATUS = 'X' WHERE USER_ID = @UserID");
                OpenAsync();
                using (SqlCommand command = new SqlCommand(query, Connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserID", id));
                    EffectedRowCount = command.ExecuteNonQuery(); //이 메소드는 영향을 미친 레코드의 수를 반환한다.
                }
                Close();

                if (EffectedRowCount == 1)
                    return true;
                else
                    return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 사용자를 로그아웃 상태로 갱신합니다.
        /// </summary>
        /// <param name="id">사용자의 ID</param>
        /// <returns></returns>
        public Boolean UpdateUserLogout(String id)
        {
            Int32 EffectedRowCount = 0;
            try
            {
                String query = String.Format($"UPDATE TB_USERINFO SET USER_STATUS = 'O' WHERE USER_ID = @UserID");
                OpenAsync();
                using (SqlCommand command = new SqlCommand(query, Connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserID", id));
                    EffectedRowCount = command.ExecuteNonQuery(); //이 메소드는 영향을 미친 레코드의 수를 반환한다.
                }
                Close();

                if (EffectedRowCount == 1)
                    return true;
                else
                    return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 사용자의 현재 위치를 갱신합니다.
        /// </summary>
        /// <param name="id">사용자의 ID</param>
        /// <returns></returns>
        public Boolean UpdateUserCurrentLocation(String id, String group_name)
        {
            Int32 EffectedRowCount = 0;
            try
            {
                String query = String.Format($"UPDATE TB_USERINFO SET CURRENT_LOCATION = @CurrentLocation WHERE USER_ID = @UserID");
                OpenAsync();
                using (SqlCommand command = new SqlCommand(query, Connection))
                {
                    command.Parameters.Add(new SqlParameter("@UserID", id));
                    command.Parameters.Add(new SqlParameter("@CurrentLocation", group_name));
                    EffectedRowCount = command.ExecuteNonQuery(); //이 메소드는 영향을 미친 레코드의 수를 반환한다.
                }
                Close();

                if (EffectedRowCount == 1)
                    return true;
                else
                    return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
