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
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
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
                }
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
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
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
                }
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
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.Add(new SqlParameter("@UserID", id));
                        EffectedRowCount = (Int32)command.ExecuteScalar();
                    }
                }
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
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
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
                }
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
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
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
                }
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

        /// <summary>
        /// 접속중인 사용자 리스트를 가져옵니다.
        /// </summary>
        /// <param name="is_exception">예외 발생여부</param>
        /// <returns></returns>
        public IEnumerable<UserInfo> GetUsersInfo(out Boolean is_exception)
        {
            List<UserInfo> list = new List<UserInfo>();
            try
            {
                String query = String.Format($"SELECT * FROM TB_USERINFO WHERE USER_STATUS = 'X'");
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                String extra_location = String.Empty;
                                CurrentLocation location = CurrentLocation.None;
                                if (Enum.TryParse<CurrentLocation>(dataReader["CURRENT_LOCATION"].ToString(), out location))
                                {



                                }
                                else
                                {
                                    if (dataReader["CURRENT_LOCATION"].ToString().Contains(CurrentLocation.PlayingRoom.ToString()))
                                    {
                                        location = CurrentLocation.PlayingRoom;
                                        extra_location = dataReader["CURRENT_LOCATION"].ToString().Replace(CurrentLocation.PlayingRoom.ToString(), String.Empty);
                                    }
                                }
                                list.Add(new UserInfo() { UserName = dataReader["USER_NAME"].ToString(), Location = location, ExtraLocation = extra_location });
                            }
                        }
                    }
                }
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
        /// 모든 방 리스트를 가져옵니다.
        /// </summary>
        /// <param name="is_exception">예외 발생 여부</param>
        /// <returns></returns>
        public IEnumerable<RoomInfo> GetRoomsInfo(out Boolean is_exception)
        {
            List<RoomInfo> list = new List<RoomInfo>();
            try
            {
                String query = String.Format($"SELECT * FROM TB_ROOMINFO");
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                list.Add(new RoomInfo() { RoomNumber = Convert.ToInt32(dataReader["ROOM_NO"].ToString()), RoomTitle = dataReader["ROOM_TITLE"].ToString(), IsPrivateAccess = Convert.ToBoolean(dataReader["PRIVATE_ACCESS_YN"].ToString()), AccessPassword = dataReader["ACCESS_PASSWORD"].ToString(), RoomMaster = dataReader["ROOM_MASTER"].ToString(), MaxJoinCount = Convert.ToInt32(dataReader["MAX_JOIN_CNT"].ToString()), CurrentJoinCount = Convert.ToInt32(dataReader["CURRENT_JOIN_CNT"].ToString()), IsPlaying = Convert.ToBoolean(dataReader["PLAYING_YN"].ToString()) });
                            }
                        }
                    }
                }
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
        /// 모든 방번호를 가져옵니다.
        /// </summary>
        /// <param name="is_exception">예외 발생 여부</param>
        /// <returns></returns>
        public IEnumerable<Int32> GetRoomNumbers(out Boolean is_exception)
        {
            List<Int32> list = new List<Int32>
            {
                0
            };
            try
            {
                String query = String.Format($"SELECT ROOM_NO FROM TB_ROOMINFO");
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                list.Add(Convert.ToInt32(dataReader["ROOM_NO"].ToString()));
                            }
                        }
                    }
                }
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
        /// <param name="id">사용자의 아이디</param>
        /// <param name="pw">사용자의 비밀번호</param>
        /// <param name="name">사용자의 이름</param>
        /// <returns>Insert문의 결과로 1개의 레코드가 영향을 받았을경우 true, 그렇지 않으면 false 입니다.</returns>
        public Boolean InsertUserInfo(String id, String pw, String name)
        {
            Int32 EffectedRowCount = 0;
            try
            {
                String query = String.Format($"INSERT INTO TB_USERINFO VALUES (@UserID, @UserPassword, @UserName, 'O', 'None', null)");
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.Add(new SqlParameter("@UserID", id));
                        command.Parameters.Add(new SqlParameter("@UserPassword", pw));
                        command.Parameters.Add(new SqlParameter("@UserName", name));

                        EffectedRowCount = command.ExecuteNonQuery(); //이 메소드는 영향을 미친 레코드의 수를 반환한다.
                    }
                }

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
        /// 방을 추가합니다
        /// </summary>
        /// <param name="room_no">방 번호</param>
        /// <param name="room_title">방 제목</param>
        /// <param name="is_private">비공개 여부</param>
        /// <param name="access_pw">입장 비밀번호</param>
        /// <param name="room_master">방장 이름</param>
        /// <param name="max_join_cnt">최대 입장 인원 수</param>
        /// <returns>Insert문의 결과로 1개의 레코드가 영향을 받았을경우 true, 그렇지 않으면 false 입니다.</returns>
        public Boolean InsertRoomInfo(Int32 room_no, String room_title, Boolean is_private, String access_pw, String room_master, Int32 max_join_cnt, out Boolean is_exception)
        {
            Int32 EffectedRowCount = 0;
            is_exception = true;
            try
            {
                String query = String.Format($"INSERT INTO TB_ROOMINFO VALUES (@RoomNumber, @RoomTitle, @IsPrivateAccess, @AccessPassword, @RoomMaster, @MaxJoinCount, 0, 0)");
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.Add(new SqlParameter("@RoomNumber", room_no));
                        command.Parameters.Add(new SqlParameter("@RoomTitle", room_title));
                        command.Parameters.Add(new SqlParameter("@IsPrivateAccess", is_private));
                        command.Parameters.Add(new SqlParameter("@AccessPassword", access_pw));
                        command.Parameters.Add(new SqlParameter("@RoomMaster", room_master));
                        command.Parameters.Add(new SqlParameter("@MaxJoinCount", max_join_cnt));

                        EffectedRowCount = command.ExecuteNonQuery(); //이 메소드는 영향을 미친 레코드의 수를 반환한다.
                    }
                }

                if (EffectedRowCount == 1)
                {
                    is_exception = false;
                    return true;
                }
                else
                    return false;
            }
            catch (InvalidOperationException)
            {
                is_exception = true;
                return false;
            }
            catch (Exception)
            {
                is_exception = true;
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
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.Add(new SqlParameter("@UserID", id));
                        command.Parameters.Add(new SqlParameter("@ConnectionID", connection_id));

                        EffectedRowCount = command.ExecuteNonQuery(); //이 메소드는 영향을 미친 레코드의 수를 반환한다.
                    }
                }

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
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.Add(new SqlParameter("@UserID", id));
                        EffectedRowCount = command.ExecuteNonQuery(); //이 메소드는 영향을 미친 레코드의 수를 반환한다.
                    }
                }

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
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.Add(new SqlParameter("@UserID", id));
                        EffectedRowCount = command.ExecuteNonQuery(); //이 메소드는 영향을 미친 레코드의 수를 반환한다.
                    }
                }

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
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.Add(new SqlParameter("@UserID", id));
                        command.Parameters.Add(new SqlParameter("@CurrentLocation", group_name));
                        EffectedRowCount = command.ExecuteNonQuery(); //이 메소드는 영향을 미친 레코드의 수를 반환한다.
                    }
                }

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

        public Boolean UpdateUserDisconnectMessage(String connection_id, String discconect_msg)
        {
            Int32 EffectedRowCount = 0;
            try
            {
                String query = String.Format($"UPDATE TB_USERINFO SET DISCONNECT_MESSAGE = @DisconnectMessage WHERE CONNECTION_ID = @ConnectionID");
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.Add(new SqlParameter("@DisconnectMessage", discconect_msg));
                        command.Parameters.Add(new SqlParameter("@ConnectionID", connection_id));

                        EffectedRowCount = command.ExecuteNonQuery(); //이 메소드는 영향을 미친 레코드의 수를 반환한다.
                    }
                }

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
