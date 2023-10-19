
namespace DashBoardAPI.Entity
{
    public class DashBoardEntity:BaseEntity
    {
        public Int64 Id { get; set; } 
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string UPI { get; set; }
        public long Amount { get; set; }
        public string Remark { get; set; }

    }
}
