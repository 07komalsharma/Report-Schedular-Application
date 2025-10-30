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
    public class Schedular
    {
        public int SchedularID { get; set; }
        public string SchedularName { get; set; }
        public string Email { get; set; }
        public int Frequency { get; set; }
        public int Weekdays { get; set; }
        public int MonthlyDate { get; set; }

        public TimeSpan SchedularTime { get; set; }

        public string seq;
    }
    public partial class RepMaster
    {
        public int Report_ID { get; set; }
        public string Report_Name { get; set; }
        public bool IsChecked { get; set; }

    }

    public partial class ReportList
    {
        public int ID { get; set; }
        public int SchedularID { get; set; }
        public int Report_ID { get; set; }
    }


    public partial class Service1 : ServiceBase
    {
        // public Service1()
        //{
        // InitializeComponent();

        private bool isTaskRunning;
        private System.Timers.Timer timer;
        public string connectionString = @"Data Source=SMLL2212089;Initial Catalog=LoginDatabase;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";

        public void startTimer()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 30000;
            timer.Elapsed += TimerElapsed;
            // Start the timer
            timer.Start();
        }
        //protected override void OnStart(string[] args)
        //{
        //    // Create and configure the timer
        //    startTimer();
        //}

        //protected override void OnStop()
        //{
        //    // Stop the timer
        //    timer.Stop();
        //    timer.Dispose();
        //}

        public void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Timer elapsed logic - execute your desired code here
            if (isTaskRunning) return;

            isTaskRunning = true;
            try
            {
                ExecuteTask(sender, e);
            }
            finally
            {
                isTaskRunning = false;
            }

        }
        public void ExecuteTask(object sender, ElapsedEventArgs e)
        {
            // LogIntoFile("Task started");
            // DateTime currentTime = DateTime.Now;

            //DateTime currentTime = new DateTime(2023, 6, 2, 03, 15, 0);

            //int wk = (int)DateTime.Today.DayOfWeek;
            //string dateOnly = currentTime.ToString("dd");s


            List<Schedular> entries = GetDatabaseEntries(connectionString);

            foreach (Schedular schedule in entries)
            {
                schedule.seq = schedule.SchedularID + "_" + DateTime.Now.ToString("ddMMyyHHmmss");
                int scheduleID = schedule.SchedularID;
                List<int> reportIDs = getreportlist(scheduleID, connectionString);
                foreach (var rep in reportIDs)
                {
                    DataTable data = YourDataAccessMethod(rep, schedule.seq);

                }

                //zip.......

                string PDFFolderPath = ConfigurationManager.AppSettings["PDFFilePath"] + schedule.seq;

                string ZipFolderPath = ConfigurationManager.AppSettings["ZipFilePath"] + schedule.seq + @"\";

                if (!Directory.Exists(ZipFolderPath)) Directory.CreateDirectory(ZipFolderPath);

                string zipFile = ZipFolderPath + schedule.seq + ".zip";

                ZipFile.CreateFromDirectory(Path.GetDirectoryName(PDFFolderPath), zipFile);

                SendEmailWithAttachment(zipFile, schedule);

                //lastrun time

                //function call for next run time......

                UpdateNextRunschedule(schedule, connectionString);
                //nextrun time with scheduleid
            }
        }


        public static void UpdateNextRunschedule(Schedular schedule, String connectionString)
        {
            int frequency = schedule.Frequency;
            int schID = schedule.SchedularID;
            DateTime nextRunTime = DateTime.Now;

            if (frequency == 1)
            {
                nextRunTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    schedule.SchedularTime.Hours, schedule.SchedularTime.Minutes, schedule.SchedularTime.Seconds);
                if (nextRunTime < DateTime.Now)
                {
                    nextRunTime = nextRunTime.AddDays(1);
                }
            }
            if (frequency == 3)
            {
                nextRunTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, schedule.MonthlyDate,
                    schedule.SchedularTime.Hours, schedule.SchedularTime.Minutes, schedule.SchedularTime.Seconds);
                if (nextRunTime < DateTime.Now)
                {
                    nextRunTime = nextRunTime.AddMonths(1);
                }
            }

            if (frequency == 2)
            {
                DateTime nextCycleDate = DateTime.Now.AddDays(((int)schedule.Weekdays - (int)DateTime.Now.DayOfWeek + 7) % 7);

                nextRunTime = new DateTime(nextCycleDate.Year, nextCycleDate.Month, nextCycleDate.Day,
                     schedule.SchedularTime.Hours, schedule.SchedularTime.Minutes, schedule.SchedularTime.Seconds);
                if (nextRunTime < DateTime.Now)
                {
                    nextRunTime = nextRunTime.AddDays(7);
                }
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = @"UPDATE Schedular SET NextTime = @NextTime WHERE SchedularID = @SchID;";

                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@NextTime", nextRunTime);
                    command.Parameters.AddWithValue("@SchID", schedule.SchedularID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SendEmailWithAttachment(string ZipFile, Schedular schedule)
        {

            string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
            string passwd = ConfigurationManager.AppSettings["EmailPasswd"];
            string bccEmail = ConfigurationManager.AppSettings["BCCEmail"];


            // Create a new email message
            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(fromEmail);
                message.To.Add(schedule.Email);
                message.Bcc.Add(bccEmail);
                message.Subject = "Reports from scheduler - " + schedule.seq;
                message.Body = "Reports from scheduler - " + schedule.seq; 

                // Attach the ZIP file to the email
                Attachment attachment = new Attachment(ZipFile, "application/zip");
                message.Attachments.Add(attachment);

                // Configure the SMTP client
                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(fromEmail, passwd);


                    try
                    {
                        // Send the email
                        smtpClient.Send(message);
                        LogIntoFile("Email sent successfully!", schedule);
                    }
                    catch (Exception ex)
                    {
                        LogIntoFile("Failed to send email: " + ex.Message, schedule);
                    }
                    finally
                    {
                        // Clean up the temporary folder and files
                        message.Dispose();
                        attachment.Dispose();
                    }
                }

            }

        }
        public void GenerateReport(int reportID, DataTable dataSource, string seq)
        {
            string RepxxFolderPath = ConfigurationManager.AppSettings["RepxFolderPath"];
            string PDFFolderPath = ConfigurationManager.AppSettings["PDFFilePath"];
            string reportFileName = reportID + ".repx";
            string reportFilePath = Path.Combine(RepxxFolderPath, reportFileName);
            XtraReport report = new XtraReport();
            report.LoadLayout(reportFilePath);
            report.DataSource = dataSource;
            report.DataMember = dataSource.TableName;
            // report.CreateDocument();
            var pdfOptions = new PdfExportOptions();
            string outputPath = PDFFolderPath + seq;
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

            //// string oFilePath = outputPath +@"\" + $"{DateTime.Now.ToString("dd-MM-yy HH:mm:ss")}_" + reportID + "_Report.pdf";
            string oFilePath = Path.Combine(outputPath, $"{DateTime.Now.ToString("dd-MM-yy HH-mm-ss")}_{reportID}_Report.pdf");
            // PdfExportOptions pdfOptions = new PdfExportOptions();
            report.ExportToPdf(oFilePath, pdfOptions);

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
        //    report.DataMember= dataSource.TableName;
        //   // report.CreateDocument();
        //    var pdfOptions = new PdfExportOptions();
        //    string outputPath = PDFFolderPath + seq;
        //    if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

        //     //// string oFilePath = outputPath +@"\" + $"{DateTime.Now.ToString("dd-MM-yy HH:mm:ss")}_" + reportID + "_Report.pdf";
        //    string oFilePath = Path.Combine(outputPath, $"{DateTime.Now.ToString("dd-MM-yy HH-mm-ss")}_{reportID}_Report.pdf");
        //   // PdfExportOptions pdfOptions = new PdfExportOptions();
        //    report.ExportToPdf(oFilePath, pdfOptions);

        //}



        public DataTable YourDataAccessMethod(int repid, string seq)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // int reportId = report.Report_ID;
                SqlCommand cmd = new SqlCommand("GetDSM_ABTData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Report_ID", repid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                GenerateReport(repid, dt, seq);
            }
            return dt;
        }




        public static List<int> getreportlist(int scheduleID, String connectionString)
        {
            List<int> list = new List<int>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Report_ID FROM ReportList WHERE SchedularID = @ScheduleID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ScheduleID", scheduleID);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int reportID = Convert.ToInt32(reader["Report_ID"]);
                    list.Add(reportID);
                }
            }
            return list;
        }


        public List<Schedular> GetDatabaseEntries(string connectionString)
        {
            List<Schedular> results = new List<Schedular>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //nextrun timne with scheduleid=0 on first run
                string query = "SELECT SchedularID, SchedularName, Email, Frequency, Weekdays, MonthlyDate,SchedularTime FROM Schedular WHERE  NextTime IS NULL OR NextTime <= @currentTime";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@currentTime", DateTime.Now);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Schedular schedule = new Schedular
                            {
                                SchedularID = Convert.ToInt32(reader["SchedularID"]),
                                SchedularName = (string)reader["SchedularName"],
                                Email = (string)reader["Email"],
                                Frequency = Convert.ToInt32(reader["Frequency"]),
                                Weekdays = Convert.ToInt32(reader["Weekdays"]),
                                MonthlyDate = Convert.ToInt32(reader["MonthlyDate"]),
                                SchedularTime = TimeSpan.Parse(reader["SchedularTime"].ToString())
                            };

                            results.Add(schedule);

                        }

                    }
                }
            }

            return results;
        }



        public void LogIntoFile(string message, Schedular schedule)
        {
            string logEntry = $"{DateTime.Now}: SchedulerID={schedule.SchedularID},Email={schedule.Email}, Message={message}";

            string directoryPath = @"C:\SchdlLogs\";
            string logFileName = $"{DateTime.Now.Date.ToString("dd-MM-yy")}_Logs.txt";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            try
            {
                using (StreamWriter writer = File.AppendText(directoryPath + logFileName))
                {
                    // Write the log entry to the file
                    writer.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                //
            }

        }

    }
}

