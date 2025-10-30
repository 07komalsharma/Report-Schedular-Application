using System;
using System.Data;
using System.IO;

namespace ReportSchedular.Controllers
{
    internal class XtraReport
    {
        internal DataTable DataSource;
        XtraReport report = new XtraReport();
        public XtraReport()
        {
        }

        public object stream { get; internal set; }

        //internal void CreateDocument()
        //{
        //    throw new NotImplementedException();
        //}

        internal void ExportToPdf(MemoryStream stream)
        {
            throw new NotImplementedException();
        }

        //internal void LoadLayout(string v)
        //{
        //    report.LoadLayout(v);
        //}


    }
}