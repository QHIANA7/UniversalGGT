using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRGGT.Models
{
    /// <summary>
    /// ID중복검사에 대한 응답코드
    /// </summary>
    public enum Packet0001ReturnState
    {
        /// <summary>
        /// 정상
        /// </summary>
        Success,
        /// <summary>
        /// 이미 사용중인 ID
        /// </summary>
        AlreadyUsed,
        /// <summary>
        /// 유효하지 않는 ID
        /// </summary>
        InvalidID,
    }

    /// <summary>
    /// 회원가입에 대한 응답코드
    /// </summary>
    public enum Packet0002ReturnState
    {
        /// <summary>
        /// 정상
        /// </summary>
        Success,
        /// <summary>
        /// 유효하지 않는 Password
        /// </summary>
        InvalidPassword,
        /// <summary>
        /// 유효하지 않는 UserName
        /// </summary>
        InvalidUserName,
        /// <summary>
        /// 유효하지 않는 Password와 UserName
        /// </summary>
        InvalidUPasswordAndUserName
    }

    /// <summary>
    /// 로그인에 대한 응답코드
    /// </summary>
    public enum Packet0003ReturnState
    {
        /// <summary>
        /// 정상
        /// </summary>
        Success,
        /// <summary>
        /// 이미 접속중인 ID
        /// </summary>
        AlreadyLogin
    }

    /// <summary>
    /// 로그아웃에 대한 응답코드
    /// </summary>
    public enum Packet0004ReturnState
    {
        /// <summary>
        /// 정상
        /// </summary>
        Success,
        /// <summary>
        /// 로그인 되지 않은 ID
        /// </summary>
        NotLogin
    }

    /// <summary>
    /// 현재 위치에 대한 열거형
    /// </summary>
    public enum CurrentLocation
    {
        /// <summary>
        /// 지정되지 않음
        /// </summary>
        None,
        /// <summary>
        /// 초기화면
        /// </summary>
        Init,
        /// <summary>
        /// 대기방
        /// </summary>
        WaitingRoom,
        /// <summary>
        /// 플레이방
        /// </summary>
        PlayingRoom
    }

    /// <summary>
    /// 사용자 상태에 대한 열거형
    /// </summary>
    public enum UserState
    {

    }
}