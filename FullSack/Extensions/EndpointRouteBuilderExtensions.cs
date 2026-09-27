using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FullSack.Extensions;

public static class EndpointRouteBuilderExtensions
{
	public static IEndpointConventionBuilder MapIdentityCustomApi<TUser>(this IEndpointRouteBuilder endpoints)
		where TUser : class, new()
	{
		ArgumentNullException.ThrowIfNull(endpoints);

		var routeGroup = endpoints.MapGroup(string.Empty);

		routeGroup.MapGet("/logout", async ([FromServices] IServiceProvider serviceProvider) =>
		{
			var signInManager = serviceProvider.GetRequiredService<SignInManager<TUser>>();
			await signInManager.SignOutAsync().ConfigureAwait(false);
		});

		//var accountGroup = routeGroup.MapGroup("/manage").RequireAuthorization();

		//accountGroup.MapDelete("/delete", async Task<Results<NoContent, NotFound, InternalServerError>>
		//	(ClaimsPrincipal claimsPrincipal, [FromServices] IServiceProvider sp) =>
		//{
		//	var userManager = sp.GetRequiredService<UserManager<TUser>>();
		//	if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
		//	{
		//		return TypedResults.NotFound();
		//	}
		//	var result = await userManager.DeleteAsync(user);
		//	if (!result.Succeeded)
		//	{
		//		return TypedResults.InternalServerError();
		//	}
			
		//	return TypedResults.NoContent();
		//});

		return routeGroup;
	}
}
