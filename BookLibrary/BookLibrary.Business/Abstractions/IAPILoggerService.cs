namespace BookLibrary.Business.Abstractions
{
	public interface IAPILoggerService
    {
		Task Log(string? requestPath, string? requestQueryString, string? method, string? userAgent, string? host, long? userId, string? headers);
	}
}
