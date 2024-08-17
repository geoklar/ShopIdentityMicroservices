	using IdentityServer4.Models;
	namespace Play.Identity.Settings
	{
	    public class IdentityServerSettings
	    {
	        public IReadOnlyCollection<ApiScope> ApiScopes { get; set; } = Array.Empty<ApiScope>();
	        public IReadOnlyCollection<Client> Clients { get; init; }
	        public IReadOnlyCollection<IdentityResource> IdentityResources =>
	            new IdentityResource[]
	            {
	                new IdentityResources.OpenId(),
                    new IdentityResources.Profile()
                };
	    }
	}