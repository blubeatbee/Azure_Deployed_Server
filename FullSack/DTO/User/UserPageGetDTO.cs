namespace FullSack.DTO.User
{
	public class UserPageGetDTO
	{
		public string? UserId { get; set; }
		public string? UserName { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Surname { get; set; }
		public string? FirstName { get; set; }
		public byte[]? ProfileImage { get; set; }

		//public ICollection<RecipeCatalogueDTO> Recipes { get; set; } = new List<RecipeCatalogueDTO>();
	}
}
