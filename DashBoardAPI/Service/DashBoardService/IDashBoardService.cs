using DashBoardAPI.Entity;

namespace DashBoardAPI.Service.DashBoardService
{
    public interface IDashBoardService
    {
        List<DashBoardEntity> GetDashBoardData();
    }
}
