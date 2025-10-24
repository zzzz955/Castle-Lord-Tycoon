namespace CastleLordTycoon.Server.Metadata.Exceptions;

public sealed class MetadataValidationException : Exception
{
    public MetadataValidationException(IEnumerable<string> errors)
        : base("메타데이터 검증에 실패했습니다.")
    {
        Errors = errors.ToArray();
    }

    public IReadOnlyList<string> Errors { get; }
}
