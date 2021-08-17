using Application.Models.Product;
using Domain.Enums;
using ExcelDataReader;
using ProductImporter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ProductImporter.Helpers
{
    public class ExcelHelper
    {
        public static ExcelProcessResult Process(Stream fileStream)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var reader = ExcelReaderFactory.CreateReader(fileStream))
            {
                var result = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    UseColumnDataType = true,
                    ConfigureDataTable = (table) => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                });

                if (!TableControl(result))
                    return new ExcelProcessResult("Excel dosyası içerisinde 'Ürünler' sayfası bulunamadı."); // Excel tablosu bulunamadı

                var dt = result.Tables["Ürünler"];
                if (!ColumnControl(dt.Columns))
                    return new ExcelProcessResult("Excel dosyasındaki sütunlar, beklenilen şablon ile uyuşmuyor."); // Excel kolon kontrolü

                var dtRows = dt.Rows;
                var brokenRows = new List<ExcelFaultProcessResult>();
                var productList = new List<ProductWriteRequestModel>();

                for (int i = 0; i < dtRows.Count; i++)
                {
                    var row = dtRows[i];
                    var rowControlResults = RowControl(row);
                    if (string.IsNullOrEmpty(rowControlResults))
                    {
                        productList.Add(new ProductWriteRequestModel
                        {
                            Barcode = row.Field<string>("Barkod"),
                            Description = row.Field<string>("Ürün Açıklaması"),
                            Name = row.Field<string>("Ürün Adı"),
                            CategoryName = row.Field<string>("Kategori"),
                            Price = (decimal)row.Field<double>("Fiyat"),
                            StockQuantity = (long)row.Field<double>("Stok Adedi"),
                            Status = row.Field<string>("Ürün Durumu").Equals("Aktif",StringComparison.InvariantCultureIgnoreCase) ? EntityStatusType.Active : EntityStatusType.Passive
                        });
                    }
                    else
                    {
                        brokenRows.Add(new ExcelFaultProcessResult { RowNumber = (i + 1), Error = rowControlResults });
                    }
                }

                return new ExcelProcessResult
                {
                    ProceedRequests = productList,
                    FailedRequests = brokenRows,
                    Succeed = true
                };
            }
        }

        private static string RowControl(DataRow row)
        {
            var results = new List<string>();

            if (string.IsNullOrEmpty(row.Field<string>("Barkod")))
                results.Add("Barkod değeri belirtilmedi");

            if (string.IsNullOrEmpty(row.Field<string>("Ürün Adı")))
                results.Add("Ürün Adı belirtilmedi");

            if (string.IsNullOrEmpty(row.Field<string>("Ürün Açıklaması")))
                results.Add("Ürün Açıklaması belirtilmedi");

            if (row.Field<double?>("Fiyat") == null)
                results.Add("Fiyat belirtilmedi");

            if (string.IsNullOrEmpty(row.Field<string>("Kategori")))
                results.Add("Kategori belirtilmedi");

            if (row.Field<double?>("Stok Adedi") == null)
                results.Add("Stok Adedi belirtilmedi");

            return string.Join(',', results);
        }

        private static bool ColumnControl(DataColumnCollection columns)
        {
            var validColumnNames = new List<string> { "Barkod", "Ürün Adı", "Ürün Açıklaması", "Fiyat", "Kategori", "Stok Adedi" };

            foreach (var columnName in validColumnNames)
            {
                if (!columns.Contains(columnName)) return false;
            }

            return true;
        }

        private static bool TableControl(DataSet result)
        {
            return result.Tables.Contains("Ürünler");
        }
    }
}
