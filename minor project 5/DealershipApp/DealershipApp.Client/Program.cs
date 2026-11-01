// Hosts the static HTML client in wwwroot (index.html, app.js, styles.css).
// It runs on http://localhost:5090, a different origin from the API (port 5080),
// so the browser enforces CORS exactly as it would for a real front end.
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
