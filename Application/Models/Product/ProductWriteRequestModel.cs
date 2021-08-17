using Domain.Enums;

namespace Application.Models.Product
{
    public class ProductWriteRequestModel
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public long StockQuantity { get; set; }
        public EntityStatusType Status { get; set; }
    }
}
