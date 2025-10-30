using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;
using System.IO;
using System.Runtime.Serialization;
using DevExpress.XtraReports.UI;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace myService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new Service1()
            //};
            //ServiceBase.Run(ServicesToRun);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Service_Form());
        }
    }
}



/// <summary>
/// The main entry point for the application.
/// </summ
//////////////////


//public class Schedular
//    {
//        public int SchedularID { get; set; }
//        public string SchedularName { get; set; }
//        public string Email { get; set; }
//        public int Frequency { get; set; }
//        public int Weekdays { get; set; }
//        public int MonthlyDate { get; set; }
//    }
//    public partial class RepMaster
//    {
//        public int Report_ID { get; set; }
//        public string Report_Name { get; set; }
//        public bool IsChecked { get; set; }

//    }

//    public partial class ReportList
//    {
//        public int ID { get; set; }
//        public int SchedularID { get; set; }
//        public int Report_ID { get; set; }
//    }


//    public class Service1 : ServiceBase
//    {
//        private System.Timers.Timer timer;
//        public string connectionString = @"Data Source=SMLL2212089;Initial Catalog=LoginDatabase;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";
//        protected override void OnStart(string[] args)
//        {
//            // Create and configure the timer
//            timer = new System.Timers.Timer();
//            timer.Interval = 5000;
//            timer.Elapsed += TimerElapsed;
//            // Start the timer
//            timer.Start();
//        }

//        protected override void OnStop()
//        {
//            // Stop the timer
//            timer.Stop();
//            timer.Dispose();
//        }


//        public void testt(object sender, ElapsedEventArgs e)
//        {
//            DateTime currentTime = DateTime.Now;
//            int wk = (int)DateTime.Today.DayOfWeek;
//            string dateOnly = currentTime.ToString("dd");


//            List<Schedular> entries = GetDatabaseEntries(connectionString, currentTime);

//            foreach (Schedular schedule in entries)
//            {
//                int frequency = schedule.Frequency;
//                int Weekdays = schedule.Weekdays;

//                string data_date;
//                int MonthlyDate = schedule.MonthlyDate;
//                data_date = MonthlyDate.ToString();


//                int scheduleID = schedule.SchedularID;
//                List<int> reportIDs = getreportlist(scheduleID, connectionString);

//                if (frequency == 1)
//                {
//                    Console.WriteLine("Daily");
//                    foreach (var rep in reportIDs)
//                    {
//                        DataTable data = YourDataAccessMethod(rep);
//                    }
//                }

//                else if (frequency == 2)
//                {
//                    if (Weekdays == wk)
//                    {
//                        Console.WriteLine("Weekly");
//                        foreach (var rep in reportIDs)
//                        {
//                            DataTable data = YourDataAccessMethod(rep);
//                        }
//                    }
//                }

//                else if (frequency == 3)
//                {
//                    if (data_date == dateOnly.TrimStart('0'))
//                    {
//                        Console.WriteLine("Monthly");

//                        foreach (var rep in reportIDs)
//                        {
//                            DataTable data = YourDataAccessMethod(rep);
//                        }
//                    }
//                }
//            }
//        }


//        public void TimerElapsed(object sender, ElapsedEventArgs e)
//        {
//            // Timer elapsed logic - execute your desired code here

//            testt(sender, e);

//        }
//        public static List<int> getreportlist(int scheduleID, String connectionString)
//        {
//            List<int> list = new List<int>();

//            using (SqlConnection connection = new SqlConnection(connectionString))
//            {
//                connection.Open();
//                string query = "SELECT Report_ID FROM ReportList WHERE SchedularID = @ScheduleID";
//                SqlCommand command = new SqlCommand(query, connection);
//                command.Parameters.AddWithValue("@ScheduleID", scheduleID);
//                SqlDataReader reader = command.ExecuteReader();



//                while (reader.Read())
//                {
//                    int reportID = Convert.ToInt32(reader["Report_ID"]);
//                    list.Add(reportID);
//                }
//            }
//            return list;
//        }
//        public List<Schedular> GetDatabaseEntries(string connectionString, DateTime currentTime)
//        {
//            List<Schedular> results = new List<Schedular>();



