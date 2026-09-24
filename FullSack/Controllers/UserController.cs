using FullSack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FullSack.Controllers
{
	[ApiController]
	[Route("account/[action]")]
	public class UserController : ControllerBase
	{
		public UserController()
		{
		}

		[Authorize]
		[HttpDelete("{email}")]
		public async Task<IActionResult> DeleteUserAsync(
			[FromRoute] string email,
			[FromServices] IUserService userService)
		{
			try
			{
				await userService.RemoveUserByEmailAsync(email);
				return NoContent();
			}
			catch (Exception)
			{
				return NotFound();
			}
		}

	}
}
