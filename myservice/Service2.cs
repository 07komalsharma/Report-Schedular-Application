//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Diagnostics;
//using System.Linq;
//using System.ServiceProcess;
//using System.Text;
//using System.Threading.Tasks;
//using System.Timers;

using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using System.Net.Mail;
using System.IO.Compression;
using System.Net;
using DevExpress.DocumentServices.ServiceModel.DataContracts;

namespace myService
{
    partial class Service2 : ServiceBase
    {
        private System.Timers.Timer timer;
        public Service2()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            Service1 se1= new Service1();
            se1.startTimer();

        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            timer.Stop();
            timer.Dispose();
        }
    }
}
