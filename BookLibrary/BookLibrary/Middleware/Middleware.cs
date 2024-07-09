using BookLibrary.API.Extensions;
using BookLibrary.Business.Abstractions;
using BookLibrary.Infrastructure.Exceptions;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json;

namespace BookLibrary.API.Middleware
{
    public class Middleware
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionLoggerService _exceptionLogger;
        private readonly IAPILoggerService _apiLogger;
        private readonly ILogger<Middleware> _logger;
        private readonly IConfiguration _configuration;

        public Middleware(RequestDelegate next, IExceptionLoggerService exceptionLogger, IAPILoggerService apiLogger, IConfiguration configuration, ILogger<Middleware> logger)
        {
            _next = next;
            _exceptionLogger = exceptionLogger;
            _apiLogger = apiLogger;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;
            await LogRequest(context);

            try
            {

                if (!ValidConsumer(context))
                    throw new UnauthorizedAccessException("Not Authorized. Invalid Request");

                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                string result = "";

                var errorObject = new ErrorObject
                {
                    path = path,
                    message = error?.Message,
                };

                var innerException = error?.InnerException;
                while (innerException != null)
                {
                    errorObject.message += $"\r\n{innerException.Message}";
                    innerException = innerException.InnerException;
                }



                switch (error)
                {
                    case KeyNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        errorObject.status = (int)HttpStatusCode.NotFound;
                        result = JsonConvert.SerializeObject(errorObject);
                        break;
                    case ValidationException e:
                        response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                        errorObject.status = (int)HttpStatusCode.UnprocessableEntity;
                        ValidationErrorObject validationErrorObject = new ValidationErrorObject(errorObject);
                        validationErrorObject.validation = e.validationObject;
                        result = JsonConvert.SerializeObject(validationErrorObject);
                        break;
                    case UnauthorizedAccessException e:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        errorObject.status = (int)HttpStatusCode.Unauthorized;
                        result = JsonConvert.SerializeObject(errorObject);
                        break;
                    case Exception e:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        errorObject.status = (int)HttpStatusCode.InternalServerError;
                        result = JsonConvert.SerializeObject(errorObject);
                        await WriteExceptionLog(context, e);
                        break;
                }

                await response.WriteAsync(result);
            }
        }

        private bool ValidConsumer(HttpContext context)
        {
            var referer = context.Request.Headers.ContainsKey("Referer")
                ? context.Request.Headers["Referer"].ToString()
                : null;

            if (referer != null)
            {
                if (_configuration["AllowedConsumers"] != null)
                {
                    string validConsumers = _configuration["AllowedConsumers"]?.ToString().Trim().ToLower() ?? "";
                    if (string.IsNullOrEmpty(validConsumers))
                        throw new Exception("Valid Consumers setting missing");

                    if (validConsumers == "*")
                        return true;

                    _logger.LogInformation("Valid Consumers - " + validConsumers);

                    Uri referrerUri = new Uri(referer.ToString());
                    _logger.LogInformation("Referer Value - " + referrerUri.Host.Replace("wwww", ""));

                    if (validConsumers.Split(',').Contains(referrerUri.Host.Replace("wwww","")))
                        return true;
                }
            }

            return false;
        }

        private async Task LogRequest(HttpContext context)
        {
            var path = context.Request.Path.Value;
            var query = context.Request.QueryString.Value;
            var method = context.Request.Method;
            var userAgent = context.Request.Headers.ContainsKey("User-Agent")
                ? context.Request.Headers["User-Agent"].ToString()
                : null;
            var host = context.Request.Headers.ContainsKey("Host")
                ? context.Request.Headers["Host"].ToString()
                : null;

            string headers = string.Empty;

            if(context.Request.Headers != null)
            {
                if(context.Request.Headers.Count > 0)
                    headers = JsonConvert.SerializeObject(context.Request.Headers);
            }
                        
            long? userId = context.User != null ? context.User.GetUserId() : null;

            await _apiLogger.Log(path, query, method, userAgent, host, userId, headers);

        }

        private async Task WriteExceptionLog(HttpContext context, Exception e)
        {
            var path = context.Request.Path.Value;
            var query = context.Request.QueryString.Value;
            var userAgent = context.Request.Headers.ContainsKey("User-Agent")
                ? context.Request.Headers["User-Agent"].ToString()
                : null;
            string? body = null;

            if (context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "PATCH")
            {
                var requestBody = context.Request.Body;
                if (requestBody.CanSeek)
                {
                    requestBody.Seek(0L, SeekOrigin.Begin);

                    var streamReader = new StreamReader(requestBody);
                    body = await streamReader.ReadToEndAsync();
                }
            }

            await _exceptionLogger.Log(e, path, query, body, userAgent);
        }
    }

    public class ErrorObject
    {
        [JsonProperty(Order = 1)]
        public int status { get; set; }
        [JsonProperty(Order = 2)]
        public string? path { get; set; }
        [JsonProperty(Order = 3)]
        public string? message { get; set; }

        public ErrorObject() { }

        protected ErrorObject(ErrorObject error)
        {
            path = error.path;
            status = error.status;
            message = error.message;
        }
    }

    public class ValidationErrorObject : ErrorObject
    {
        [JsonProperty(Order = 4)]
        public dynamic? validation { get; set; }

        public ValidationErrorObject(ErrorObject errorObject) : base(errorObject) { }


    }
}
