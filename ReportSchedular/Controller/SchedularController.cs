using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReportSchedular.Models;
using System.Data;
using System.Data.SqlClient;
using System.Web.DynamicData;
using DevExpress.ClipboardSource.SpreadsheetML;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using DevExpress.Utils.About;
using DevExpress.XtraRichEdit.Import.Rtf;
using System.IO;
using System.Runtime.Serialization;
using DevExpress.XtraReports.UI;
using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraReports;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.Data;


namespace ReportSchedular.Controllers
{
    public class SchedularController : Controller
    {
        string Str = @"Data Source=SMLL2212089;Initial Catalog=LoginDatabase;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";

        // GET: Schedular

        public ActionResult Index()
        {

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Str))
            {
                con.Open();
                string q = "Select SchedularID,SchedularName,Email,Frequency,Weekdays,MonthlyDate,SchedularTime from Schedular";
                SqlDataAdapter da = new SqlDataAdapter(q, con);
                da.Fill(dt);

            }

            return View(dt);

        }


        //---------------------------------- Code for conversion to pdf all data ......................... 
        public DataTable YourDataAccessMethod()
        {
            using (SqlConnection con = new SqlConnection(Str))
            {
                con.Open();
                DataTable dt = new DataTable();

                //foreach (var report in selectedReports)
                //{
                // Call the stored procedure based on the report ID
                //SqlCommand cmd = new SqlCommand(" SELECT ID, CAST(dsm_charges AS FLOAT) dsm_charges FROM DSM_tb", con); working
                //
                //cmd.CommandType = CommandType.Text; //workinggggg


                //cmd.Parameters.AddWithValue("@Report_ID", 2);
                SqlCommand cmd = new SqlCommand("GetDSM_ABTData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Report_ID",1);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    GenerateReport(dt);
              //  }

                return dt;
            }
        }
        //right working code............
        //public ActionResult GenerateReport(DataTable dataSource)
        //{
        //    try
        //    {
        //        string reportfilename = "1" + ".repx";
        //        string reportfilepath = Server.MapPath("~/App_Data/Reports/" + reportfilename);
        //        XtraReport report = new XtraReport();
        //        report.LoadLayout(reportfilepath);
        //        // string reportFilePath = Path.Combine(reportfilepath, reportfilename);
        //        report.DataSource = dataSource;
        //        report.DataMember = dataSource.TableName;
        //        // Specify CSV export options.
        //        var options = new CsvExportOptions();
        //        options.SkipEmptyColumns = false;
        //        options.SkipEmptyRows = false;
        //        options.EncodeExecutableContent = DefaultBoolean.True;
        //        string outputPath = @"C:\SchdlLogs\PDF";
        //        // Export a report to CSV.
        //        string oFilePath = outputPath + @"\" + "1Report.csv";
        //        report.ExportToCsvAsync(oFilePath, options);

        //    }
        //    // Return false if the CSV export failed.
        //    catch
        //    {

        //    }
        //    return View();
        //}

        public ActionResult GenerateReport(DataTable dataSource)
        {
            try
            {
                string reportfilename = "1" + ".repx";
                string reportfilepath = Server.MapPath("~/App_Data/Reports/" + reportfilename);
                XtraReport report = new XtraReport();
                report.LoadLayout(reportfilepath);
                // string reportFilePath = Path.Combine(reportfilepath, reportfilename);
                report.DataSource = dataSource;
                report.DataMember = dataSource.TableName;
                // Specify CSV export options.
                var options = new PdfExportOptions();
                //options.SkipEmptyColumns = false;
                //options.SkipEmptyRows = false;
                //options.EncodeExecutableContent = DefaultBoolean.True;
                string outputPath = @"C:\SchdlLogs\PDF";
                // Export a report to CSV.
                string oFilePath = outputPath + @"\" + "MyReporttt.pdf";
                report.ExportToPdf(oFilePath, options);

            }
            // Return false if the CSV export failed.
            catch
            {

            }
            return View();
        }


        //public void GenerateReport(int reportID, DataTable dataSource, string seq)
        //{
        //    string RepxxFolderPath = ConfigurationManager.AppSettings["RepxFolderPath"];
        //    string PDFFolderPath = ConfigurationManager.AppSettings["PDFFilePath"];
        //    string reportFileName = reportID + ".repx";
        //    string reportFilePath = Path.Combine(RepxxFolderPath, reportFileName);
        //    XtraReport report = new XtraReport();
        //    report.LoadLayout(reportFilePath);
        //    report.DataSource = dataSource;
        //    // report.DataMember=
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        report.CreateDocument();
        //        //string outputPath = PDFFolderPath + seq;
        //        //if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

        //        //// string oFilePath = outputPath +@"\" + $"{DateTime.Now.ToString("dd-MM-yy HH:mm:ss")}_" + reportID + "_Report.pdf";
        //        //string oFilePath = Path.Combine(outputPath, $"{DateTime.Now.ToString("dd-MM-yy HH-mm-ss")}_{reportID}_Report.pdf");
        //        //PdfExportOptions pdfOptions = new PdfExportOptions();
        //        //report.ExportToPdf(oFilePath, pdfOptions);
        //        PdfExportOptions options = new PdfExportOptions();
        //        options.ShowPrintDialogOnOpen = true;
        //        report.ExportToPdf(ms, options);
        //        ms.Seek(0, SeekOrigin.Begin);
        //        byte[] reportContent = ms.ToArray();

        //    }
        //}

      
      



        [HttpGet]
        public ActionResult Create()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(Str))
            {
                con.Open();
                string r = "Select Report_ID,Report_Name from RepMaster";
                SqlDataAdapter da = new SqlDataAdapter(r, con);
                da.Fill(dt);
            }
           

            // Create a new instance of ReportScheduleViewModel and populate the SchedularViewModel and RepMasterViewModel properties separately.
            var viewModel = new ReportScheduleViewModel();
            
            viewModel.RepMasterViewModel = new List<RepMaster>();

            foreach (DataRow dtRow in dt.Rows)
            {
                viewModel.RepMasterViewModel.Add(new RepMaster { Report_Name = Convert.ToString(dtRow["Report_Name"]), Report_ID = Convert.ToInt32(dtRow["Report_ID"]), IsChecked = false });
            }
            viewModel.SchedularViewModel = new Schedular();
            return View("Create",viewModel);
        }

        

        [HttpPost]
        public ActionResult Create(ReportScheduleViewModel schd)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Str))
                {
                    con.Open();
                    int schedularId=0;
                    int userId = (int)Session["userID"];


                    if ((int)schd.SchedularViewModel.Frequency == 1)
                    {
                        string q = "insert into Schedular values(@SchedularName, @Email, @Frequency, @Weekdays, @MonthlyDate,@SchedularTime,GETDATE(),@UserID,NULL); SELECT SCOPE_IDENTITY();";
                        SqlCommand cmd = new SqlCommand(q, con);
                        cmd.Parameters.AddWithValue("@SchedularName", schd.SchedularViewModel.SchedularName);
                        cmd.Parameters.AddWithValue("@Email", schd.SchedularViewModel.Email);
                       
                        cmd.Parameters.AddWithValue("@Frequency", (int)schd.SchedularViewModel.Frequency);
                        cmd.Parameters.AddWithValue("@Weekdays", -1);
                        cmd.Parameters.AddWithValue("@MonthlyDate", -1);
                        cmd.Parameters.AddWithValue("@SchedularTime", schd.SchedularViewModel.SchedularTime);
                        cmd.Parameters.AddWithValue("@UserID", userId); // Add the userID parameter
                        schedularId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    else if ((int)schd.SchedularViewModel.Frequency == 2)
                    {
                        string q = "insert into Schedular values(@SchedularName, @Email, @Frequency, @Weekdays, @MonthlyDate, @SchedularTime,GETDATE(),@UserID,NULL); SELECT SCOPE_IDENTITY();";
                        SqlCommand cmd = new SqlCommand(q, con);
                        cmd.Parameters.AddWithValue("@SchedularName", schd.SchedularViewModel.SchedularName);
                        cmd.Parameters.AddWithValue("@Email", schd.SchedularViewModel.Email);
                   
                        cmd.Parameters.AddWithValue("@Frequency", (int)schd.SchedularViewModel.Frequency);
                        cmd.Parameters.AddWithValue("@Weekdays", (int)schd.SchedularViewModel.Weekdays);
                        cmd.Parameters.AddWithValue("@MonthlyDate", -1);
                        cmd.Parameters.AddWithValue("@SchedularTime", schd.SchedularViewModel.SchedularTime);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        schedularId = Convert.ToInt32(cmd.ExecuteScalar());
                    }


                    else if ((int)schd.SchedularViewModel.Frequency == 3)
                    {
                        string q = "insert into Schedular values(@SchedularName, @Email, @Frequency, @Weekdays, @MonthlyDate, @SchedularTime,GETDATE(),@UserID,NULL); SELECT SCOPE_IDENTITY();";
                        SqlCommand cmd = new SqlCommand(q, con);
                        cmd.Parameters.AddWithValue("@SchedularName", schd.SchedularViewModel.SchedularName);
                        cmd.Parameters.AddWithValue("@Email", schd.SchedularViewModel.Email);
                      
                        cmd.Parameters.AddWithValue("@Frequency", (int)schd.SchedularViewModel.Frequency);
                        cmd.Parameters.AddWithValue("@Weekdays", -1);
                        cmd.Parameters.AddWithValue("@MonthlyDate", schd.SchedularViewModel.MonthlyDate);
                        cmd.Parameters.AddWithValue("@SchedularTime", schd.SchedularViewModel.SchedularTime);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        schedularId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    foreach (var item in schd.RepMasterViewModel)
                    {
                        if (item.IsChecked)
                        {
                            string st = "INSERT INTO ReportList (SchedularID, Report_ID) VALUES (@SchedularID, @Report_ID)";
                            SqlCommand cm = new SqlCommand(st, con);
                            cm.Parameters.AddWithValue("@SchedularID", schedularId);
                            cm.Parameters.AddWithValue("@Report_ID", item.Report_ID);
                            cm.ExecuteNonQuery();
                        }
                    }

                  //  List<RepMaster> selectedReports = schd.RepMasterViewModel.Where(r => r.IsChecked).ToList();
                  //  DataTable dataSource = YourDataAccessMethod(selectedReports);
                }
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(Str))
                {
                    con.Open();
                    string q = "Select SchedularID,SchedularName,Email,Frequency,Weekdays,MonthlyDate,SchedularTime from Schedular";
                    SqlDataAdapter da = new SqlDataAdapter(q, con);
                    da.Fill(dt);

                }
                return View("Index",dt);
            }

            catch (Exception ex)
            {
                return View();
            }
        }

    }

   
}






