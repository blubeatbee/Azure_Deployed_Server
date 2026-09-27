using FullSack.DTO.User;
using FullSack.Entities;
using FullSack.Persistent;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FullSack.Services
{
	public class UserService : IUserService
	{
		public readonly IUnitOfWork workUnit;
		public readonly UserManager<User> userManager;

		public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager)
		{
			this.workUnit = unitOfWork;
			this.userManager = userManager;
		}

		public async Task<UserPageGetDTO> GetUserPageByIdAsync(string id)
		{
			var user = await this.userManager.FindByIdAsync(id)
				?? throw new ArgumentException($"No user found with that {nameof(id)}.", nameof(id));
			var userPage = new UserPageGetDTO()
			{
				UserId = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				Surname = user.Surname,
				FirstName = user.FirstName,
				ProfileImage = user.ProfileImage,
			};
			return userPage;
		}

		public async Task<UserPageGetDTO> GetUserPageByEmailAsync(string email)
		{
			var user = await this.userManager.FindByEmailAsync(email)
				?? throw new ArgumentException($"No user found with that {nameof(email)}.", nameof(email));
			var userPage = new UserPageGetDTO()
			{
				UserId = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				Surname = user.UserName,
				FirstName = user.FirstName,
				ProfileImage = user.ProfileImage,
			};
			return userPage;
		}

		public async Task<UserPageGetDTO> PatchPersonalDataAsync(ClaimsPrincipal claimsPrincipal, UserPersonalDataPatchDTO newData)
		{
			var user = await this.userManager.GetUserAsync(claimsPrincipal)
				?? throw new ArgumentException($"No user found with that {nameof(claimsPrincipal)}.", nameof(claimsPrincipal));
			user.FirstName = newData.FirstName;
			user.NormalizedFirstName = newData.FirstName?.ToUpperInvariant();
			user.Surname = newData.Surname;
			user.NormalizedSurname = newData.Surname?.ToUpperInvariant();
			user.PhoneNumber = newData.PhoneNumber;
			user.ProfileImage = newData.ProfileImage;
			var result = await this.userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				var errors = string.Empty;
				foreach (var e in result.Errors)
				{
					errors += $"{e.Code}: {e.Description}\n";
				}
				throw new InvalidOperationException(errors);
			}
			return await this.GetUserPageByIdAsync(user.Id);
		}

		public async Task<UserNamesPatchDTO> PatchNamesAsync(ClaimsPrincipal claimsPrincipal, UserNamesPatchDTO newNames)
		{
			var user = await this.userManager.GetUserAsync(claimsPrincipal)
				?? throw new ArgumentException($"No user found with that {nameof(claimsPrincipal)}.", nameof(claimsPrincipal));
			user.FirstName = newNames.FirstName;
			user.NormalizedFirstName = newNames.FirstName?.ToUpperInvariant();
			user.Surname = newNames.Surname;
			user.NormalizedSurname = newNames.Surname?.ToUpperInvariant();
			var result = await this.userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				var errors = string.Empty;
				foreach (var e in result.Errors)
				{
					errors += $"{e.Code}: {e.Description}\n";
				}
				throw new InvalidOperationException(errors);
			}
			return newNames;
		}

		public async Task<byte[]?> PatchProfileImageAsync(ClaimsPrincipal claimsPrincipal, byte[]? newProfileImage)
		{
			var user = await this.userManager.GetUserAsync(claimsPrincipal)
				?? throw new ArgumentException($"No user found with that {nameof(claimsPrincipal)}.", nameof(claimsPrincipal));
			user.ProfileImage = newProfileImage;
			var result = await this.userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				var errors = string.Empty;
				foreach (var e in result.Errors)
				{
					errors += $"{e.Code}: {e.Description}\n";
				}
				throw new InvalidOperationException(errors);
			}
			return newProfileImage;
		}

		public async Task<string?> PatchPhoneNumberAsync(ClaimsPrincipal claimsPrincipal, string? newPhoneNumber)
		{
			var user = await userManager.GetUserAsync(claimsPrincipal)
				?? throw new ArgumentException($"No user found with that {nameof(claimsPrincipal)}.", nameof(claimsPrincipal));
			if (newPhoneNumber == null || newPhoneNumber.Length == 0)
			{
				newPhoneNumber = null;
			}
			var result = await userManager.SetPhoneNumberAsync(user, newPhoneNumber);
			if (!result.Succeeded)
			{
				var errors = string.Empty;
				foreach (var e in result.Errors)
				{
					errors += $"{e.Code}: {e.Description}\n";
				}
				throw new InvalidOperationException(errors);
			}
			return newPhoneNumber;
		}

		public async Task DeleteUserAsync(ClaimsPrincipal claimsPrincipal)
		{
			var userToDelete = await this.userManager.GetUserAsync(claimsPrincipal)
				?? throw new ArgumentException($"No user found with that {nameof(claimsPrincipal)}.", nameof(claimsPrincipal));
			var result = await this.userManager.DeleteAsync(userToDelete);
			if (!result.Succeeded)
			{
				var errors = string.Empty;
				foreach (var e in result.Errors)
				{
					errors += $"{e.Code}: {e.Description}\n";
				}
				throw new InvalidOperationException(errors);
			}
		}

		public async Task DeleteUserByIdAsync(string id)
		{
			var userToDelete = await this.userManager.FindByIdAsync(id)
				?? throw new ArgumentException($"No user found with that {nameof(id)}.", nameof(id));
			var result = await this.userManager.DeleteAsync(userToDelete);
			if (!result.Succeeded)
			{
				var errors = string.Empty;
				foreach (var e in result.Errors)
				{
					errors += $"{e.Code}: {e.Description}\n";
				}
				throw new InvalidOperationException(errors);
			}
		}

		public async Task<string> GenerateChangeEmailTokenAsync(ClaimsPrincipal claimsPrincipal, string newEmail)
		{
			var user = await this.userManager.GetUserAsync(claimsPrincipal)
				?? throw new ArgumentException($"No user found with that {nameof(claimsPrincipal)}.", nameof(claimsPrincipal));
			if (await this.userManager.FindByEmailAsync(newEmail) != null)
			{
				throw new ArgumentException("User with that email already exists.", nameof(newEmail));
			}
			return await this.userManager.GenerateChangeEmailTokenAsync(user, newEmail);
		}

		public async Task ChangeEmailAsync(ClaimsPrincipal claimsPrincipal, string token, string oldEmail, string newEmail)
		{
			var user = await this.userManager.GetUserAsync(claimsPrincipal)
				?? throw new ArgumentException($"No user found with that {nameof(claimsPrincipal)}.", nameof(claimsPrincipal));

			// Updates email
			var resultEmail = await this.userManager.ChangeEmailAsync(user, newEmail, token);
			if (!resultEmail.Succeeded)
			{
				var errors = string.Empty;
				foreach (var e in resultEmail.Errors)
				{
					errors += $"{e.Code}: {e.Description}\n";
				}
				throw new InvalidOperationException(errors);
			}
			await userManager.UpdateNormalizedEmailAsync(user);

			// Updates username
			var userNameResult = await userManager.SetUserNameAsync(user, newEmail);
			if (!userNameResult.Succeeded)
			{
				var errors = string.Empty;
				foreach (var e in userNameResult.Errors)
				{
					errors += $"{e.Code}: {e.Description}\n";
				}
				throw new InvalidOperationException(errors);
			}
			await userManager.UpdateNormalizedUserNameAsync(user);
		}

	}
}
