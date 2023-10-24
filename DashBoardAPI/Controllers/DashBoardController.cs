using DashBoardAPI.Configuration;
using DashBoardAPI.Entity;
using DashBoardAPI.Service.DashBoardService;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace DashBoardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private readonly IDashBoardService _dashBoardservice;
     

        public DashBoardController(IDashBoardService dashBoardservice)
        {

            this._dashBoardservice = dashBoardservice;
        }


        #region GetDashBoardData
        [HttpGet(Name = "GetDashBoardData")]
        public JsonResponseEntity GetDashBoardData()
        {
            JsonResponseEntity apiResponse = new JsonResponseEntity();
            try
            {
               
                var data = _dashBoardservice.GetDashboardData();
                
                if (data != null)
                {
                    apiResponse.Status = ApiStatus.OK;
                    apiResponse.Data = data;
                    apiResponse.Message = "Success";
                }
                else
                {
                    apiResponse.Status = ApiStatus.Error;
                    apiResponse.Data = null;
                    apiResponse.Message = "Fail";
                }

            }
            catch (Exception ex)
            {
                apiResponse.Status = ApiStatus.Error;
                apiResponse.Data = null;
                apiResponse.Message = "Something Went Wrong.";
            }
            return apiResponse;
        }
        #endregion
        #region API Upload Dashboard Data
        [HttpPost(Name="UploadBulkLocationDetails")]
        public JsonResponseEntity UploadBulkLocationDetails([FromForm]IFormFile file)
        {
            JsonResponseEntity apiResponse = new JsonResponseEntity();
            try
            {
                #region Check File Null or Empty
                if (file == null || file.Length == 0)
                {
                    apiResponse.Status = ApiStatus.Error;
                    apiResponse.Data = "Please select file.";
                    apiResponse.Message = "File_Error";
                    return apiResponse;
                }
                #endregion

                #region Allowed Extensions and Extension of File we Get from User
                string[] allowedExtensions = { ".xlsx", ".xls" };
                string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                #endregion

                #region Check Validation Extension
                if (!allowedExtensions.Contains(extension))
                {
                    apiResponse.Status = ApiStatus.Error;
                    apiResponse.Data = "Please upload a valid XLSX or XLS file.";
                    apiResponse.Message = "File_Error";
                    return apiResponse;
                }
                #endregion

                var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage(memoryStream))
                {
                    List<String> ValidationList = new List<String>();
                    bool IsLocationexceldataerror = false;

                    foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                    {
                        List<string> validationError = ExcelValidation(worksheet, IsLocationexceldataerror);
                        ValidationList.AddRange(validationError);
                    }

                    #region Check Validation Count
                    if (ValidationList.Count > 0)
                    {
                        apiResponse.Status = ApiStatus.Error;
                        apiResponse.Data = ValidationList;
                        apiResponse.Message = "VALIDATION_ERROR";
                        return apiResponse;
                    }
                    #endregion

                    foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                    {
                        // Find the last row and column with data
                        int lastRow = worksheet.Dimension.End.Row;
                        int lastCol = worksheet.Dimension.End.Column;
                        DashBoardEntity objentity = new DashBoardEntity();

                        // Read the data from the Excel file
                        for (int i = 2; i <= lastRow; i++)
                        {
                            string NamePresent = worksheet.Cells[i, 1].Value?.ToString() ?? "";

                            if (!string.IsNullOrEmpty(NamePresent))
                            {
                                #region Get Row Wise Value for Investor 
                                string IDate = worksheet.Cells[i, 1].Value?.ToString() ?? "";
                                string IName = worksheet.Cells[i, 2].Value?.ToString() ?? "";
                                string IContactNumber = worksheet.Cells[i, 3].Value?.ToString() ?? "";
                                string IUPI = worksheet.Cells[i, 4].Value?.ToString() ?? "";
                                string IAmount = worksheet.Cells[i, 5].Value?.ToString() ?? "";
                                string IRemark = worksheet.Cells[i, 6].Value?.ToString() ?? "";
                                #endregion

                                #region Insert Record For Location Master
                                objentity.Dates = IDate;
                                objentity.Name = IName;
                                objentity.ContactNumber = IContactNumber;
                                objentity.UPI = IUPI;
                                objentity.Amount = Convert.ToInt64(IAmount);
                                objentity.Remark = IRemark;
                                objentity.IsActive = Convert.ToBoolean(Convert.ToInt64(1));
                                var BGetLastInsertedId = _dashBoardservice.InsertBulkUploadLocationData(objentity);

                                if (BGetLastInsertedId.Message == "Error")
                                {
                                    apiResponse.Status = ApiStatus.Error;
                                    apiResponse.Data = BGetLastInsertedId;
                                    apiResponse.Message = "Fail";
                                    return apiResponse;
                                }
                                #endregion
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                apiResponse.Status = ApiStatus.Error;
                apiResponse.Data = ex.Message;
                apiResponse.Message = "error";
                return apiResponse;
            }
            apiResponse.Status = ApiStatus.OK;
            apiResponse.Data = "File uploaded successfully.";
            apiResponse.Message = "Ok";
            return apiResponse;
        }

        [NonAction]
        public List<String> ExcelValidation(ExcelWorksheet worksheet, bool IsLocationexceldataerror)
        {
            try
            {
                List<string> validationError = new List<string>();

                int lastRow = worksheet.Dimension.End.Row;
                int lastCol = worksheet.Dimension.End.Column;

                #region Header Count Vaildation for Broker sheet
                if (lastCol != 6)
                {
                    validationError.Add("Invalid Header Count for Payment sheet Please Check.");

                }
                #endregion

                // Validating Headers

                #region Header Names for Location sheet from Users and Expected
                var headers = new List<string>();
                foreach (var cell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    headers.Add(cell.Text);
                }
                var expectedHeadersforpayment = new List<string>
                        {
                            "Date",
                            "Name",
                            "ContactNumber",
                            "UPI",
                            "Amount",
                            "Remark"
                        };

                #endregion

                #region Compare headers and expectedHeaders of Location 
                if (!headers.SequenceEqual(expectedHeadersforpayment))
                {
                    validationError.Add("Invalid Header Names for Payment DashBoard sheet Please Check.");

                }
                if (worksheet.Dimension.End.Row <= 1)
                {
                    validationError.Add("Please provide data for Excel sheet.");
                }
                #endregion
                #region Check Broker Excel sheet contains data
                if (lastRow > 1)
                {
                    IsLocationexceldataerror = true;
                }
                #endregion
                //validating data

                #region Vaildation for Broker sheet
                for (int row = 2; row <= lastRow; row++)
                {
                    #region Insert Data from Cells into String 
                    int col = 1;
                    string BDate = worksheet.Cells[row, col].Value?.ToString() ?? ""; col++;
                    string BName = worksheet.Cells[row, col].Value?.ToString() ?? ""; col++;
                    string BContactNumber = worksheet.Cells[row, col].Value?.ToString() ?? ""; col++;
                    string BUPI = worksheet.Cells[row, col].Value?.ToString() ?? ""; col++;
                    string BAmount = worksheet.Cells[row, col].Value?.ToString() ?? ""; col++;
                    string BRemark = worksheet.Cells[row, col].Value?.ToString() ?? ""; col++;
                    string BIsActive = worksheet.Cells[row, col].Value?.ToString() ?? ""; col++;
                    #endregion
                    //#region Location
                    //if (string.IsNullOrEmpty(BName))
                    //{
                    //    validationError.Add($"Payment sheet error : Name is empty at row {row}");
                    //}
                    //if (!BName.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
                    //{
                    //    validationError.Add($"Paymeny sheet error : Name  is contains special characters at row {row}");
                    //}
                    //#endregion
                    //#region Code
                    //if (string.IsNullOrEmpty(BCode))
                    //{
                    //    validationError.Add($"Location sheet error :  Code is empty at row {row}");
                    //}
                    //if (!BCode.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
                    //{
                    //    validationError.Add($"Location sheet error :Code contains special characters at row {row}");
                    //}
                    //#endregion

                    //#region UnitCodeId
                    //// No need to check that as it will non mandatory, Raj Potdar, Date: 11-08-2023
                    //if (string.IsNullOrEmpty(BUnitCodeId))
                    //{
                    //    validationError.Add($"Location sheet error :  UnitCodeId empty at row {row}");
                    //}
                    //if (!BUnitCodeId.All(char.IsDigit))
                    //{
                    //    validationError.Add($"Location sheet error :BUnitCodeId contains non-digit characters at row {row}");
                    //}
                    //#endregion
                    //#region IsActive
                    //if (string.IsNullOrEmpty(BIsActive))
                    //{
                    //    validationError.Add($"Payment sheet error:  IsActive is empty at row {row}");
                    //}
                    //if (!BIsActive.All(c => c == '0' || c == '1'))
                    //{
                    //    validationError.Add($"Payment sheet error : IsActive contains special characters at row {row}");
                    //}
                    //#endregion


                }
                #endregion
                return validationError;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Export To Excel

        #endregion
    }
}
