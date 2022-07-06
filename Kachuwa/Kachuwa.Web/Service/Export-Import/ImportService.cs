using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Kachuwa.Web.Service
{
    public class ImportService : IImportService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ImportService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public List<T> Import<T>(string filePath)
        {

            string webRootPath = _hostingEnvironment.WebRootPath;
            string physicalfilePath = Path.Combine(webRootPath, filePath);
            StringBuilder sb = new StringBuilder();
            if (!File.Exists(filePath))
            {
                throw new Exception("File Not Found!");
            }
            List<T> data = new List<T>();

            string sFileExtension = Path.GetExtension(filePath).ToLower();


            using (var stream = new FileStream(physicalfilePath, FileMode.Create))
            {

                stream.Position = 0;
                if (sFileExtension == ".xls")
                {
                    HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  

                    for (int i = 0; i < hssfwb.NumberOfSheets; i++)
                    {
                        ISheet sheet = hssfwb.GetSheetAt(i); //get first sheet from workbook  

                        var sheetDatas = GetSheetData<T>(sheet);
                        data.AddRange(sheetDatas);
                    }
                }
                else
                {
                    XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                    for (int i = 0; i < hssfwb.NumberOfSheets; i++)
                    {
                        ISheet sheet = hssfwb.GetSheetAt(i); //get first sheet from workbook  

                        var sheetDatas = GetSheetData<T>(sheet);
                        data.AddRange(sheetDatas);
                    }

                }

            }

            return data;
        }


        public List<T> Import<T>(IFormFile file)
        {
            string folderName = "Upload";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string uploadDirectory = Path.Combine(webRootPath, folderName);

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            if (file.Length > 0)
            {
                List<T> data = new List<T>();
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                string fullPath = Path.Combine(uploadDirectory, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (sFileExtension == ".xls")
                    {
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  

                        for (int i = 0; i < hssfwb.NumberOfSheets; i++)
                        {
                            //  ISheet sheet = hssfwb.GetSheetAt(i); //get first sheet from workbook  

                            string sheetName = hssfwb.GetSheetAt(0).SheetName;
                            ISheet sheet = (HSSFSheet)hssfwb.GetSheet(sheetName);
                            var sheetDatas = GetSheetData<T>(sheet, false);
                            data.AddRange(sheetDatas);
                        }
                    }
                    else
                    {//.xlsx
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        for (int i = 0; i < hssfwb.NumberOfSheets; i++)
                        {
                            string sheetName = hssfwb.GetSheetAt(i).SheetName;
                            ISheet sheet = (XSSFSheet)hssfwb.GetSheet(sheetName);
                            var sheetDatas = GetSheetData<T>(sheet, true);
                            data.AddRange(sheetDatas);
                        }

                    }

                }


                return data;
            }
            else return null;
        }

        private List<T> GetSheetData<T>(ISheet sheet, bool isXls = false)
        {


            //List<T> RetVal = null;
            //var Entity = typeof(T);
            var Entity = typeof(T);
            //List<Entity> RetVal = null;
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(Entity);

            var RetVal = (List<T>)Activator.CreateInstance(constructedListType);
            // var PropDict = new Dictionary<string, PropertyInfo>();
            var PropDict = new Dictionary<string, PropertyInfo>();

            try
            {
                DataTable dt = new DataTable();
                IRow headerRow = sheet.GetRow(0);
                IEnumerator rows = sheet.GetRowEnumerator();

                int colCount = headerRow.LastCellNum;
                int rowCount = sheet.LastRowNum;

                //for (int c = 0; c < colCount; c++)
                //    dt.Columns.Add(headerRow.GetCell(c).ToString());
                var Props = Entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                PropDict = Props.ToDictionary(p => p.Name.ToUpper(), p => p);
                int counter = 0;
                while (rows.MoveNext())
                {
                    IRow row = null;
                    if (isXls)
                        row = (XSSFRow)rows.Current;
                    else row = (HSSFRow)rows.Current;
                    counter++;
                    //skip header
                    if (counter == 1)
                    {
                        continue;
                    }

                    T newObject = (T)Activator.CreateInstance(Entity);
                    // write row value
                    for (int j = 0; j < row.Cells.Count; j++)
                    {
                        var cell = row.GetCell(j);

                        if (cell != null)
                        {


                            if (PropDict.ContainsKey(headerRow.GetCell(j).ToString().Replace(" ", string.Empty).ToUpper()))
                            {
                                //getting header value
                                var Info = PropDict[headerRow.GetCell(j).ToString().Replace(" ", string.Empty).ToUpper()];
                                if ((Info != null) && Info.CanWrite)
                                {
                                    object Val = null;
                                    //Info.SetValue(newObject, (Val == DBNull.Value) ? null : Val, null);
                                    if (row.GetCell(j) != null)
                                        Val = row.GetCell(j).ToString();
                                    //switch (cell.CellType)
                                    //{
                                    //    case NPOI.SS.UserModel.CellType.Numeric:
                                    //        Val = row.GetCell(j).NumericCellValue;
                                    //        //dataGridView1[j, i].Value = sh.GetRow(i).GetCell(j).NumericCellValue;

                                    //        break;
                                    //    case NPOI.SS.UserModel.CellType.String:
                                    //        Val = row.GetCell(j).StringCellValue;
                                    //        break;
                                    //    case NPOI.SS.UserModel.CellType.Boolean:
                                    //        Val = row.GetCell(j).BooleanCellValue;
                                    //        break;
                                    //    case NPOI.SS.UserModel.CellType.Unknown:
                                    //        Val = row.GetCell(j).ToString();
                                    //        break;
                                    //    case NPOI.SS.UserModel.CellType.Blank:
                                    //        Val = row.GetCell(j).ToString();
                                    //        break;
                                    //}
                                    if (Nullable.GetUnderlyingType(Info.PropertyType) != null)
                                    {
                                        if (Val != DBNull.Value)
                                        {
                                            object newA = Convert.ChangeType(Val,
                                                Nullable.GetUnderlyingType(Info.PropertyType));

                                            Info.SetValue(newObject, newA, null);
                                        }
                                        else
                                        {
                                            Info.SetValue(newObject, null, null);

                                        }
                                    }
                                    else
                                    {
                                        object newA = Convert.ChangeType(Val, Info.PropertyType);

                                        Info.SetValue(newObject, (newA == DBNull.Value) ? null : newA, null);
                                    }
                                }
                            }




                        }
                    }
                    RetVal.Add(newObject);

                }

                return RetVal;
                // data.AddRange(list);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
