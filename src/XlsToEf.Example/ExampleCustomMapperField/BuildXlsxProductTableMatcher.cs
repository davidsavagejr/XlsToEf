using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using XlsToEf.Example.Domain;
using XlsToEf.Import;

namespace XlsToEf.Example.ExampleCustomMapperField
{
    public class BuildXlsxProductTableMatcher : IAsyncRequestHandler<XlsProductColumnMatcherQuery, ImportColumnData>
    {
        private readonly IExcelIoWrapper _excelIoWrapper;

        public BuildXlsxProductTableMatcher(IExcelIoWrapper excelIoWrapper)
        {
            _excelIoWrapper = excelIoWrapper;
        }

        public async Task<ImportColumnData> Handle(XlsProductColumnMatcherQuery message)
        {
            message.FilePath = Path.GetTempPath() + message.FileName;
            var product = new Product();

            var columnData = new ImportColumnData
            {
                XlsxColumns = (await _excelIoWrapper.GetImportColumnData(message)).ToArray(),
                FileName = message.FileName,
                TableColumns = new Dictionary<string, SingleColumnData>
                {
                    {"ProductCategoryCode", new SingleColumnData("Category Code")},
                    {PropertyNameHelper.GetPropertyName(() => product.ProductName), new SingleColumnData("Product Name", required:false)},
                }
            };

            return columnData;
        }
    }
}