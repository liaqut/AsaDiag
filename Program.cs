using Blazored.LocalStorage;
using Blazored.SessionStorage;
using DigiEquipSys.Components;
using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Services;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddDbContext<BASS_DBContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"), 
    sqlServerOptionsAction: sqlOptions => { sqlOptions.CommandTimeout(300); sqlOptions.EnableRetryOnFailure();}), ServiceLifetime.Transient);   

	//builder.Services.AddDbContext<BASS_DBContext>(options =>
	//{
	//	options.UseSqlServer(builder.configuration.GetConnectionString("DbConnection"))
	//		   .EnableSensitiveDataLogging()
	//		   .LogTo(Console.WriteLine, LogLevel.Information); // 👀
	//});
builder.Services.AddHttpClient<ZohoApiService>();
builder.Services.AddScoped<AccYear>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IMenuInfo, MenuInfoManager>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IGenCountryService, GenCountryService>();
builder.Services.AddScoped<IGenCityService, GenCityService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IItemMasterService, ItemMasterService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IGroupMasterService, GroupMasterService>();
builder.Services.AddScoped<IItemUnitService, ItemUnitService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IRcptHeadService, RcptHeadService>();
builder.Services.AddScoped<IRcptDetailService, RcptDetailService>();
builder.Services.AddScoped<IDelHeadService, DelHeadService>();
builder.Services.AddScoped<IDelDetlService, DelDetlService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ISystemPagesService, SystemPagesService>();
builder.Services.AddScoped<ISysPagesControlService, SysPagesControlService>();
builder.Services.AddScoped<IGenScanSpecService, GenScanSpecService>();
builder.Services.AddScoped<ICatMasterService, CatMasterService>();
builder.Services.AddScoped<ISDelHeadService, SDelHeadService>();
builder.Services.AddScoped<ISDelDetlService, SDelDetlService>();
builder.Services.AddScoped<ITrHeadService, TrHeadService>();
builder.Services.AddScoped<ITrDetailService, TrDetailService>();
builder.Services.AddScoped<IClientCityService, ClientCityService>();
builder.Services.AddScoped<IvwReceiptService, vwReceiptService>();
builder.Services.AddScoped<IvwSaleService, vwSaleService>();
builder.Services.AddScoped<IvwTransferService, vwTransferService>();
builder.Services.AddScoped<IStockTransService, StockTransService>();
builder.Services.AddScoped<IStockCheckService, StockCheckService>();
builder.Services.AddScoped<ICommChargeService, CommChargeService>();
builder.Services.AddScoped<IGenCurrencyService, GenCurrencyService>();
builder.Services.AddScoped<IPoHeadService, PoHeadService>();
builder.Services.AddScoped<IPoDetailService, PoDetailService>();
builder.Services.AddScoped<IDivisionService, DivisionService>();
//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF5cWWtCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXZfeXVUQmddV0J1W0Y=");
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo + DSMBMAY9C3t3VVhhQlJDfV5AQmBIYVp / TGpJfl96cVxMZVVBJAtUQF1hTH5VdEBjUH1dc3VWR2dYWkd2; MzkyMDEyNEAzMzMwMmUzMDJlMzAzYjMzMzAzYmdXY1pMMGo2Q1ZPakJ6NHlIakxyZXJSZzFIYmlQd0QweExTaXU5WU1pZ3M9");
//Bold.Licensing.BoldLicenseProvider.RegisterLicense("XnGYLTns3kvQ4EoGpULefBGMGrFShskNKxQy0Rm63rM=");
builder.Services.AddControllersWithViews();

builder.Services.AddSyncfusionBlazor();

builder.Services.AddServerSideBlazor().AddCircuitOptions(options => {
    options.DetailedErrors = false;
});
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<ZohoTokenService>();

//builder.Services.AddServerSideBlazor()
//    .AddHubOptions(o =>
//    {
//        o.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
//        o.HandshakeTimeout = TimeSpan.FromMinutes(2);
//        o.KeepAliveInterval = TimeSpan.FromMinutes(2);
//    });


var app = builder.Build();

//app.MapGet("/zohoauthcallback", async (HttpContext context, ZohoTokenService tokenService) =>
//{
//    var code = context.Request.Query["code"].ToString();
//    if (string.IsNullOrEmpty(code))
//    {
//        return Results.BadRequest("Authorization code is missing.");
//    }

//    var accessToken = tokenService.ExchangeCodeForTokensAsync(code);

//    // For testing, show tokens
//    return Results.Json(new
//    {
//        message = "Authorization successful",
//        access_token = accessToken,
//        refresh_token = tokenService.GetAccessTokenAsync()
//    });
//});


// Global exception handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next(); // Continue to the next middleware
    }
    catch (Exception ex)
    {
        // Handle the exception (log it, show a custom page, etc.)
        Console.WriteLine($"Caught global exception: {ex.Message}");
        context.Response.Redirect("/Error");
    }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
