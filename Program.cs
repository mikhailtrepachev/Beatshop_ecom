using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Beatshop.Data;
using Beatshop.Models;
using Beatshop.Repositories;
using Beatshop.Interfaces;
using Amazon.S3;
using Amazon;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .WithOrigins("https://localhost:44404")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddTransient<IBeatUploadRepository, BeatUploadRepository>();

//Configure AWS client from env. json file
builder.Configuration.AddJsonFile("awsSettings.json", optional: true);

var configuration = builder.Configuration.GetSection("AWS");
var accessKey = configuration["AccessKey"];
var secretKey = configuration["SecretKey"];

builder.Services.AddSingleton<AmazonS3Client>(s => new AmazonS3Client(accessKey, secretKey, RegionEndpoint.EUCentral1));

builder.Services.AddScoped<IAmazonServiceRepository, AmazonServiceRepository>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
     name: "beatupload",
     pattern: "api/beatupload",
     defaults: new { controller = "BeatUpload", action = "Post" }
)
    .RequireCors("CorsPolicy");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapFallbackToFile("index.html");

app.Run();