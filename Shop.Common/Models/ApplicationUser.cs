	using Microsoft.AspNetCore.Identity;
	namespace Shop.Common.Models
	{
	    public class ApplicationUser : IdentityUser<Guid>
	    {
	        public decimal Budget { get; set; }
	    }
	}
