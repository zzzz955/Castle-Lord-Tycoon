using System;

namespace CastleLordTycoon.Network
{
    /// <summary>
    /// 서버 API 응답 표준 형식
    /// </summary>
    [Serializable]
    public class ApiResponse<T>
    {
        public bool success;
        public T data;
        public ApiError error;
    }

    /// <summary>
    /// API 에러 정보
    /// </summary>
    [Serializable]
    public class ApiError
    {
        public string code;
        public string message;
        public string[] details;
    }

    /// <summary>
    /// 페이징 응답
    /// </summary>
    [Serializable]
    public class PagedResponse<T>
    {
        public T[] items;
        public int totalCount;
        public int page;
        public int pageSize;
    }
}
