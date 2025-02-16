namespace Auth.Application.DTOs.Results;

public class ResultSuccessDto<T>(bool isSuccess, T data)
{
    public bool IsSuccess { get; set; } = isSuccess;

    public T Data { get; set; } = data;
}