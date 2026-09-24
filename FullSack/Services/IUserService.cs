namespace FullSack.Services
{
	public interface IUserService
	{
		Task RemoveUserByEmailAsync(string email);
	}
}
