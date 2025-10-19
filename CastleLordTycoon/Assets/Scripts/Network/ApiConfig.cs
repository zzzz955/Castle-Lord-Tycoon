namespace CastleLordTycoon.Network
{
    /// <summary>
    /// 서버 API 설정 및 엔드포인트 상수
    /// 실제 URL은 빌드 설정 또는 Remote Config에서 로드
    /// </summary>
    public static class ApiConfig
    {
        // 런타임에 설정됨 (Remote Config 또는 빌드 설정)
        private static string _baseUrl;

        public static string BASE_URL
        {
            get
            {
                if (string.IsNullOrEmpty(_baseUrl))
                {
                    // 기본값 (개발 환경)
#if UNITY_EDITOR
                    _baseUrl = "http://localhost:10010";
#else
                    // 실제 빌드에서는 반드시 SetBaseUrl로 설정 필요
                    _baseUrl = string.Empty;
#endif
                }
                return _baseUrl;
            }
        }

        /// <summary>
        /// 서버 URL 설정 (앱 시작 시 Remote Config에서 가져옴)
        /// </summary>
        public static void SetBaseUrl(string url)
        {
            _baseUrl = url;
        }

        // SignalR Hub URL
        public static string WebSocketHubUrl => BASE_URL + "/gamehub";

        // API 엔드포인트 (상대 경로만 저장)
        public static class Auth
        {
            public const string REGISTER = "/api/auth/register";
            public const string LOGIN = "/api/auth/login";
            public const string GOOGLE_LOGIN = "/api/auth/google-login";
            public const string REFRESH = "/api/auth/refresh";
            public const string LOGOUT = "/api/auth/logout";
        }

        public static class Player
        {
            public const string GET_PROFILE = "/api/player/profile";
            public const string UPDATE_POSITION = "/api/player/position";
            public const string GET_INVENTORY = "/api/player/inventory";
        }

        public static class Combat
        {
            public const string START_BATTLE = "/api/combat/start";
            public const string GET_RESULT = "/api/combat/result";
        }

        public static class Territory
        {
            public const string PLACE_FLAG = "/api/territory/flag";
            public const string GET_TERRITORIES = "/api/territory/list";
        }

        public static class Town
        {
            public const string GET_TOWNS = "/api/town/list";
            public const string VISIT_TOWN = "/api/town/visit";
        }

        // 네트워크 설정
        public const int REQUEST_TIMEOUT = 30; // 초
        public const int HEARTBEAT_INTERVAL = 30; // 초
        public const int POSITION_SYNC_INTERVAL = 10; // 초
        public const int MAX_RETRY_COUNT = 3;
    }
}
