using FullSack.Data;
using FullSack.Entities;
using FullSack.Extensions;
using FullSack.Middlewares;
using FullSack.Persistent;
using FullSack.Repositories;
using FullSack.Services;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SemiWare.Utils;

namespace FullSack
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			#region REGISTER SERVICES TO CONTAINER

			builder.Services.AddHttpLogging(options =>
			{
				options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders | HttpLoggingFields.ResponsePropertiesAndHeaders;
				options.MediaTypeOptions.AddText("application/javascript");
				options.RequestBodyLogLimit = 4096;
				options.ResponseBodyLogLimit = 4096;
				options.CombineLogs = false;
			});

			builder.Services.AddProblemDetails(options =>
			{
				options.CustomizeProblemDetails = (context) =>
				{
					context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
				};
			});

			builder.Services.AddExceptionHandler<ValidationExceptionHandlerMiddleware>();
			builder.Services.AddExceptionHandler<GlobalExceptionHandlerMiddleware>();
			
			builder.Services.AddDbContext<FullSackDbContext>();

			builder.Services.AddIdentityApiEndpoints<User>(options =>
			{
				options.User.RequireUniqueEmail = true;
				options.Password.RequiredLength = 6;
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireNonAlphanumeric = true;
			})
				.AddRoles<IdentityRole>()
				.AddDefaultTokenProviders()
				.AddEntityFrameworkStores<FullSackDbContext>();

			builder.Services.ConfigureApplicationCookie(config =>
			{
				config.ClaimsIssuer = builder.Configuration.GetValue<string>("Cookie:issuer");
				config.ExpireTimeSpan = TimeSpan.FromMinutes(2);
				config.Cookie.SameSite = SameSiteMode.Strict;
				config.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
			});

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("CorsDev", policy =>
				{
					policy.WithOrigins(builder.Configuration.GetAllowedOrigin("ClientUrl1")!)
						.AllowAnyMethod()
						.AllowAnyHeader()
						.AllowCredentials();
				});
				options.AddPolicy("CorsProd", policy =>
				{
					policy.WithOrigins(builder.Configuration.GetAllowedOrigin("ClientUrl1")!)
					.WithMethods(HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete, HttpMethods.Patch)
					.AllowAnyHeader()
					.AllowCredentials();
				});
			});

			builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			builder.Services.AddScoped<IRecipeService, RecipeService>();
			builder.Services.AddScoped<IUserService, UserService>();

			builder.Services.AddControllers();
			// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
			builder.Services.AddOpenApi();

			#endregion

			var app = builder.Build();

			#region ADD MIDDLEWARES

			using (var scope = app.Services.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<FullSackDbContext>();
				// Attempts to apply any pending migrations
				// See https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli#migration-locking
				Attempt.ToDo(
					action: context.Database.Migrate,
					interval: TimeSpan.FromSeconds(2),
					maxAttempts: 10,
					retryMessage: "Database is not ready to migrate yet. Retrying...");
			}

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseHttpLogging();
				app.MapOpenApi();
				app.MapScalarApiReference();
				//app.UseDeveloperExceptionPage();
				await app.SeedRolesAndUsersAsync(app.Services.GetRequiredService<IConfiguration>());
			}
			if (!app.Environment.IsDevelopment())
			{
				app.UseHsts();
			}

			app.UseExceptionHandler();

			app.UseHttpsRedirection();

			if (app.Environment.IsDevelopment())
			{
			app.UseCors("CorsDev");
			}
			else
			{
				app.UseCors("CorsProd");
			}

			app.UseAuthentication();

			app.UseAuthorization();

			var account = app.MapGroup("/account");

			account.MapIdentityApi<User>();
			account.MapIdentityCustomApi<User>();

			app.MapControllers();

			#endregion

			app.Run();
		}
	}
}
