	using System.ComponentModel.DataAnnotations.Schema;
	using Microsoft.AspNetCore.Identity;
	namespace Shop.Common.Models
	{
	    [Table("Roles")]
	    public class ApplicationRole : IdentityRole<Guid>
	    {        
	    }
}