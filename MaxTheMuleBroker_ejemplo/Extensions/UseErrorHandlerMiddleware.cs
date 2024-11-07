using MaxTheMuleBroker_ejemplo.Middlewares;

namespace MaxTheMuleBroker_ejemplo.Extensions
{
	public static class UseErrorHandlerMiddleware
	{
		public static void UseErrorHandler(this IApplicationBuilder app)
		{
			app.UseMiddleware<ErrorHandlerMiddleware>();
		}
	}
}
