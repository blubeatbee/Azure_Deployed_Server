using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FullSack.Middlewares
{
	public sealed partial class GlobalExceptionHandlerMiddleware : IExceptionHandler
	{
		private readonly ILogger<GlobalExceptionHandlerMiddleware> logger;
		private readonly IProblemDetailsService problemDetailsService;

		public GlobalExceptionHandlerMiddleware(
			ILogger<GlobalExceptionHandlerMiddleware> logger, IProblemDetailsService problemDetailsService)
		{
			ArgumentNullException.ThrowIfNull(logger);
			ArgumentNullException.ThrowIfNull(problemDetailsService);
			this.logger = logger;
			this.problemDetailsService = problemDetailsService;
		}

		public async ValueTask<bool> TryHandleAsync(
			HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
		{
			LogException(this.logger, exception);

			//context.Response.Clear();

			var statusCode = GetStatusCodeAsync(exception);
			httpContext.Response.StatusCode = statusCode;
			return await this.problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
			{
				HttpContext = httpContext,
				Exception = exception,
				ProblemDetails = new ProblemDetails()
				{
					Type = GetProblemDetailsType(statusCode),
					Title = "An error has occured!",
					Status = statusCode,
					Detail = exception.Message,

				}
			});
		}

		[LoggerMessage(1, LogLevel.Warning, "An unhandled exception has occured: ", EventName = nameof(LogException))]
		private static partial void LogException(ILogger logger, Exception exception);


#pragma warning disable IDE0022 // Use expression body for method
		private static int GetStatusCodeAsync(Exception exception)
		{
			return exception switch
			{
				ApplicationException => StatusCodes.Status400BadRequest,
				ArithmeticException => StatusCodes.Status500InternalServerError,
				ArgumentException => StatusCodes.Status404NotFound,
				JsonException => StatusCodes.Status400BadRequest,
				FormatException => StatusCodes.Status400BadRequest,
				DbUpdateException => StatusCodes.Status409Conflict,
				NotImplementedException => StatusCodes.Status501NotImplemented,
				Exception => StatusCodes.Status500InternalServerError,
				_ => StatusCodes.Status500InternalServerError,
			};
		}

		private static string GetProblemDetailsType(int statusCode)
		{
			return statusCode switch
			{
				StatusCodes.Status400BadRequest => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.1",
				StatusCodes.Status401Unauthorized => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.2",
				StatusCodes.Status404NotFound => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.5",
				StatusCodes.Status409Conflict => "https://www.rfc-editor.org/info/rfc9110/#section-15.5.10",
				StatusCodes.Status500InternalServerError => "https://www.rfc-editor.org/info/rfc9110/#section-15.6.1",
				StatusCodes.Status501NotImplemented => "https://www.rfc-editor.org/info/rfc9110/#section-15.6.2",
				StatusCodes.Status503ServiceUnavailable => "https://www.rfc-editor.org/info/rfc9110/#section-15.6.4",
				_ => "https://www.rfc-editor.org/info/rfc9110/#section-15.6.1",
			};
		}
#pragma warning restore IDE0022 // Use expression body for method

	}
}
