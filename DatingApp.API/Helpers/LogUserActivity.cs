using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;


namespace DatingApp.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            /*allow us to run some code while the action is executing*/
            ActionExecutingContext context,
             /*allow us to run some code after the action is executing - we will use this approach*/
            ActionExecutionDelegate next)
        {

           //we wait until the action is completed
           var resultContext = await next();

           //we get userId
           var userId = int.Parse(resultContext.HttpContext.User
           .FindFirst(ClaimTypes.NameIdentifier).Value);

           //we need to get our repo, provided as a service in our DI container
           var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
           var user = await repo.GetUser(userId);
           user.LastActive = DateTime.Now;
           await repo.SaveAll();
        }
    }
}