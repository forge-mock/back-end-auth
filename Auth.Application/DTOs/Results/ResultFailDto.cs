using FluentResults;

namespace Auth.Application.DTOs.Results;

public class ResultFailDto(bool isSuccess, List<IError> errors)
{
    public bool IsSuccess { get; set; } = isSuccess;

    public List<IError> Errors { get; set; } = errors;
}