//            using (SqlConnection connection = new SqlConnection(connectionString))
//            {
//                connection.Open();
//                string query = "SELECT SchedularID, SchedularName, Email, Frequency, Weekdays, MonthlyDate FROM Schedular WHERE CAST(SchedularTime AS TIME) = CAST(@currentTime AS TIME)";
//                using (SqlCommand command = new SqlCommand(query, connection))
//                {
//                    command.Parameters.AddWithValue("@currentTime", currentTime);

//                    using (SqlDataReader reader = command.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            Schedular schedule = new Schedular
//                            {
//                                SchedularID = Convert.ToInt32(reader["SchedularID"]),
//                                SchedularName = (string)reader["SchedularName"],
//                                Email = (string)reader["Email"],
//                                Frequency = Convert.ToInt32(reader["Frequency"]),
//                                Weekdays = Convert.ToInt32(reader["Weekdays"]),
//                                MonthlyDate = Convert.ToInt32(reader["MonthlyDate"])
//                            };

//                            results.Add(schedule);

//                        }

//                    }
//                }
//            }

//            return results;
//        }

//        public static void Main()
//        {
//            ServiceBase.Run(new Service1());
//        }


//        public DataTable YourDataAccessMethod(int repid)
//        {
//            DataTable dt = new DataTable();
//            using (SqlConnection con = new SqlConnection(connectionString))
//            {
//                con.Open();

//                // int reportId = report.Report_ID;
//                SqlCommand cmd = new SqlCommand("GetDSM_ABTData", con);
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.Parameters.AddWithValue("@Report_ID", repid);
//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                da.Fill(dt);

//                GenerateReport(repid, dt);
//            }
//            return dt;
//        }

//        public void GenerateReport(int reportID, DataTable dataSource)
//        {
//            // Load the report based on the provided reportID
//            string reportFileName = reportID + ".repx";
//            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
//            string reportFilePath = Path.Combine(baseDirectory, "Reports", reportFileName);

//            XtraReport report = new XtraReport();
//            report.LoadLayout(reportFilePath);

//            // Generate the CSV content from the report's data source
//            StringBuilder reportCsvContent = GenerateCsvContent(dataSource, report);

//            // Export the CSV content to a file
//            byte[] csvBytes = Encoding.UTF8.GetBytes(reportCsvContent.ToString());
//            string csvFileName = reportID + ".csv";
//            string csvFilePath = Path.Combine(baseDirectory, "Reports", csvFileName);
//            File.WriteAllBytes(csvFilePath, csvBytes);
//        }

//        public StringBuilder GenerateCsvContent(DataTable dataSource, XtraReport report)
//        {
//            StringBuilder csvContent = new StringBuilder();
//            // Extract the column names from the report's data source
//            List<string> columnNames = new List<string>();
//            foreach (DataColumn column in dataSource.Columns)
//            {
//                columnNames.Add(QuoteCsvValue(column.ColumnName));
//            }
//            csvContent.AppendLine(string.Join(",", columnNames));

//            foreach (DataRow row in dataSource.Rows)
//            {
//                // Extract the values for each column
//                List<string> columnValues = new List<string>();
//                foreach (DataColumn column in dataSource.Columns)
//                {
//                    string value = QuoteCsvValue(row[column.ColumnName].ToString());
//                    columnValues.Add(value);
//                }

//                csvContent.AppendLine(string.Join(",", columnValues));
//            }

//            return csvContent;
//        }

//        public string QuoteCsvValue(string value)
//        {
//            // Check if the value contains special characters that require quoting
//            if (value.Contains(",") || value.Contains("\"") || value.Contains("\r") || value.Contains("\n"))
//            {
//                // Escape double quotes by doubling them
//                value = value.Replace("\"", "\"\"");
//                // Enclose the value in double quotes
//                value = "\"" + value + "\"";
//            }
//            return value;
//        }
//    }




