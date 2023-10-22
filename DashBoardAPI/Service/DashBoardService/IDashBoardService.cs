using DashBoardAPI.Entity;

namespace DashBoardAPI.Service.DashBoardService
{
    public interface IDashBoardService
    {
        JsonResponseEntity GetDashboardData();
        JsonResponseEntity InsertBulkUploadLocationData(DashBoardEntity objentity);
    }
}
