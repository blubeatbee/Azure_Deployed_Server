using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace FullSack.Middlewares
{
	public sealed partial class ValidationExceptionHandlerMiddleware : IExceptionHandler
	{
		private readonly ILogger<ValidationExceptionHandlerMiddleware> logger;
		private readonly IProblemDetailsService problemDetailsService;

		public ValidationExceptionHandlerMiddleware(
			ILogger<ValidationExceptionHandlerMiddleware> logger, IProblemDetailsService problemDetailsService)
		{
			ArgumentNullException.ThrowIfNull(logger);
			ArgumentNullException.ThrowIfNull(problemDetailsService);
			this.logger = logger;
			this.problemDetailsService = problemDetailsService;
		}

		public async ValueTask<bool> TryHandleAsync(
			HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
		{
			if (exception is not ValidationException validationException)
			{
				return false;
			}

			httpContext.Response.Clear();

			httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

			var context = new ProblemDetailsContext()
			{
				HttpContext = httpContext,
				Exception = exception,
				ProblemDetails = new ProblemDetails()
				{
					Type = "https://www.rfc-editor.org/info/rfc9110/#section-15.5.1",
					Title = "One or more validation errors has occured!",
					Status = StatusCodes.Status400BadRequest,
				}
			};

			var errors = new Dictionary<string, string>();
			int i = 0;
			foreach (var m in validationException.ValidationResult.MemberNames)
			{
				errors.Add(i.ToString(CultureInfo.InvariantCulture), m);
				i++;
			}
			context.ProblemDetails.Extensions.Add("errors", errors);
			return await problemDetailsService.TryWriteAsync(context);
		}

	}
}
