using Application.Models.Product;
using System.Collections.Generic;

namespace ProductImporter.Models
{
    public class ExcelProcessResult
    {
        public List<ProductWriteRequestModel> ProceedRequests { get; set; }
        public List<ExcelFaultProcessResult> FailedRequests { get; set; }
        public bool Succeed { get; set; }
        public string ErrorMessage { get; set; }

        public ExcelProcessResult()
        {

        }

        public ExcelProcessResult(string errorMessage)
        {
            Succeed = false;
            ErrorMessage = errorMessage;
        }
    }
}
