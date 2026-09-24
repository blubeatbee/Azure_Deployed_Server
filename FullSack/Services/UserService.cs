using FullSack.Entities;
using FullSack.Persistent;
using Microsoft.AspNetCore.Identity;

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

		public async Task RemoveUserByEmailAsync(string email)
		{
			var userToDelete = await this.userManager.FindByEmailAsync(email)
				?? throw new ArgumentNullException(nameof(email));
			await this.userManager.DeleteAsync(userToDelete);
		}

	}
}
