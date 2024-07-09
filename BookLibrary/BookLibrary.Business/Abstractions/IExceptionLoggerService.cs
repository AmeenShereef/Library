using BookLibrary.Data;
using BookLibrary.Data.Entities;
using BookLibrary.Models;

namespace BookLibrary.Business.Abstractions
{
    public interface IExceptionLoggerService
    {
        Task Log(Exception exception, string? requestPath, string? requestQueryString, string? requestBody, string? userAgent);
        ResponseMessage<PagedList<ExceptionLog>> GetAllAsync(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, string search);
    }
}
