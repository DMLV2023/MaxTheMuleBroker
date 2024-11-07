using MaxTheMuleBroker_ejemplo.Extensions;
using MaxTheMuleBroker_ejemplo.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Agrega los servicios necesarios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Condici�n para habilitar Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// Habilita el middleware de autenticaci�n b�sica
app.UseMiddleware<BasicAuthenticationHandlerMiddleware>("MyRealm");

app.UseHttpsRedirection();
app.UseAuthorization();

// Configuraci�n de manejo de errores (si est� en tu proyecto)
app.UseErrorHandler();

app.MapControllers();

app.Run();