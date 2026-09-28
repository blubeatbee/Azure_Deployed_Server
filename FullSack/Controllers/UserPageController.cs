using FullSack.DTO.User;
using FullSack.Services;
using Microsoft.AspNetCore.Mvc;

namespace FullSack.Controllers
{
	[ApiController]
	[Route("userPage")]
	public class UserPageController : ControllerBase
	{
		private readonly IUserService userService;

		public UserPageController(IUserService userService)
		{
			ArgumentNullException.ThrowIfNull(userService);
			this.userService = userService;
		}

		[HttpGet("getContent/{id}")]
		public async Task<ActionResult<UserPageGetDTO>> GetUserPageContent([FromRoute] string id)
		{
			var userPage = await this.userService.GetUserPageByIdAsync(id);
			return Ok(userPage);
		}

	}
}
