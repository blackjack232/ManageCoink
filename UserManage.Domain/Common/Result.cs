

namespace UserManage.Domain.Common
{
    public class Result<T>
    {
        public bool Success { get; private set; }
        public T? Data { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public List<string> Errors { get; private set; } = new();

        public static Result<T> SuccessResult(T data, string message = "Operación exitosa")
        {
            return new Result<T> { Success = true, Data = data, Message = message };
        }

        public static Result<T> FailureResult(string message, List<string>? errors = null)
        {
            return new Result<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
