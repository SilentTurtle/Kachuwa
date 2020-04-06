using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kachuwa.Data.Crud.Attribute;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Kachuwa.Web.Service
{
    public class ExportService : IExportService
    {
        private List<string> _headers = new List<string>();
        private string _sheetName;
        private string _fileName;
        private List<string> _type = new List<string>();
        private IWorkbook _workbook;
        private ISheet _sheet;

        public ExportService()
        {

        }

        private const string DefaultSheetName = "Sheet1";

        public Task<HttpResponseMessage> Export<T>(List<T> dataSources, string fileName, string sheetName = DefaultSheetName)
        {
            _headers = new List<string>();
            _type = new List<string>();
            #region Creating SpreadSheet
            _fileName = fileName;
            _sheetName = sheetName;

            _workbook = new XSSFWorkbook(); //Creating New Excel object
            _sheet = _workbook.CreateSheet(_sheetName); //Creating New Excel Sheet object

            var headerStyle = _workbook.CreateCellStyle(); //Formatting
            var headerFont = _workbook.CreateFont();
            headerFont.IsBold = true;
            headerStyle.SetFont(headerFont);
            #endregion
            #region Data Binding
            var properties = typeof(T).GetProperties();
            DataTable table = new DataTable();
            foreach (PropertyInfo prop in properties)
            {
                var propAttributes = prop.GetCustomAttributes();
                if (propAttributes.Any(attr => attr.GetType().Name == typeof(ReportHideAttribute).Name || attr.GetType().Name == typeof(ReportHideAttribute).Name))
                    continue;

                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var customAttribute = prop.GetCustomAttribute<DateFormatAttribute>();
                bool _isCustomType = false;
                if (customAttribute != null)
                {
                    //customAttribute.DataFormatString
                    _type.Add("String");
                    _isCustomType = true;
                }
                else
                {
                    _type.Add(type.Name);
                }

                var datamember = prop.GetCustomAttribute<DataMemberAttribute>();
                table.Columns.Add(prop.Name, _isCustomType ? typeof(string) : type);
                string name = Regex.Replace(prop.Name, "([A-Z])", " $1").Trim(); //space separated 
                //name by caps for header
                if (datamember != null)
                    _headers.Add(datamember.Name);
                else
                {
                    _headers.Add(name);
                }
            }

            foreach (T item in dataSources)
            {
                DataRow row = table.NewRow();
                foreach (var prop in properties)
                {
                    if (prop.GetCustomAttributes(true).Any(attr =>
                        attr.GetType().Name == typeof(ReportHideAttribute).Name ||
                        attr.GetType().Name == typeof(ReportHideAttribute).Name))
                        continue;
                    var customAttribute = prop.GetCustomAttribute<DateFormatAttribute>();
                    if (customAttribute != null)
                    {

                        var propValue = prop.GetValue(item) ?? DBNull.Value;
                        if (propValue != null)
                        {
                            string propValueRes = Convert.ToDateTime(propValue).ToString(customAttribute.DateFormatter);
                            row[prop.Name] = propValueRes;
                        }
                        else
                        {
                            row[prop.Name] = string.Empty;
                        }
                    }
                    else
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }

                }

                table.Rows.Add(row);
            }

            IRow sheetRow = null;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                sheetRow = _sheet.CreateRow(i + 1);
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    ICell Row1 = sheetRow.CreateCell(j);

                   // string type = _type[j].ToLower();
                   
                    var currentCellValue = table.Rows[i][j];
                    string type = currentCellValue.GetType().Name.ToLower();
                    if (currentCellValue != null &&
                        !string.IsNullOrEmpty(Convert.ToString(currentCellValue)))
                    {
                        if (type == "string")
                        {
                            Row1.SetCellValue(Convert.ToString(currentCellValue));
                        }
                        else if (type == "int32")
                        {
                            Row1.SetCellValue(Convert.ToInt32(currentCellValue));
                        }
                        else if (type == "double")
                        {
                            Row1.SetCellValue(Convert.ToDouble(currentCellValue));
                        }
                        else if (type == "boolean")
                        {
                            Row1.SetCellValue(Convert.ToString(currentCellValue));
                        }
                        else if (type == "datetime")
                        {
                            Row1.SetCellValue(Convert.ToString(currentCellValue));
                        }
                        else
                        {

                            Row1.SetCellValue(Convert.ToString(currentCellValue));
                        }
                    }
                    else
                    {
                        Row1.SetCellValue(string.Empty);
                    }
                }

            }
            #endregion

            //Header
            var header = _sheet.CreateRow(0);
            for (var i = 0; i < _headers.Count; i++)
            {
                var cell = header.CreateCell(i);
                cell.SetCellValue(_headers[i]);
                cell.CellStyle = headerStyle;
            }

            for (var i = 0; i < _headers.Count; i++)
            {
                _sheet.AutoSizeColumn(i);
            }

            using (var memoryStream = new MemoryStream()) //creating memoryStream
            {
                _workbook.Write(memoryStream);
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(memoryStream.ToArray())
                };

                response.Content.Headers.ContentType = new MediaTypeHeaderValue
                    ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                response.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = $"{_fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx"
                    };

                return Task.FromResult(response);
            }
        }
    }
}