namespace CastleLordTycoon.Server.Configuration;

/// <summary>
/// 전역 메타데이터 설정. CSV 루트 경로를 지정한다.
/// </summary>
public sealed class MetadataOptions
{
    /// <summary>
    /// CSV 데이터가 위치한 루트 경로. 상대 경로로 설정하면 애플리케이션 기준 경로로 변환된다.
    /// </summary>
    public string DataRootPath { get; set; } = Path.Combine("..", "docs", "02-data");
}
