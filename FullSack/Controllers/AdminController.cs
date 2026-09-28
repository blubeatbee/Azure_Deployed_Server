using FullSack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FullSack.Controllers
{
	[ApiController]
	[Authorize(Roles = "Admin")]
	[Route("admin")]
	public class AdminController : ControllerBase
	{
		private readonly IUserService userService;

		public AdminController(IUserService userService)
		{
			ArgumentNullException.ThrowIfNull(userService);
			this.userService = userService;
		}

		[HttpDelete("manage/users/delete/{id:guid}")]
		public async Task<IActionResult> DeleteUser([FromRoute] string id)
		{
			await this.userService.DeleteUserByIdAsync(id);
			return NoContent();
		}
	}
}
