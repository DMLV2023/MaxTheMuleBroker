using MaxTheMuleBroker_ejemplo.Extensions;
using MaxTheMuleBroker_ejemplo.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Agrega los servicios necesarios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Condición para habilitar Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// Habilita el middleware de autenticación básica
app.UseMiddleware<BasicAuthenticationHandlerMiddleware>("MyRealm");

app.UseHttpsRedirection();
app.UseAuthorization();

// Configuración de manejo de errores (si está en tu proyecto)
app.UseErrorHandler();

app.MapControllers();

app.Run();