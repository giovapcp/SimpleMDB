namespace Smdb.Api.Movies;

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Shared.Http;
using Smdb.Core.Movies;

public class MoviesController
{
    private readonly IMovieService movieService;

    // Constructor obligatorio: inyecta IMovieService desde el bootstrap
    public MoviesController(IMovieService movieService)
    {
        this.movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
    }

    // ---------------------------------------------------------
    // GET /api/v1/movies?page=1&size=10
    // ---------------------------------------------------------
    public async Task ReadMovies(HttpListenerRequest req, HttpListenerResponse res,
        Hashtable props, Func<Task> next)
    {
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 9;

        var result = await movieService.ReadMovies(page, size);

        // Delega la serialización y códigos a JsonUtils
        await JsonUtils.SendPagedResultResponse(req, res, props, result, page, size);
        await next();
    }

    // ---------------------------------------------------------
    // POST /api/v1/movies
    // ---------------------------------------------------------
    public async Task CreateMovie(HttpListenerRequest req,
        HttpListenerResponse res, Hashtable props, Func<Task> next)
    {
        var text = props["req.text"] as string;
        if (string.IsNullOrWhiteSpace(text))
        {
            res.StatusCode = (int)HttpStatusCode.BadRequest;
            await JsonUtils.SendResultResponse(req, res, props, new Shared.Http.Result<object>(new Exception("Empty request body"), (int)HttpStatusCode.BadRequest));
            return;
        }

        Movie? movie;
        try
        {
            movie = JsonSerializer.Deserialize<Movie>(text, JsonSerializerOptions.Web);
        }
        catch (Exception ex)
        {
            res.StatusCode = (int)HttpStatusCode.BadRequest;
            await JsonUtils.SendResultResponse(req, res, props, new Shared.Http.Result<object>(ex, (int)HttpStatusCode.BadRequest));
            return;
        }

        if (movie == null)
        {
            res.StatusCode = (int)HttpStatusCode.BadRequest;
            await JsonUtils.SendResultResponse(req, res, props, new Shared.Http.Result<object>(new Exception("Invalid movie payload"), (int)HttpStatusCode.BadRequest));
            return;
        }

        var result = await movieService.CreateMovie(movie);
        await JsonUtils.SendResultResponse(req, res, props, result);
        await next();
    }

    // ---------------------------------------------------------
    // GET /api/v1/movies/:id
    // ---------------------------------------------------------
    public async Task ReadMovie(HttpListenerRequest req, HttpListenerResponse res,
        Hashtable props, Func<Task> next)
    {
        var uParams = props["req.params"] as NameValueCollection;
        if (uParams == null)
        {
            res.StatusCode = (int)HttpStatusCode.BadRequest;
            await JsonUtils.SendResultResponse(req, res, props, new Shared.Http.Result<object>(new Exception("Missing route parameters"), (int)HttpStatusCode.BadRequest));
            return;
        }

        if (!int.TryParse(uParams["id"], out int id) || id < 0)
        {
            res.StatusCode = (int)HttpStatusCode.BadRequest;
            await JsonUtils.SendResultResponse(req, res, props, new Shared.Http.Result<object>(new Exception("Invalid id parameter"), (int)HttpStatusCode.BadRequest));
            return;
        }

        var result = await movieService.ReadMovie(id);
        await JsonUtils.SendResultResponse(req, res, props, result);
        await next();
    }

    // ---------------------------------------------------------
    // PUT /api/v1/movies/:id
    // ---------------------------------------------------------
    public async Task UpdateMovie(HttpListenerRequest req,
        HttpListenerResponse res, Hashtable props, Func<Task> next)
    {
        var uParams = props["req.params"] as NameValueCollection;
        if (uParams == null)
        {
            res.StatusCode = (int)HttpStatusCode.BadRequest;
            await JsonUtils.SendResultResponse(req, res, props, new Shared.Http.Result<object>(new Exception("Missing route parameters"), (int)HttpStatusCode.BadRequest));
            return;
        }

        if (!int.TryParse(uParams["id"], out int id) || id < 0)
        {
            res.StatusCode = (int)HttpStatusCode.BadRequest;
            await JsonUtils.SendResultResponse(req, res, props, new Shared.Http.Result<object>(new Exception("Invalid id parameter"), (int)HttpStatusCode.BadRequest));
            return;
        }

        var text = props["req.text"] as string;
        if (string.IsNullOrWhiteSpace(text))
        {
            res.StatusCode = (int)HttpStatusCode.BadRequest;
            await JsonUtils.SendResultResponse(req, res, props, new Shared.Http.Result<object>(new Exception("Empty request body"), (int)HttpStatusCode.BadRequest));
            return;
        }

        Movie? movie;
        try
        {
            movie = JsonSerializer.Deserialize<Movie>(text, JsonSerializerOptions.Web);
        }
        catch (Exception ex)
        {
            res.StatusCode = (int)HttpStatusCode.BadRequest;
            await JsonUtils.SendResultResponse(req, res, props, new Shared.Http.Result<object>(ex, (int)HttpStatusCode.BadRequest));
            return;
        }

        if (movie == null)
        {
            res.StatusCode = (int)HttpStatusCode.BadRequest;
            await JsonUtils.SendResultResponse(req, res, props, new Shared.Http.Result<object>(new Exception("Invalid movie payload"), (int)HttpStatusCode.BadRequest));
            return;
        }

        var result = await movieService.UpdateMovie(id, movie);
        await JsonUtils.SendResultResponse(req, res, props, result);
        await next();
    }

    // ---------------------------------------------------------
    // DELETE /api/v1/movies/:id
    // ---------------------------------------------------------
    public async Task DeleteMovie(HttpListenerRequest req,
        HttpListenerResponse res, Hashtable props, Func<Task> next)
    {
        var uParams = props["req.params"] as NameValueCollection;
        if (uParams == null)
        {
            res.StatusCode = (int)HttpStatusCode.BadRequest;
            await JsonUtils.SendResultResponse(req, res, props, new Shared.Http.Result<object>(new Exception("Missing route parameters"), (int)HttpStatusCode.BadRequest));
            return;
        }

        if (!int.TryParse(uParams["id"], out int id) || id < 0)
        {
            res.StatusCode = (int)HttpStatusCode.BadRequest;
            await JsonUtils.SendResultResponse(req, res, props, new Shared.Http.Result<object>(new Exception("Invalid id parameter"), (int)HttpStatusCode.BadRequest));
            return;
        }

        var result = await movieService.DeleteMovie(id);
        await JsonUtils.SendResultResponse(req, res, props, result);
        await next();
    }
}
// READ MOVIES-------------------------
// curl -X GET "http://localhost:8080/api/v1/movies?page=1&size=10" >>
// FUNCIONA ---------------------------

// CREATE MOVIE------------------------
// curl -X POST "http://localhost:8080/api/v1/movies" -H "Content-Type:application/json" -d "{ \"id\": -1, \"title\": \"Inception\", \"year\": 2010,\"description\": \"A skilled thief who enters dreams to steal secrets.\" }"
// FUNCIONA----------------------------

// READ MOVIE--------------------------
// curl -X GET "http://localhost:8080/api/v1/movies/1"
// FUNCIONA----------------------------

// UPDATE MOVIE------------------------
// curl -X PUT "http://localhost:8080/api/v1/movies/1" -H "Content-Type:application/json" -d "{ \"title\": \"Joker 2\", \"year\": 2020, \"description\":\"A man that is a joke.\" }"
// FUNCIONA----------------------------

// DELETE MOVIE------------------------
// curl -X DELETE http://localhost:8080/api/v1/movies/1
// FUNCIONA----------------------------

