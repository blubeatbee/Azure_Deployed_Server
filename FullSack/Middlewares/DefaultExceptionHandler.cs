using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FullSack.Middlewares
{
	public partial class DefaultExceptionHandler : IExceptionHandler
	{
		private readonly ILogger<DefaultExceptionHandler> logger;

		public DefaultExceptionHandler(ILogger<DefaultExceptionHandler> logger)
		{
			ArgumentNullException.ThrowIfNull(logger);
			this.logger = logger;
		}

		public async ValueTask<bool> TryHandleAsync(
			HttpContext httpContext,
			Exception exception,
			CancellationToken cancellationToken)
		{
			LogException(logger, exception);

			await httpContext.Response.WriteAsJsonAsync<ProblemDetails>(new()
			{
				Status = StatusCodes.Status500InternalServerError,
				Type = "https://www.rfc-editor.org/info/rfc7231/#section-6.6.1",
				Title = "The server encountered an unexpected error",
				Detail = "Something Fucked Up!",
				Instance =  $"{httpContext.Request.Method} {httpContext.Request.PathBase}{httpContext.Request.Path}",
			}, CancellationToken.None);
			return true;
		}

		[LoggerMessage(1, LogLevel.Error, "Ex:", EventName = nameof(LogException))]
		private static partial void LogException(ILogger logger, Exception exception);

	}
}
