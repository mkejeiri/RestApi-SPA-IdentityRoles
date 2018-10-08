using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    public class Fallback : Controller 
    {
       public IActionResult Index(){

            //return a physical full path filename : index.html            
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), 
            "wwwroot","index.html"),"text/HTML");
        }
    }
}