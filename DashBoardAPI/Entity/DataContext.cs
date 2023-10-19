
using Microsoft.EntityFrameworkCore;

namespace DashBoardAPI.Entity

{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<DashBoardEntity> Dashboards=> Set<DashBoardEntity>();
    }
}
