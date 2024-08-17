namespace Shop.Common.Models
    {
        public class CatalogItem
        {
            public long Id { get; set; }
    
            public string Name { get; set; }
    
            public string Description { get; set; }
    
            public decimal Price { get; set; }
    
            public DateTime CreatedDate { get; set; }
        }
    }
