using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Presentetion.ActionFilters
{
    public class ValidateMediaTypeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var acceptHeaderPresent = context.HttpContext
                .Request
                .Headers
                .ContainsKey("Accept");

            if (!acceptHeaderPresent)
            {
                context.Result = new BadRequestObjectResult($"Accept header is missing!");
                return;
            }

            var mediaType = context.HttpContext
                .Request
                .Headers["Accept"]
                .FirstOrDefault();

            if(MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue? outMediaType))
            {
                context.Result = new BadRequestObjectResult($"Media Type not present." +
                    $"Please add Accept header with required media type.");
                return;
            }

            context.HttpContext.Items.Add("AcceptHeaderMediaType", outMediaType);
        }
    }
}
