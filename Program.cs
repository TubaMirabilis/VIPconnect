using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectX.Data;
using ProjectX.Hubs;
using ProjectX.Models;
using Tweetinvi;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables(prefix: "PROJECTX_");
// builder.Services.Configure<CookiePolicyOptions>(options =>
// {
//     options.MinimumSameSitePolicy = SameSiteMode.Lax;
// });
var connectionstring = builder.Configuration["Authentication:AzureSQL:ConnectionString"];
var client = new TwitterClient(builder.Configuration["Authentication:Twitter:ConsumerAPIKey"], builder.Configuration["Authentication:Twitter:ConsumerSecret"]);
await client.Auth.InitializeClientBearerTokenAsync();
if(builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(connectionstring);
    });
}
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ ";
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddSignInManager<MySignInManager>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/account/google-login";
})
.AddGoogle(options =>
{
    IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleAuthNSection["ClientId"];
    options.ClientSecret = googleAuthNSection["ClientSecret"];
})
.AddTwitter(options =>
{
    options.ConsumerKey = builder.Configuration["Authentication:Twitter:ConsumerAPIKey"];
    options.ConsumerSecret = builder.Configuration["Authentication:Twitter:ConsumerSecret"];
});
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<TwitterClient>(client);

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    IConfigurationSection adminAuthNSection = builder.Configuration.GetSection("Authentication:ProjectXAdmin");
    var testUserPw = adminAuthNSection["Password"];
    await SeedData.Initialize(services, testUserPw);
}
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
    app.UseHsts();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});
app.MapHub<LikesWidgetHub>("/likeHub");
app.Run();