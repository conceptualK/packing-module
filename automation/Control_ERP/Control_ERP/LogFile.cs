using System;
using System.IO;

namespace Control_ERP
{
    public static class LogFile
    {
        //private static readonly string logDirectory = @"C:\Users\bew.bun\Desktop\create-order-from-odoo-api\automation\Control_ERP\Control_ERP\ServiceLog"; // เปลี่ยนเป็นโฟลเดอร์ที่ต้องการ

       private static readonly string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServiceLog");

     


        private static string currentLogFile = string.Empty;
        private static readonly long maxFileSize = 4000 * 1024; // 4000 KB
        private static DateTime currentLogDate;
        private static readonly object lockObj = new object(); // ใช้สำหรับ thread-safe

        public static void WriteLog(string message)
        {
            InitializeLogFileIfNeeded();

            Log(message);
        }

        private static void InitializeLogFileIfNeeded()
        {
            lock (lockObj)
            {
                // หากยังไม่มีไฟล์ log ปัจจุบัน ให้สร้างใหม่
                if (string.IsNullOrEmpty(currentLogFile))
                {
                    InitializeLogFile();
                    return;
                }

                // ตรวจสอบวันที่หรือขนาดไฟล์
                FileInfo fileInfo = new FileInfo(currentLogFile);

                if (DateTime.Today > currentLogDate || fileInfo.Length >= maxFileSize)
                {
                    InitializeLogFile();
                }
            }
        }

        private static void InitializeLogFile()
        {
            lock (lockObj)
            {
                // สร้างโฟลเดอร์หากยังไม่มี
                if (!Directory.Exists(logDirectory))
                {
                    Console.WriteLine($"Creating log directory: {logDirectory}");
                    Directory.CreateDirectory(logDirectory);
                }

                // ตั้งค่า log ใหม่
                currentLogDate = DateTime.Today;

                // สร้างชื่อไฟล์ใหม่ (เพิ่ม milliseconds เพื่อไม่ให้ซ้ำ)
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                currentLogFile = Path.Combine(logDirectory, $"{timestamp}.log");

                // เขียนข้อความเริ่มต้นในไฟล์ใหม่
                File.WriteAllText(currentLogFile, $"Log file created at {DateTime.Now}{Environment.NewLine}");
            }
        }

        private static void Log(string message)
        {
            lock (lockObj)
            {
                string date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                string logMessage = $"{date} - {message}{Environment.NewLine}";

                using (var stream = new FileStream(currentLogFile, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(logMessage);
                }
            }
        }
    }
}
