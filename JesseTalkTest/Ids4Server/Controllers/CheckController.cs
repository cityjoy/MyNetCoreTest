using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Ids4Server.Controllers
{
    [Route("api/[controller]")]
    public class CheckController : Controller
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        
    }
}
