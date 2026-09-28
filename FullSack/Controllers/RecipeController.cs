using FullSack.DTO.RecipeGet;
using FullSack.DTO.RecipePut;
using FullSack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FullSack.Controllers
{
	[ApiController]
	[Route("recipes/")]
	public class RecipeController : ControllerBase
	{
		private readonly IRecipeService recipeService;

		public RecipeController(IRecipeService recipeService)
		{
			ArgumentNullException.ThrowIfNull(recipeService);
			this.recipeService = recipeService;
		}

		[HttpGet("search")]
		public async Task<ActionResult<IList<RecipePageDTO>>> GetRecipeCatalogue(
			[FromQuery(Name = "page")] int pageIndex,
			[FromQuery(Name = "size")] int pageSize)
		{
			var result = await this.recipeService.GetRecipesAsListAsync(pageIndex, pageSize);
			return Ok(result);
		}

		[HttpGet("getPage/{slug}")]
		public async Task<ActionResult<RecipePageDTO>> GetRecipePageContent(
			[FromRoute] string slug)
		{
			var result = await this.recipeService.GetRecipeBySlugAsync(slug);
			return Ok(result);
		}

		[Authorize]
		[HttpPost("post")]
		public async Task<ActionResult<RecipePageDTO>> PostRecipe([FromBody] RecipePutDTO newRecipe)
		{
			var result = await this.recipeService.AddRecipeAsync(newRecipe);
			return CreatedAtAction(nameof(GetRecipePageContent), new { id = result.RecipeId }, result);
		}

		[Authorize]
		[HttpPut("put/{id}")]
		public async Task<ActionResult<RecipePageDTO>> PutRecipe(
			[FromRoute] string id,
			[FromBody] RecipePutDTO updatedRecipe)
		{
			var result = await this.recipeService.UpdateRecipeByIdAsync(id, updatedRecipe);
			return Ok(result);
		}

		[Authorize]
		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> DeleteRecipe([FromRoute] string id)
		{
			await this.recipeService.RemoveRecipeByIdAsync(id);
			return Ok();
		}
	}
}
