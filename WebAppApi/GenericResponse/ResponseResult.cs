namespace WebAppApi.GenericResponse
{
    public class ResponseResult<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public object? Meta { get; set; }

        public static ResponseResult<T> Success(T? data, string message, object? meta = null)
        {
            return new ResponseResult<T>
            {
                Data = data,
                Message = message,
                Status = true,
                Meta = meta
            };
        }

        public static ResponseResult<T> Failure(T? data, string message)
        {
            return new ResponseResult<T>
            {
                Data = data,
                Message = message,
                Status = false
            };
        }
    }
}
