using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using CW.ClientAPI.Services;
using CW.ClientAPI.Services.General;
using CW.ClientAPI.Controllers;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CW.ClientAPI.Controllers
{
    [Route("")]
    [Route("api/Home")]
    [ApiController]

    public class HomeController : Controller
    {
         
        public HomeController( )
        {
            
        }


       
    }   
}
