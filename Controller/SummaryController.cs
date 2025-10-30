using DevExpress.XtraPrinting.Native;
using ReportSchedular.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace ReportSchedular.Controllers
{
    public class SummaryController : Controller
    {
       
       
        public ActionResult Index()
        {
            
            return View();
          
        }
      
    }


}
