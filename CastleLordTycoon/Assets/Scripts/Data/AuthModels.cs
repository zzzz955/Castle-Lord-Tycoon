using System;

namespace CastleLordTycoon.Data
{
    /// <summary>
    /// 회원가입 요청
    /// </summary>
    [Serializable]
    public class RegisterRequest
    {
        public string username;
        public string password;
        public string email;
    }

    /// <summary>
    /// 회원가입 응답
    /// </summary>
    [Serializable]
    public class RegisterResponse
    {
        public string userId;
        public string username;
        public string createdAt;
    }

    /// <summary>
    /// 로그인 요청
    /// </summary>
    [Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }

    /// <summary>
    /// 로그인 응답
    /// </summary>
    [Serializable]
    public class LoginResponse
    {
        public string accessToken;
        public string refreshToken;
        public int expiresIn;
        public string userId;
        public string username;
    }

    /// <summary>
    /// 토큰 갱신 요청
    /// </summary>
    [Serializable]
    public class RefreshTokenRequest
    {
        public string refreshToken;
    }

    /// <summary>
    /// 토큰 갱신 응답
    /// </summary>
    [Serializable]
    public class RefreshTokenResponse
    {
        public string accessToken;
        public int expiresIn;
    }
}
