using DashBoardAPI.Repository;
using DashBoardAPI.Entity;
using System.Data.SqlClient;
using DashBoardAPI.Service.DashBoardService;
using System.Data;
using Microsoft.IdentityModel.Logging;
using OfficeOpenXml;

namespace DashBoardAPI.Service.DashBoardService
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IRepository<DashBoardEntity> _DashBoardRepository;

        public DashBoardService(IRepository<DashBoardEntity> dashBoardRepository)
        {
            this._DashBoardRepository = dashBoardRepository;

        }
        #region DashBoard Api GetDashBoard Data
        public JsonResponseEntity GetDashboardData()
        {
            JsonResponseEntity Jsonmodel = new JsonResponseEntity();
            try
            {
                SqlCommand command = new SqlCommand("stpGetDashBoardData");
                command.CommandType = CommandType.StoredProcedure;
                List<DashBoardEntity> getdata = _DashBoardRepository.GetRecords(command).ToList();
                Jsonmodel.Data = getdata;
                Jsonmodel.Message = "Success";
                Jsonmodel.Status = ApiStatus.OK;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Jsonmodel;
        }
       public JsonResponseEntity InsertBulkUploadLocationData(DashBoardEntity objentity)
        {
            JsonResponseEntity Jsonmodel = new JsonResponseEntity();
            try
            {
                SqlCommand command = new SqlCommand("stpBulkUploadDashBoadData");
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@Name", SqlDbType.VarChar).Value = objentity.Name;
                command.Parameters.Add("@ContactNumber", SqlDbType.VarChar).Value = objentity.ContactNumber;
                command.Parameters.Add("@UPI", SqlDbType.VarChar).Value = objentity.UPI;
                command.Parameters.Add("@Amount", SqlDbType.BigInt).Value = objentity.Amount;
                command.Parameters.Add("@Remark", SqlDbType.VarChar).Value = objentity.Remark;
                command.Parameters.Add("@Dates", SqlDbType.VarChar).Value = objentity.Dates;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = objentity.IsActive;
                var BGetLastINsertedId = _DashBoardRepository.ExecuteProc(command);
                Jsonmodel.Data = BGetLastINsertedId;
                Jsonmodel.Message = "Success";
                Jsonmodel.Status = ApiStatus.OK;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Jsonmodel;
        }

        #endregion

     
    }
}
