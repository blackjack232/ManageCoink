
using UserManage.Application.Interface.Repository;
using UserManage.Application.Interface.Service;
using UserManage.Application.Services;
using UserManage.Domain.Constants;
using UserManage.Infrastructure.Data;
using UserManage.Infrastructure.Helpers;
using UserManage.Infrastructure.Interface;
using UserManage.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "User Management API",
        Version = "v1",
        Description = "API para gestión de usuarios con PostgreSQL"
    });
});
var encodedConnectionString = builder.Configuration.GetConnectionString("CoinkDb")
    ?? throw new InvalidOperationException(
        string.Format(InfrastructureMessages.ConnectionStringNotConfigured, "CoinkDb")
    );

var connectionString = ConnectionStringDecoder.Decode(encodedConnectionString, Console.WriteLine);

builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
    new PostgresConnectionFactory(connectionString));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IRegionService, RegionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
