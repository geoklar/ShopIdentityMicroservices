	using System.ComponentModel.DataAnnotations;
	namespace Shop.Common.Models
	    {
	        public class CartItem
	        {
	           [Key]
	            public long Id { get; set; }
	    
	            public Guid UserId { get; set; }
	            public long CatalogId { get; set; }
	    
	            public int Quantity { get; set; }
	    
	            public DateTime AcquiredDate { get; set; }
	        }
	    }
