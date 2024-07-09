using BookLibrary.Business.Abstractions;
using BookLibrary.Data;
using BookLibrary.Data.Entities;
using BookLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace BookLibrary.Business.Services.Logger
{
    public class ExceptionLoggerService : IExceptionLoggerService
	{
		private readonly IServiceProvider _serviceProvider;

		public ExceptionLoggerService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task<ResponseMessage<PagedList<ExceptionLog>>> GetAllAsync(int? pageNumber, int? pageSize, string orderBy, bool orderDirection, string search)
		{
			ResponseMessage<PagedList<ExceptionLog>> responseMessage = new ResponseMessage<PagedList<ExceptionLog>>();
			using (var scope = _serviceProvider.CreateScope())
			using (var context = scope.ServiceProvider.GetService<APIContext>())
			{
				if (context != null)
				{
					List<ExceptionLog> allValues = new List<ExceptionLog>();
					if (!string.IsNullOrEmpty(search))
					{
						allValues = context.ExceptionLogs.Take(20).OrderByDescending(p => p.Id).Where(p => (p.StackTrace != null && p.StackTrace.ToLower().Contains(search))).ToList();
					}
					else
					{
						allValues = context.ExceptionLogs.Take(20).OrderByDescending(p => p.Id).ToList(); // await GetAll().ToListAsync();
					}


					if (pageNumber == null)
						pageNumber = 1;

					if (pageSize == null)
						pageSize = 10;


					responseMessage.Data = PagedList<ExceptionLog>.ToPagedList(allValues.AsQueryable(), pageNumber.Value, pageSize.Value, orderBy, orderDirection);

				}
				else
				{
					responseMessage.SetErrorMessage("Context is null");
				}

				return responseMessage;
			}
		}

		public async Task Log(Exception exception, string? requestPath, string? requestQueryString, string? requestBody, string? userAgent)
		{
			using (var scope = _serviceProvider.CreateScope())
			using (var context = scope.ServiceProvider.GetService<APIContext>())
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.Append(exception.Message);

				var innerException = exception.InnerException;
				while (innerException != null)
				{
					stringBuilder.Append($"\r\n{innerException.Message}");
					innerException = innerException.InnerException;
				}

				if (context != null)
				{
					context.ExceptionLogs.Add(new ExceptionLog
					{
						Source = exception.Source,
						TargetSite = exception.TargetSite != null ? exception.TargetSite.ToString() : null,
						Message = stringBuilder.ToString(),
						StackTrace = exception.StackTrace,
						Path = requestPath,
						QueryString = requestQueryString,
						Body = requestBody,
						UserAgent = userAgent
					});

					await context.SaveChangesAsync();
				}
			}
		}
	}
}