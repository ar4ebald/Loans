﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Loans.Controllers
{
    [Route("receipts")]
    public class QrController : Controller
    {
        /// <summary>
        /// Returns items from receipt
        /// </summary>
        /// <param name="param">params from receipt</param>
        /// <returns></returns>
        [HttpGet("get"), Produces("application/json")]
        public async Task<string> GetReceipt([FromQuery] QueryParam param)
        {
            string uri = $"http://brand.cash/v1/receipts/get?{WebUtility.UrlDecode(param.qr)}";

            using (var http = new HttpClient())
            {
                return await http.GetStringAsync(uri);
            }
        }
    }

    public class QueryParam
    {
        [BindRequired]
        public string qr { get; set; }
    }
}
