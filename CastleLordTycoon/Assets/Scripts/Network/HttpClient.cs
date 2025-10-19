using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace CastleLordTycoon.Network
{
    /// <summary>
    /// HTTP REST API 클라이언트
    /// Unity UnityWebRequest 기반
    /// </summary>
    public class HttpClient : MonoBehaviour
    {
        private static HttpClient _instance;
        public static HttpClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("HttpClient");
                    _instance = go.AddComponent<HttpClient>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private string _accessToken;
        private string _refreshToken;

        /// <summary>
        /// 액세스 토큰 설정
        /// </summary>
        public void SetAccessToken(string token)
        {
            _accessToken = token;
        }

        /// <summary>
        /// 리프레시 토큰 설정
        /// </summary>
        public void SetRefreshToken(string token)
        {
            _refreshToken = token;
        }

        /// <summary>
        /// GET 요청
        /// </summary>
        public void Get<T>(string endpoint, Action<ApiResponse<T>> onComplete, Action<string> onError = null)
        {
            StartCoroutine(SendRequest<T>(UnityWebRequest.kHttpVerbGET, endpoint, null, onComplete, onError));
        }

        /// <summary>
        /// POST 요청
        /// </summary>
        public void Post<TRequest, TResponse>(string endpoint, TRequest body, Action<ApiResponse<TResponse>> onComplete, Action<string> onError = null)
        {
            string jsonBody = JsonUtility.ToJson(body);
            StartCoroutine(SendRequest<TResponse>(UnityWebRequest.kHttpVerbPOST, endpoint, jsonBody, onComplete, onError));
        }

        /// <summary>
        /// PUT 요청
        /// </summary>
        public void Put<TRequest, TResponse>(string endpoint, TRequest body, Action<ApiResponse<TResponse>> onComplete, Action<string> onError = null)
        {
            string jsonBody = JsonUtility.ToJson(body);
            StartCoroutine(SendRequest<TResponse>(UnityWebRequest.kHttpVerbPUT, endpoint, jsonBody, onComplete, onError));
        }

        /// <summary>
        /// DELETE 요청
        /// </summary>
        public void Delete<T>(string endpoint, Action<ApiResponse<T>> onComplete, Action<string> onError = null)
        {
            StartCoroutine(SendRequest<T>(UnityWebRequest.kHttpVerbDELETE, endpoint, null, onComplete, onError));
        }

        /// <summary>
        /// HTTP 요청 코루틴
        /// </summary>
        private IEnumerator SendRequest<T>(string method, string endpoint, string body, Action<ApiResponse<T>> onComplete, Action<string> onError)
        {
            string url = ApiConfig.BASE_URL + endpoint;
            UnityWebRequest request;

            // 요청 생성
            if (method == UnityWebRequest.kHttpVerbGET || method == UnityWebRequest.kHttpVerbDELETE)
            {
                request = UnityWebRequest.Get(url);
                request.method = method;
            }
            else
            {
                request = new UnityWebRequest(url, method);
                if (!string.IsNullOrEmpty(body))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                }
                request.downloadHandler = new DownloadHandlerBuffer();
            }

            // 헤더 설정
            request.SetRequestHeader("Content-Type", "application/json");

            // 인증 토큰 추가
            if (!string.IsNullOrEmpty(_accessToken))
            {
                request.SetRequestHeader("Authorization", "Bearer " + _accessToken);
            }

            // 타임아웃 설정
            request.timeout = ApiConfig.REQUEST_TIMEOUT;

            // 요청 전송
            yield return request.SendWebRequest();

            // 응답 처리
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;

                try
                {
                    ApiResponse<T> response = JsonUtility.FromJson<ApiResponse<T>>(responseText);
                    onComplete?.Invoke(response);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"JSON 파싱 실패: {ex.Message}\nResponse: {responseText}");
                    onError?.Invoke($"응답 파싱 실패: {ex.Message}");
                }
            }
            else
            {
                // 401 Unauthorized → 토큰 갱신 시도
                if (request.responseCode == 401 && !string.IsNullOrEmpty(_refreshToken))
                {
                    Debug.Log("액세스 토큰 만료, 갱신 시도...");
                    // TODO: 토큰 갱신 로직 구현
                }

                string errorMsg = $"[{request.responseCode}] {request.error}";
                Debug.LogError($"HTTP 요청 실패: {errorMsg}\nURL: {url}");
                onError?.Invoke(errorMsg);
            }

            request.Dispose();
        }
    }
}
