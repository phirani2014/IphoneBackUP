using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Timers;
namespace IphoneBackUp
{
    
    class Program
    {
      
        static StringBuilder error = new StringBuilder();
        public static int filescopied = 0;
        static void Main(string[] args)
        {
           
            doTask();
        }

        public static void CreateFolder(string directory)
        {
            bool exists = System.IO.Directory.Exists(directory);
            if (!exists)
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }
        private static Regex r = new Regex(":");


        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                string dateTaken = "";
                try
                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                   // Console.WriteLine("Date " + dateTaken + " retrieved from " + path);
                }
                catch (Exception ex)
                {
                    //error.Append(ex.Message.ToString() + "For " + path + "\n");
                    //System.IO.File.WriteAllText(@"C:\Users\" + Environment.UserName + @"\Desktop" + @"\" + "imgconversionerror.txt", error.ToString());
                    dateTaken = DateTime.Today.ToShortDateString();
                }
                return DateTime.Parse(dateTaken);
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        
        public static void doTask()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sc = new StringBuilder();
            int counter = 0;

            int existscounter = 0;
            long totalsize = 0;
            long transfersize = 0;
            StringBuilder log = new StringBuilder();
            string method = Properties.Settings.Default.Method;
            string[] folders = System.IO.Directory.GetDirectories(Properties.Settings.Default.FolderPath, "*", System.IO.SearchOption.AllDirectories);
            foreach (var file in folders)
            {

                // Console.WriteLine(DateTime.Now);
                DirectoryInfo d = new DirectoryInfo(file);
                FileInfo[] Files = d.GetFiles();
                //Files.OrderBy(f => f.CreationTime);
                foreach (var image in Files)
                {
                    //bool f = image.IsSynchronized();
                    if (image.Extension != ".AAE")
                    {
                        var LastWriteTime = image.LastWriteTime;
                        var CreationTime = image.CreationTime;
                        var imgsource = image.FullName;

                        var Name = image.Name;
                        var month = CreationTime.Month;
                        string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);
                        var year = CreationTime.Year;
                        string source = d + @"\" + Name;
                        long length = image.Length;
                        totalsize = totalsize + length;
             
                        string result = FormatBytes(totalsize);
                    //  Console.WriteLine(result);
                        //var year = LastWriteTime.Year;
                        //if (image.Extension == ".JPG" || image.Extension == ".jpg" )
                        //{
                        //    if (image.Length > 1000)
                        //    {
                        //        var x = GetDateTakenFromImage(source.ToString());
                        //        var y = DateTime.Today.ToShortDateString();

                        //        if (x.ToShortDateString() != y)
                        //        {
                        //            year = x.Year;
                        //            month = x.Month;
                        //            monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);
                        //        }
                        //    }
                        //}
                        //else if (image.Extension == ".mp4" || image.Extension == ".PNG" || image.Extension == ".MOV")
                        //{
                        //var year = CreationTime.Year;
                        //month = CreationTime.Month;
                        //monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);
                        //}
                        var src = Properties.Settings.Default.BackUpPath + @"\" + year + @"\" + Name;
                        CreateFolder(Properties.Settings.Default.BackUpPath + @"\" + year);

                        var src2 = Properties.Settings.Default.BackUpPath + @"\" + year + @"\" + monthName;
                        CreateFolder(src2);
                        var src3 = Properties.Settings.Default.BackUpPath + @"\" + year + @"\" + monthName + @"\" + image.Extension;
                        CreateFolder(src3);
                        bool exists = System.IO.File.Exists(src3 + @"\" + Name);
                        long imglength = image.Length;
                        transfersize = transfersize + length;
                        string results = FormatBytes(transfersize);
                        if (!exists)
                        {
                            if (image.Extension != ".AAE")
                                try
                                {
                                   
                                    DirectoryInfo size = new DirectoryInfo(Properties.Settings.Default.BackUpPath);
                                  
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    if (method == "CopyTo")
                                    {
                                        image.CopyTo(src3 + @"\" + Name);
                                    
                                        
                                        sc.Append(DateTime.Now + "  File - " + imgsource + " - " + Properties.Settings.Default.BackUpPath + @"\" + year + @"\" + monthName + @"\" + image.Extension + @"\" + Name + " "+ results +" Copied " + "\n");
                                        if (Properties.Settings.Default.DeleteFiles == "Y") 
                                        {
                                            image.Delete();
                                        }
                                        Console.WriteLine(DateTime.Now + "  File - " + imgsource + " - " + Properties.Settings.Default.BackUpPath + @"\" + year + @"\" + monthName + @"\" + image.Extension + @"\" + Name  + "\n");
                                       // if (Properties.Settings.Default.DeleteFiles == "Y")
                                            counter++;
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                    //    long s = image.Length;
                                    //    //double.Parse(image.Length, out s);
                                    //    string size = FormatBytes(s);
                                    //    if (size.Contains("KB")) 
                                    //    {
                                    //       size = size.Replace("KB","");
                                    //        double kbsize = double.Parse(size);
                                    //        totalsize = totalsize + kbsize/ 1048576;
                                    //    }
                                    //    double n = double.Parse(size);
                                    //totalsize = totalsize + n/1000;
                                        Console.WriteLine(counter + " files - "+ results +" copied ");
                                    }
                                    else if (method == "MoveTo")
                                    {
                                        image.MoveTo(src3 + @"\" + Name);
                                        sc.Append(DateTime.Now + "  File - " + imgsource + " - " + Properties.Settings.Default.BackUpPath + @"\" + year + @"\" + monthName + @"\" + image.Extension + @"\" + Name + " "+ result +" Moved" + "\n");
                                        Console.WriteLine(DateTime.Now + "  File - " + imgsource + " - " + Properties.Settings.Default.BackUpPath + @"\" + year + @"\" + monthName + @"\" + image.Extension + @"\" + Name + " " + result + " Moved" + "\n");
                                    }
                                    filescopied++;

                                   
                                  
                                 //   Console.WriteLine(counter + " files copied");
                                    System.IO.File.WriteAllText(@"C:\Users\" + Environment.UserName + @"\Desktop" + @"\" + "Log.txt", sc.ToString());

                                }
                                catch (Exception ex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(imgsource +" - " + ex.Message.ToString());
                                    error.Append(imgsource + " - " + ex.Message.ToString());
                                    System.IO.File.WriteAllText(@"C:\Users\" + Environment.UserName + @"\Desktop" + @"\" + "MajorError.txt", error.ToString());

                                }

                        }
                        else
                        {
                          
                            //Deletes the filesif they exist in the origin folder
                            FileInfo info = new FileInfo(src3 + @"\" + Name);
                            if (info.LastWriteTime == image.LastWriteTime && Properties.Settings.Default.DeleteFiles == "Y" && Properties.Settings.Default.Method == "MoveTo")
                            {
                                image.Delete();
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("File deleted from " + imgsource);
                                sb.Append("File deleted from " + imgsource + "\n");
                            }
                            else
                            {
                                existscounter ++;
                                
                                string msg = DateTime.Now + " - File " + imgsource + " exists at " + src3 + @"\" + Name + "\n";
                              
                                sb.Append(msg);
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine(DateTime.Now + " File " + imgsource + " exists at " + src3 + @"\" + Name);
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(existscounter + " files - "+ result+" not copied - " + counter  +" Copied");
                            }

                           


                        }

                    }
                    System.IO.File.WriteAllText(@"C:\Users\" + Environment.UserName + @"\Desktop" + @"\" + "Error.txt", sb.ToString());
                    
                }
            }

        }
    }


}
