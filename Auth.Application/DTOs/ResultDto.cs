using FluentResults;

namespace Auth.Application.DTOs;

public class ResultFailDto(bool isSuccess, List<IError> errors)
{
    public bool IsSuccess { get; set; } = isSuccess;

    public List<IError> Errors { get; set; } = errors;
}

public class ResultSuccessDto<T>(bool isSuccess, T data)
{
    public bool IsSuccess { get; set; } = isSuccess;

    public T Data { get; set; } = data;
}