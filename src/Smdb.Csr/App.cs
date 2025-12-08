namespace Smdb.Csr;

using Shared.Http;
public class App : HttpServer
{
    public App()
    {
        
    }
    public override void Init()
    {
        router.Use(HttpUtils.StructuredLogging);
        router.Use(HttpUtils.CentralizedErrorHandling);
        router.Use(HttpUtils.AddResponseCorsHeaders);
        router.Use(HttpUtils.DefaultResponse);
        router.Use(HttpUtils.ParseRequestUrl);
        router.Use(HttpUtils.ParseRequestQueryString);
        router.Use(HttpUtils.ServeStaticFiles);
        router.UseSimpleRouteMatching();

        router.MapGet("/", LandingPageIndexRedirect);
        router.MapGet("/movies", MoviesPageIndexRedirect);
    }

    public static async Task LandingPageIndexRedirect(HttpListnerRequest req, HttpListnerResponse res, Hashtable props, Func<Task> next)
    {
        res.Redirect("/index.html"); 
        await next();
    }

    public static async Task MoviesPageIndexRedirect(HttpListnerRequest req, HttpListnerResponse res, Hashtable props, Func<Task> next)
    {
        res.Redirect("/movies/index.html"); 
        await next();
    }
}