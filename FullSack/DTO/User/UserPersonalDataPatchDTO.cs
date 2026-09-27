namespace FullSack.DTO.User
{
	public class UserPersonalDataPatchDTO
	{
		public string? Surname { get; set; }
		public string? FirstName { get; set; }
		public string? PhoneNumber { get; set; }
		public byte[]? ProfileImage { get; set; }
	}
}
