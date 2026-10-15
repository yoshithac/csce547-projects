using MusicStore.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton(new Database(builder.Configuration.GetConnectionString("MusicDb")));
builder.Services.AddScoped<ArtistRepo>();
builder.Services.AddScoped<AlbumRepo>();
builder.Services.AddScoped<CustomerRepo>();

var app = builder.Build();
app.UseRouting();
app.MapRazorPages();
app.Run();
