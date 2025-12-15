using ShortUrl.Common;

namespace ShortUrl.WebApp.Utility
{
    public static class ResultWebUtility
    {
        public static IResult ToResponse<T>(this Result<T> result) where T : notnull
        {
            return result switch
            {
                { IsSuccess: true } => Results.Ok(result.Value),
                { Error.Type: ErrorType.Unauthorized } => Results.Unauthorized(),
                { Error.Type: ErrorType.Forbidden } => Results.Forbid(),
                { Error.Type: ErrorType.NotFound } => Results.NotFound(result.Error.Message),
                { Error.Type: ErrorType.Conflict } => Results.Conflict(result.Error.Message),
                { Error.Type: ErrorType.Validation } => Results.BadRequest(result.Error.Message),
                _ => Results.Problem(result.Error.Message)
            };
        }
    }
}

