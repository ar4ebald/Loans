using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Loans.Options;
using Newtonsoft.Json.Linq;

namespace Loans.Models.VK
{
    public class Client
    {
        public string Email { get; }

        private readonly string _accessToken;
        private readonly string _version;

        public Client(string accessToken, string version, string email)
        {
            _accessToken = accessToken;
            _version = version;

            Email = email;
        }

        public static async Task<Client> AuthenticateFromCode(VKSettings settings, string code)
        {
            using (var http = new HttpClient())
            {
                var response = JObject.Parse(await http.GetStringAsync(
                    $"https://oauth.vk.com/access_token" +
                    $"?client_id={settings.ClientId}" +
                    $"&client_secret={settings.SecureKey}" +
                    $"&redirect_uri={WebUtility.UrlEncode(settings.RedirectUri)}" +
                    $"&code={code}"
                ));

                return new Client(
                    response.Value<string>("access_token"),
                    settings.Version,
                    response.Value<string>("email")
                );
            }
        }

        public Task<JToken> PostAsync(string method, params (string key, object value)[] values)
        {
            return PostAsync(method, values.AsEnumerable());
        }

        public async Task<JToken> PostAsync(string method, IEnumerable<(string key, object value)> values)
        {
            using (var http = new HttpClient())
            {
                string uri = $"https://api.vk.com/method/{WebUtility.UrlEncode(method)}" +
                             $"?access_token={_accessToken}" +
                             $"&v={_version}";

                HttpContent content = new FormUrlEncodedContent(
                    values.Select(value => new KeyValuePair<string, string>(value.key, value.value.ToString()))
                );

                var response = await http.PostAsync(uri, content);

                return JToken.Parse(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
