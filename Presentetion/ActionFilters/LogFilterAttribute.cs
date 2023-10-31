using Entities.LogModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentetion.ActionFilters
{
    public class LogFilterAttribute : ActionFilterAttribute
    {
        private readonly ILoggerService _logger;
        public LogFilterAttribute(ILoggerService logger)
        {
            _logger = logger;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInfo(Log("OnActionExecuting", context.RouteData));
        }

        private string Log(string ModelName, RouteData RouteData)
        {
            var logDetails = new LogDetails()
            {
                ModelModel = ModelName,
                Controller = RouteData.Values["controller"],
                Action = RouteData.Values["action"],
            };
            if(RouteData.Values.Count >= 3 )
            {
                logDetails.Id = RouteData.Values["Id"];
            }
            return logDetails.ToString();
        }
    }
}
