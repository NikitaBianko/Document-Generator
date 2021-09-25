using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentGenerator.Core;
using DocumentGenerator.Models;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DocumentGenerator.Controller
{
    [ApiController]
    [Route("api/holidays")]
    public class HolidaysController : ControllerBase
    {
        [HttpGet]
        public List<DateTime> Get()
        {
            var pathToHolidaysFile = Path.Combine(Directory.GetCurrentDirectory(), "holidays.txt");
            var holidays = System.IO.File.ReadAllLines(pathToHolidaysFile).Select(date => DateTime.Parse(date)).ToList();

            return holidays;
        }
    }
}