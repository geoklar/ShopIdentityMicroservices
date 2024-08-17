using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
namespace Shop.Common.Clients
	{
	    public class HttpShopClient<T>: IHttpShopClient<T>
	    {
	        private readonly HttpClient _httpClient;
	        private readonly IHttpContextAccessor _httpContextAccessor;
	        public HttpShopClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
	        {
	            _httpContextAccessor = httpContextAccessor;
	            _httpClient = httpClient;
	        }
	        public async Task<IReadOnlyCollection<T>> GetItemsAsync(string baseURL, string requestURI)
	        {
	            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
	            _httpClient.BaseAddress = new Uri(baseURL);
	            if (!string.IsNullOrEmpty(token))
	            {
	                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Split(" ").Last());
	            }
	            var items = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<T>>(requestURI);
	            return items;
	        }
	
	        public async Task<T> GetItemAsync(string baseURL, string requestURI)
	        {
	            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
	            _httpClient.BaseAddress = new Uri(baseURL);
	            if (!string.IsNullOrEmpty(token))
	            {
	                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Split(" ").Last());
	            }
	            var item = await _httpClient.GetFromJsonAsync<T>(requestURI);
	            return item;
	        }
	    }
	}
