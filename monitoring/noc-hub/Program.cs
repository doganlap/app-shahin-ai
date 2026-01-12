using NocHub.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Register HTTP Client with long timeout for monitoring tools
builder.Services.AddHttpClient<IMonitoringToolService, MonitoringToolService>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    });

builder.Services.AddHttpClient<IMetricsService, MetricsService>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    });

builder.Services.AddHttpClient<IAlertService, AlertService>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    });

builder.Services.AddHttpClient<IProxyService, ProxyService>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    });

// Register services
builder.Services.AddScoped<IMonitoringToolService, MonitoringToolService>();
builder.Services.AddScoped<IMetricsService, MetricsService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IProxyService, ProxyService>();

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseRouting();
app.UseAuthorization();

// Map health check
app.MapHealthChecks("/health");

// Map Razor Pages
app.MapRazorPages();

// Map Controllers
app.MapControllers();

// Redirect root to dashboard
app.MapGet("/", context =>
{
    context.Response.Redirect("/dashboard");
    return Task.CompletedTask;
});

app.Run();
