namespace DashBoardAPI.Entity
{
    public class JsonResponseEntity
    {
        public ApiStatus Status { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }
        public Int64 TotalCount { get; set; }
    }

    public enum ApiStatus
    {
        OK,
        Error,
        OutOfTime,
        AccessDenied,
        Success
    }
}
