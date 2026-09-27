using FullSack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
			try
			{
				await this.userService.DeleteUserByIdAsync(id);
				return NoContent();
			}
			catch (Exception ex)
			{
				return BadRequest(JsonConvert.SerializeObject(ex));
			}
		}
	}
}
