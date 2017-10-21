using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Loans.Controllers
{
    [Route("receipts")]
    public class QrController : Controller
    {
        [HttpGet("get")]
        public async Task<string> GetReciept([FromQuery] QueryParam param)
        {
            string uri = $"http://brand.cash/v1/receipts/get{HttpContext.Request.QueryString}";

            using (var http = new HttpClient())
            {
                return await http.GetStringAsync(uri);
            }
        }
    }

    public class QueryParam
    {
        [BindRequired]
        public string t { get; set; }

        public string s { get; set; }

        public string fn { get; set; }

        public string i { get; set; }

        public string fp { get; set; }

        public string n { get; set; }
    }
}
