using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Product
{
    public class ProductWriteResponse
    {
        public long InsertedCount { get; set; }
        public long UpdatedCount { get; set; }
        public long FailedCount { get; set; }
    }
}
