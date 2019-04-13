using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CallChecker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CallChecker.Controllers
{
    [ApiController]
    public class WildcardController : ControllerBase
    {
        private IHttpContextAccessor _httpContextAccessor;

        private static ConcurrentDictionary<string, List<string>> _requests = new ConcurrentDictionary<string, List<string>>();

        public WildcardController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // GET api/values
        [Route("/getcalls")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetCalls()
        {

            var groupedRequests = _requests.Select(v => new
            {
                verb = v.Key,
                entries = v.Value.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count())
            });
            
            return new JsonResult(groupedRequests);
        }

        // GET api/values
        [Route("{*url}")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            var requests = _requests.GetOrAdd("GET", verb => new List<string>());
            requests.Add(path);

            return new string[] { path };
        }

        // POST api/values
        [Route("{*url}")]
        [HttpPost]
        public ActionResult Post()
        {
            try
            {
                var sr = new StreamReader(_httpContextAccessor.HttpContext.Request.Body);
                var body = sr.ReadToEnd();

                var webhook = Newtonsoft.Json.JsonConvert.DeserializeObject<AzureEventGridWebHookValidation[]>(body);
                if (webhook != null && !string.IsNullOrEmpty(webhook[0].data.validationCode))
                {
                    return new JsonResult(new
                    {
                        validationResponse = webhook[0].data.validationCode
                    });
                }
            }
            catch (Exception)
            {
                // eat the exception
            }

            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            var requests = _requests.GetOrAdd("POST", verb => new List<string>());
            requests.Add(path);

            return new EmptyResult();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        [Route("{*url}")]
        public void Put(int id, [FromBody] string value)
        {
            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            var requests = _requests.GetOrAdd("PUT", verb => new List<string>());
            requests.Add(path);

        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [Route("{*url}")]
        public void Delete(int id)
        {
            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            var requests = _requests.GetOrAdd("DELETE", verb => new List<string>());
            requests.Add(path);

        }
    }
}
