using DashBoardAPI.Repository;
using DashBoardAPI.Entity;
using System.Data.SqlClient;
using DashBoardAPI.Service.DashBoardService;
using System.Data;

namespace DashBoardAPI.Service.DashBoardService
{
    public class DashBoardService:IDashBoardService
    {
        private readonly IRepository<DashBoardEntity> _DashBoardRepository;

        public DashBoardService(IRepository<DashBoardEntity> dashBoardRepository)
        {
            this._DashBoardRepository = dashBoardRepository;
        }
        public List<DashBoardEntity> GetDashBoardData()
        {
            List<DashBoardEntity> data = new List<DashBoardEntity>();
            try
            {
                SqlCommand command = new SqlCommand("stpGetDashBoardData");
                command.CommandType = System.Data.CommandType.StoredProcedure;  
                data = _DashBoardRepository.GetRecords(command).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return data;
        }
    
    }
}
