using FullSack.DTO.User;
using System.Security.Claims;

namespace FullSack.Services
{
	public interface IUserService
	{
		/// <summary>
		/// Asynchronously finds an user by their <paramref name="id"/> and maps their properties to the returned <see cref="UserPageGetDTO"/>.
		/// </summary>
		/// <param name="id">The user's Id to search for.</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="UserPageGetDTO"/>.</returns>
		Task<UserPageGetDTO> GetUserPageByIdAsync(string id);

		/// <summary>
		/// Asynchronously finds an user by their <paramref name="email"/> and maps their properties to the returned <see cref="UserPageGetDTO"/>.
		/// </summary>
		/// <param name="email">The user's email address to search for.</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="UserPageGetDTO"/>.</returns>
		Task<UserPageGetDTO> GetUserPageByEmailAsync(string email);

		/// <summary>
		/// Asynchronously patches the current user's phone number, profile image, surname and first name.
		/// </summary>
		/// <param name="claimsPrincipal">The current <see cref="HttpContext.User"/> that sent this HTTP request.</param>
		/// <param name="newData">The updated value(s).</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing the updated value(s).</returns>
		Task<UserPageGetDTO> PatchPersonalDataAsync(ClaimsPrincipal claimsPrincipal, UserPersonalDataPatchDTO newData);

		/// <summary>
		/// Asynchronously patches the current user's surname and first name.
		/// </summary>
		/// <param name="claimsPrincipal">The current <see cref="HttpContext.User"/> that sent this HTTP request.</param>
		/// <param name="newNames">The updated value(s).</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing the updated value(s).</returns>
		Task<UserNamesPatchDTO> PatchNamesAsync(ClaimsPrincipal claimsPrincipal, UserNamesPatchDTO newNames);

		/// <summary>
		/// Asynchronously patches the current user's profile image.
		/// </summary>
		/// <param name="claimsPrincipal">The current <see cref="HttpContext.User"/> that sent this HTTP request.</param>
		/// <param name="newProfileImage">The updated profile image.</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing the updated profile image.</returns>
		Task<byte[]?> PatchProfileImageAsync(ClaimsPrincipal claimsPrincipal, byte[]? newProfileImage);

		/// <summary>
		/// Asynchronously patches the current user's phone number.
		/// </summary>
		/// <param name="claimsPrincipal">The current <see cref="HttpContext.User"/> that sent this HTTP request.</param>
		/// <param name="newPhoneNumber">The updated value(s).</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing the updated value(s).</returns>
		Task<string?> PatchPhoneNumberAsync(ClaimsPrincipal claimsPrincipal, string? newPhoneNumber);

		/// <summary>
		/// Asynchronously deletes the current user.
		/// </summary>
		/// <param name="claimsPrincipal">The current <see cref="HttpContext.User"/> that sent this HTTP request.</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
		Task DeleteUserAsync(ClaimsPrincipal claimsPrincipal);

		/// <summary>
		/// Asynchronously deletes user by the their <paramref name="id"/>.
		/// </summary>
		/// <param name="id">The id to search for.</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
		Task DeleteUserByIdAsync(string id);

		/// <summary>
		/// Asynchronously generates a new token to be used for changing email address.
		/// </summary>
		/// <param name="claimsPrincipal">The current <see cref="HttpContext.User"/> that sent this HTTP request.</param>
		/// <param name="newEmail">The new email address which will recieve a confirmation email.</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing the change email token as string.</returns>
		Task<string> GenerateChangeEmailTokenAsync(ClaimsPrincipal claimsPrincipal, string newEmail);

		/// <summary>
		/// Asynchronously changes the current user's email address.
		/// </summary>
		/// <param name="claimsPrincipal">The current <see cref="HttpContext.User"/> that sent this HTTP request.</param>
		/// <param name="token">The change email token to compare to.</param>
		/// <param name="oldEmail">The current user's email address.</param>
		/// <param name="newEmail">The new email address.</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
		Task ChangeEmailAsync(ClaimsPrincipal claimsPrincipal, string token, string oldEmail, string newEmail);
	}
}
