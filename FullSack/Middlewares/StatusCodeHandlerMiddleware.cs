using Microsoft.AspNetCore.Mvc;

namespace FullSack.Middlewares
{
	public class StatusCodeHandlerMiddleware : IMiddleware
	{
		private readonly ILogger<StatusCodeHandlerMiddleware> logger;
		public StatusCodeHandlerMiddleware(ILogger<StatusCodeHandlerMiddleware> logger)
		{
			ArgumentNullException.ThrowIfNull(logger);
			this.logger = logger;
		}

		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "FOOBAR!");

				//context.Response.StatusCode = StatusCodes.Status500InternalServerError;

				await context.Response.WriteAsJsonAsync<ProblemDetails>(new()
				{
					Status = StatusCodes.Status500InternalServerError,
					Title = "The server encountered an unexpected error",
					Detail = "Es",
				});
				await context.Response.CompleteAsync();
				//context.Response.StatusCode = context.Response.
			}
		}

	}
}
