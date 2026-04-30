using GpdWebApi.Extensions;
using GpdWebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── Services ─────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Custom extension methods (see Extensions/ServiceExtensions.cs)
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwt();

// ── Build app ─────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────────────

// Global exception handler — always first
app.UseMiddleware<ExceptionMiddleware>();

/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GPD Web API v1");
        c.RoutePrefix = string.Empty;   // Swagger UI at root: https://localhost:port/
    });
}*/
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GPD Web API v1");
    c.RoutePrefix = string.Empty;
});
//app.UseHttpsRedirection();

// Order matters: Authentication THEN Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
