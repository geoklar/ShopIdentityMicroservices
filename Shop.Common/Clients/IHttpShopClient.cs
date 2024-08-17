	using Shop.Common.Models;
	namespace Shop.Common.Clients
	{
	    public interface IHttpShopClient<T>
	    {
			Task<IReadOnlyCollection<T>> GetItemsAsync(string baseURL, string requestURI);
              Task<T> GetItemAsync(string baseURL, string requestURI);
	    }
	}
