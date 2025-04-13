using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OnlineEnrollmentSystem.Controllers  
{  
    public class NodeErrorController : Controller  
    {  
        // GET: /NodeError/Index?affectedPage={page}&node={node}  
        public IActionResult Index(string affectedPage, string node)  
        {  
            // Use ViewBag or a model to pass the values to the view  
            ViewBag.AffectedPage = affectedPage ?? "this page";  
            ViewBag.Node = node ?? "an unknown node";  

            return View();  
        }  
    }  
}