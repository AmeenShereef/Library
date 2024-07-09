using Microsoft.Extensions.DependencyInjection;
using BookLibrary.Business.Abstractions;
using BookLibrary.Data;
using BookLibrary.Data.Entities;

namespace BookLibrary.Business.Services.Logger
{
    public class APILoggerService : IAPILoggerService
    {
		private readonly IServiceProvider _serviceProvider;

		public APILoggerService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

        public async Task Log(string? requestPath, string? requestQueryString, string? method, string? userAgent, string? host, long? userId, string? headers)
        {
            using (var scope = _serviceProvider.CreateScope())
			using (var context = scope.ServiceProvider.GetService<APIContext>())
			{
				
				if (context != null)
				{
					context.APILogs.Add(new APILog
					{
						Path = requestPath,
						QueryString = requestQueryString,
						Method = method,
						UserAgent = userAgent,
						Host = host,
                        UserId = userId,
						Headers = headers
					});

					await context.SaveChangesAsync();
				}
			}
		}
        
    }
}