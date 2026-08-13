using ABCRetailManagement.Services;

using Azure.Data.Tables;
var builder = WebApplication.CreateBuilder(args);

var azureStorageConnectionString =
    builder.Configuration.GetConnectionString("AzureStorage");

if (string.IsNullOrWhiteSpace(azureStorageConnectionString))
{
    throw new InvalidOperationException(
        "Azure Storage connection string 'AzureStorage' was not found.");
}

builder.Services.AddSingleton(
    new TableServiceClient(azureStorageConnectionString));

builder.Services.AddScoped<TableStorageService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
