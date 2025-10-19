namespace CastleLordTycoon.Network
{
    /// <summary>
    /// 서버 API 설정 및 엔드포인트 상수
    /// </summary>
    public static class ApiConfig
    {
        // 환경별 베이스 URL
#if UNITY_EDITOR
        public const string BASE_URL = "http://localhost:10010";
#elif DEVELOPMENT
        public const string BASE_URL = "http://localhost:10010";
#else
        public const string BASE_URL = "https://castle.yourdomain.com";
#endif

        // SignalR Hub URL
        public static string WebSocketHubUrl => BASE_URL + "/gamehub";

        // API 엔드포인트
        public static class Auth
        {
            public const string REGISTER = "/api/auth/register";
            public const string LOGIN = "/api/auth/login";
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
