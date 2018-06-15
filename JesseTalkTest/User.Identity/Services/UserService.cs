using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace User.Identity.Services
{
    public class UserService : IUserService
    {
        private HttpClient httpClient;
        public UserService(HttpClient client)
        {
            httpClient = client;
        }
        public async Task<int> CheckOrCreate(string phone)
        {
            Dictionary<string, string> dict = new Dictionary<string, string> { { "phone", phone } };
            var baseAddress = "http://localhost:22186/";
            HttpContent content = new FormUrlEncodedContent(dict);
            var result = await httpClient.PostAsync(baseAddress + "api/users/CheckOrCreate", content);
            var userId = 0;
            var resultId =await result.Content.ReadAsStringAsync();
            if (result.StatusCode== System.Net.HttpStatusCode.OK)
            {
                int.TryParse(resultId,out userId);
            }
            return userId;
        }
    }
}
