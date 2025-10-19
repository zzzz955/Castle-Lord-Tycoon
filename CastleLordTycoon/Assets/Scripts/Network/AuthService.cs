using System;
using UnityEngine;
using CastleLordTycoon.Data;
using CastleLordTycoon.Core;

namespace CastleLordTycoon.Network
{
    /// <summary>
    /// 인증 서비스 (로그인, 회원가입, 토큰 관리)
    /// SecureStorage 기반 암호화 저장
    /// </summary>
    public class AuthService : MonoBehaviour
    {
        private static AuthService _instance;
        public static AuthService Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("AuthService");
                    _instance = go.AddComponent<AuthService>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        // 이벤트
        public event Action<string> OnLoginSuccess;
        public event Action<string> OnLoginFailed;
        public event Action OnLogout;

        // 메모리 내 사용자 정보 (세션)
        private string _currentUserId;
        private string _currentUsername;

        public bool IsLoggedIn => !string.IsNullOrEmpty(_currentUserId);
        public string CurrentUserId => _currentUserId;
        public string CurrentUsername => _currentUsername;

        private void Awake()
        {
            // SecureStorage 초기화
            SecureStorage.Initialize();
        }

        /// <summary>
        /// 회원가입
        /// </summary>
        public void Register(string username, string password, string email, Action<bool, string> callback)
        {
            var request = new RegisterRequest
            {
                username = username,
                password = password,
                email = email
            };

            HttpClient.Instance.Post<RegisterRequest, RegisterResponse>(
                ApiConfig.Auth.REGISTER,
                request,
                response =>
                {
                    if (response.success)
                    {
                        Debug.Log($"회원가입 성공: {response.data.username}");
                        callback?.Invoke(true, "회원가입 성공");
                    }
                    else
                    {
                        Debug.LogWarning($"회원가입 실패: {response.error.message}");
                        callback?.Invoke(false, response.error.message);
                    }
                },
                error =>
                {
                    Debug.LogError($"회원가입 요청 실패: {error}");
                    callback?.Invoke(false, error);
                }
            );
        }

        /// <summary>
        /// 로그인
        /// </summary>
        public void Login(string username, string password, Action<bool, string> callback)
        {
            var request = new LoginRequest
            {
                username = username,
                password = password
            };

            HttpClient.Instance.Post<LoginRequest, LoginResponse>(
                ApiConfig.Auth.LOGIN,
                request,
                response =>
                {
                    if (response.success)
                    {
                        SaveAuthData(response.data);
                        callback?.Invoke(true, "로그인 성공");
                    }
                    else
                    {
                        Debug.LogWarning($"로그인 실패: {response.error.message}");
                        OnLoginFailed?.Invoke(response.error.message);
                        callback?.Invoke(false, response.error.message);
                    }
                },
                error =>
                {
                    Debug.LogError($"로그인 요청 실패: {error}");
                    OnLoginFailed?.Invoke(error);
                    callback?.Invoke(false, error);
                }
            );
        }

        /// <summary>
        /// Google 로그인
        /// </summary>
        public void LoginWithGoogle(string idToken, Action<bool, string> callback)
        {
            var request = new GoogleLoginRequest
            {
                idToken = idToken
            };

            HttpClient.Instance.Post<GoogleLoginRequest, LoginResponse>(
                ApiConfig.Auth.GOOGLE_LOGIN,
                request,
                response =>
                {
                    if (response.success)
                    {
                        SaveAuthData(response.data);
                        callback?.Invoke(true, "Google 로그인 성공");
                    }
                    else
                    {
                        Debug.LogWarning($"Google 로그인 실패: {response.error.message}");
                        OnLoginFailed?.Invoke(response.error.message);
                        callback?.Invoke(false, response.error.message);
                    }
                },
                error =>
                {
                    Debug.LogError($"Google 로그인 요청 실패: {error}");
                    OnLoginFailed?.Invoke(error);
                    callback?.Invoke(false, error);
                }
            );
        }

        /// <summary>
        /// 로그아웃
        /// </summary>
        public void Logout()
        {
            // 토큰 제거
            HttpClient.Instance.SetAccessToken(null);
            HttpClient.Instance.SetRefreshToken(null);

            // 메모리 세션 제거
            _currentUserId = null;
            _currentUsername = null;

            // 암호화된 저장소에서 제거
            SecureStorage.DeleteKey("RefreshToken");
            SecureStorage.DeleteKey("UserId");
            SecureStorage.DeleteKey("Username");

            Debug.Log("로그아웃 완료");
            OnLogout?.Invoke();
        }

        /// <summary>
        /// 자동 로그인 시도 (저장된 리프레시 토큰 사용)
        /// </summary>
        public void TryAutoLogin(Action<bool> callback)
        {
            string refreshToken = SecureStorage.GetString("RefreshToken");

            if (string.IsNullOrEmpty(refreshToken))
            {
                Debug.Log("저장된 리프레시 토큰 없음");
                callback?.Invoke(false);
                return;
            }

            var request = new RefreshTokenRequest
            {
                refreshToken = refreshToken
            };

            HttpClient.Instance.Post<RefreshTokenRequest, RefreshTokenResponse>(
                ApiConfig.Auth.REFRESH,
                request,
                response =>
                {
                    if (response.success)
                    {
                        // 새 액세스 토큰 저장
                        HttpClient.Instance.SetAccessToken(response.data.accessToken);
                        HttpClient.Instance.SetRefreshToken(refreshToken);

                        // 사용자 정보 복원
                        _currentUserId = SecureStorage.GetString("UserId");
                        _currentUsername = SecureStorage.GetString("Username");

                        Debug.Log($"자동 로그인 성공: {_currentUsername}");
                        OnLoginSuccess?.Invoke(_currentUsername);
                        callback?.Invoke(true);
                    }
                    else
                    {
                        Debug.LogWarning("자동 로그인 실패: 토큰 만료");
                        Logout();
                        callback?.Invoke(false);
                    }
                },
                error =>
                {
                    Debug.LogError($"자동 로그인 실패: {error}");
                    Logout();
                    callback?.Invoke(false);
                }
            );
        }

        /// <summary>
        /// 인증 데이터 저장 (암호화)
        /// </summary>
        private void SaveAuthData(LoginResponse data)
        {
            // 토큰 설정
            HttpClient.Instance.SetAccessToken(data.accessToken);
            HttpClient.Instance.SetRefreshToken(data.refreshToken);

            // 메모리 세션 저장
            _currentUserId = data.userId;
            _currentUsername = data.username;

            // 암호화된 로컬 저장소에 저장
            SecureStorage.SetString("RefreshToken", data.refreshToken);
            SecureStorage.SetString("UserId", data.userId);
            SecureStorage.SetString("Username", data.username);

            Debug.Log($"로그인 성공: {_currentUsername}");
            OnLoginSuccess?.Invoke(_currentUsername);
        }
    }
}
