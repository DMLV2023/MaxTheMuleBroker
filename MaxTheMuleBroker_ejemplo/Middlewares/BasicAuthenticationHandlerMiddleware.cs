using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace MaxTheMuleBroker_ejemplo.Middlewares
{
	public class BasicAuthenticationHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly string _realm;

		public BasicAuthenticationHandlerMiddleware(RequestDelegate next, string realm)
		{
			_next = next;
			_realm = realm;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			// Verifica si el encabezado "Authorization" existe
			if (!context.Request.Headers.ContainsKey("Authorization"))
			{
				context.Response.StatusCode = 401; // Unauthorized
				context.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{_realm}\"";
				await context.Response.WriteAsync("Unauthorized");
				return;
			}

			try
			{
				// Extrae y decodifica las credenciales
				var header = context.Request.Headers["Authorization"].ToString();
				var encodedCreds = header.Substring("Basic ".Length).Trim();
				var creds = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCreds)).Split(':');

				if (creds.Length != 2)
				{
					throw new FormatException("Invalid Authorization header format.");
				}

				var username = creds[0];
				var password = creds[1];

				// Valida las credenciales (usa valores seguros en producción)
				if (username != "diego123" || password != "diego321")
				{
					context.Response.StatusCode = 401; // Unauthorized
					context.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{_realm}\"";
					await context.Response.WriteAsync("Unauthorized");
					return;
				}

				// Si la autenticación es correcta, pasa al siguiente middleware
				await _next(context);
			}
			catch (FormatException)
			{
				context.Response.StatusCode = 400; // Bad Request
				await context.Response.WriteAsync("Invalid Authorization header format.");
			}
		}
	}
}