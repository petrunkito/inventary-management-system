namespace InventaryManagementSystem.Services.Result
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string Error { get; set; } = default!;
        public T Data { get; set; } = default!;

        public static Result<T> Failure(string error) => new Result<T> { Success = false, Error = error };
        public static Result<T> Ok(T data) => new Result<T> { Success = true, Data = data };
    }
}
