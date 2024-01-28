using belajarnetapi.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace belajarnetapi.Helper
{
    public class CustomValidationFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            var result = new ObjectResult(new
            {
                Success = false,
                Data = (string)null,
                ErrorMessage = string.Join(", ", errors)                
            })
            {
                StatusCode = 400,
            };

            context.Result = result;
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        // Do nothing on result execution
    }
}
}