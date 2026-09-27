using FullSack.DTO.User;
using FullSack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FullSack.Controllers
{
	[ApiController]
	[Authorize]
	[Route("account")]
	public class AccountController : ControllerBase
	{
		private readonly IUserService userService;

		public AccountController(IUserService userService)
		{
			ArgumentNullException.ThrowIfNull(userService);
			this.userService = userService;
		}


		[HttpPatch("manage/personalData")]
		public async Task<ActionResult<UserPageGetDTO>> PatchPersonalData(
			[FromBody] UserPersonalDataPatchDTO newData)
		{
			try
			{
				var result = await this.userService.PatchPersonalDataAsync(HttpContext.User, newData);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(JsonConvert.SerializeObject(ex));
			}
		}

		[HttpPost("manage/token/changeEmail")]
		public async Task<IActionResult> GenerateChangeEmailToken(
			[FromBody] UserEmailPatchDTO emailDto)
		{
			try
			{
				var token = await this.userService.GenerateChangeEmailTokenAsync(HttpContext.User, emailDto.NewEmail);
				var link = Url.Action(nameof(ChangeEmailToken), "account", new { token = token, oldEmail = emailDto.OldEmail, newEmail = emailDto.NewEmail }, HttpContext.Request.Scheme);
				throw new NotImplementedException("Need to implement EmailSender service that sends a confirmation email to the new email address.");
			}
			catch (NotImplementedException ex)
			{
				return StatusCode(501, JsonConvert.SerializeObject(ex));
			}
			catch (Exception ex)
			{
				return BadRequest(JsonConvert.SerializeObject(ex));
			}
		}

		[HttpPatch]
		public async Task<IActionResult> ChangeEmailToken(
			[FromQuery] string token, [FromQuery] string oldEmail, [FromQuery] string newEmail)
		{
			try
			{
				throw new NotImplementedException();
				//await this.userService.ChangeEmailAsync(HttpContext.User, token, oldEmail, newEmail);
				//return Ok();
			}
			catch (NotImplementedException ex)
			{
				return StatusCode(501, JsonConvert.SerializeObject(ex));
			}
			catch (Exception ex)
			{
				return BadRequest(JsonConvert.SerializeObject(ex));
			}
		}

		[HttpDelete("manage/deleteUser")]
		public async Task<IActionResult> DeleteUser()
		{
			try
			{
				await this.userService.DeleteUserAsync(HttpContext.User);
				return NoContent();
			}
			catch (Exception ex)
			{
				return BadRequest(JsonConvert.SerializeObject(ex));
			}
		}

	}
}
