using ABCRetailManagement.Services;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Files.Shares;

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

builder.Services.AddSingleton(
    new BlobServiceClient(azureStorageConnectionString));

builder.Services.AddSingleton(
    new QueueServiceClient(azureStorageConnectionString));

builder.Services.AddSingleton(
    new ShareServiceClient(azureStorageConnectionString));

builder.Services.AddScoped<TableStorageService>();
builder.Services.AddScoped<BlobStorageService>();
builder.Services.AddScoped<QueueStorageService>();
builder.Services.AddScoped<OrderProcessingService>();
builder.Services.AddScoped<FileStorageService>();

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
