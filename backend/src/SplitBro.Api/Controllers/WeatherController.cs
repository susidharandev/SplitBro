using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace SplitBro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private static Dictionary<string, int> _weather = new Dictionary<string, int> { { "chennai", 25 }, { "mumbai", 27 } };


        [HttpGet("getall")]
        public ActionResult getAllWeather()
        {
            _weather.Take(5).Skip(0);
            return Ok(JsonSerializer.Serialize(_weather));
        }

        [HttpGet("get/{location}")]
        public ActionResult getWeather (string location)
        {
            location = location.Trim().ToLower();

            int result;
            _weather.TryGetValue(location, out result);

            return Ok($" {location} : {result} Degree Celcius");
        }

        [HttpPost("post/{location}/{data}")]
        public ActionResult PostWeather(string location, int data)
        {
            location = location.Trim().ToLower();

            _weather.Add(location, data);
            return Ok($"{location} {data} Weather Added");
        }

        [HttpPut("put/{location}/{data}")] // update ifexist, else create
        public ActionResult PutWeather(string location, int data)
        {
            location = location.Trim().ToLower();

            int result;
            _weather.TryGetValue(location, out result);
            if (result == 0)
            {
                _weather.Add(location, data);
                return Ok($"{location} = {data} Weather Added as not found");
            }
            else
            {
                _weather[location] = data;
                return Ok($"{location} = {data} Weather Updated");
            }
        }

        [HttpPatch("patch/{location}/{data}")] // only updates
        public ActionResult PatchWeather(string location, int data)
        {
            location = location.Trim().ToLower();

            _weather[location] = data;
            return Ok($"{location} = {data} Weather Updated");
        }

        [HttpDelete("delete/{location}")]
        public ActionResult DeleteWeather(string location)
        {
            location = location.Trim().ToLower();

            _weather.Remove(location);
            return Ok($"{location} Weather Removed");
        }
    }
}



//API - Level Features to Implement
//To make this a true production-grade API, implement these advanced concepts:
//API Versioning: Use URL versioning (like /v1/ above) so you can practice migrating to /v2/ later without breaking current endpoints.
//Rate Limiting: Restrict anonymous users to 60 requests per minute and authenticated weather stations to 1000 requests per minute. Return standard X-RateLimit-* headers.
//Pagination, Filtering, & Sorting: On GET /api/v1/weather/reports, allow clients to filter by min_temp, sort by created_at, and paginate results (e.g., ?page=2&limit=20).
//Authentication & Authorization (RBAC): Use JWT tokens. Differentiate permissions: Guests can only read data, Spotters can post reports, and Admins can delete reports.
//Content Negotiation: Design the API to return JSON by default, but support XML if the client passes Accept: application / xml in the header.
//Idempotency Keys: For the POST endpoint, require an Idempotency-Key header to prevent a user from submitting the exact same weather report twice due to a laggy connection.
//CORS & Secure Headers: Configure Cross-Origin Resource Sharing rules and secure your API using standard security headers.

