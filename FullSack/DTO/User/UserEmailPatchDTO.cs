namespace FullSack.DTO.User
{
	public class UserEmailPatchDTO
	{
		public string OldEmail { get; set; } = null!;
		public string NewEmail { get; set; } = null!;
	}
}
