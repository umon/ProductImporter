using Domain.Enums;
using System;

namespace Application.Models.Product
{
    public class ProductListItemModel
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public long StockQuantity { get; set; }
        public EntityStatusType Status { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
