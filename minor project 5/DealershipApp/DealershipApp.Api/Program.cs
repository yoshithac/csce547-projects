using DealershipApp.Business;
using DealershipApp.Data;
using DealershipApp.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Data layer: one connection factory, one repository per table.
// ---------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DealershipDb")
    ?? throw new InvalidOperationException(
        "Connection string 'DealershipDb' is missing. Add it to appsettings.json.");

builder.Services.AddSingleton<IDbConnectionFactory>(new SqlConnectionFactory(connectionString));
builder.Services.AddScoped<IDealershipRepository, DealershipRepository>();
builder.Services.AddScoped<ISalespersonRepository, SalespersonRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

// ---------------------------------------------------------------------------
// Business layer: one manager per table.
// ---------------------------------------------------------------------------
builder.Services.AddScoped<DealershipManager>();
builder.Services.AddScoped<SalespersonManager>();
builder.Services.AddScoped<VehicleManager>();

// ---------------------------------------------------------------------------
// Service layer: controllers + Swagger page for trying the endpoints.
// ---------------------------------------------------------------------------
builder.Services.AddControllers(options =>
{
    // Validation rules live in the business layer, so don't let MVC add its own
    // "field is required" checks for non-nullable strings first.
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------------------------------------------------------------------------
// CORS: the HTML client runs on a different port (a different "origin"),
// so the browser blocks its requests unless the API names that origin here.
// The list lives in appsettings.json under Cors:AllowedOrigins.
// ---------------------------------------------------------------------------
const string ClientCorsPolicy = "ClientCorsPolicy";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy(ClientCorsPolicy, policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// Swagger is on in every environment so the running service is easy to see and test.
app.UseSwagger();
app.UseSwaggerUI();

// UseCors must come before the endpoints (MapControllers) so the policy applies to them.
// UseHttpsRedirection() is left out on purpose: if the API redirected the client's http:// requests
// to https://, any request that needs a CORS preflight would fail (browsers don't follow redirects
// on preflights), and https calls fail anyway when the development certificate isn't trusted.
app.UseCors(ClientCorsPolicy);

// Turn unexpected errors (for example, SQL Server not reachable) into a JSON 500 response
// that the client can display, instead of the developer error page's plain-text stack trace.
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex) when (!context.Response.HasStarted)
    {
        app.Logger.LogError(ex, "Unhandled error for {Method} {Path}", context.Request.Method, context.Request.Path);

        await Results.Problem(
                title: "The server hit an error.",
                detail: app.Environment.IsDevelopment() ? ex.Message : "See the server log for details.",
                statusCode: StatusCodes.Status500InternalServerError)
            .ExecuteAsync(context);
    }
});

// Opening the base URL in a browser goes straight to the Swagger page.
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

app.Run();
