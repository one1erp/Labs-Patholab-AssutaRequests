using Microsoft.Win32;
using Oracle.ManagedDataAccess.Client;
//using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AssutaRequests;
using Patholab_DAL_V1;

namespace AssutaRequests
{

    public class Program
    {
        public const string ServiceName = "Assuta Requests Service";

        private static DataLayer _dal;
        public static string NautConStr;
        public static string InputPath;
        public static string OuputPath;
        public static string InstrumentInput;
        private static string LogPath;


        static void Main(string[] args)
        {
            if (!(System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1))
            {
                Program.log("AssutaRequests Started");
                SetAppSettings();

                log("CONNECTING TO DB");
                _dal = new DataLayer();
                _dal.MockConnect(NautConStr);
                log("Connected TO DB");

                
                log("Get system Parameters");
                var systemParams = _dal.GetPhraseByName("System Parameters");

                log("Get Interface Assuta Parameters");
                var InterfaceParams = _dal.GetPhraseByName("Assuta Interface Parameters");


                XML2Nautilus xml2nautilus = new XML2Nautilus(_dal, InterfaceParams);
                xml2nautilus.Run();

                Send2Instrument s = new Send2Instrument(_dal, systemParams);
                s.Run();

                Arrived2Nautilus ar = new Arrived2Nautilus(_dal, InterfaceParams);
                ar.Run();

                ReadyToWork rw = new ReadyToWork(_dal, InterfaceParams);
                rw.Run();

                UpdateCancelRequest urq = new UpdateCancelRequest(_dal, InterfaceParams);
                urq.Run();

                log("Disconnect from DB");
                _dal.Close();
                _dal = null;
                log("End Program");
            }
            else
            {
                log("Program already running, canceling current program");
            }
            
        }

        private static void Exit(string p)
        {
            log("Exit Program");
            log(p);

        }

        private static void SetAppSettings()
        {
            NautConStr = ConfigurationManager.ConnectionStrings["NautConnectionString"].ConnectionString;
            InputPath = ConfigurationManager.AppSettings["InputPath"];
            OuputPath = ConfigurationManager.AppSettings["OutputPath"];
            InstrumentInput = ConfigurationManager.AppSettings["InstrumentInput"];
            LogPath = ConfigurationManager.AppSettings["LogPath"];
            Program.log(string.Format("Connection string is {0} \n xml Input is {1} \n xml Output is {2} InstrumentInput is {3}"
                , NautConStr, InputPath, OuputPath, InstrumentInput));
        }

        

        public static void log(string s)
        {
            try
            {
                using (FileStream file = new FileStream(LogPath + DateTime.Now.ToString("dd-MM-yyyy") + ".log", FileMode.Append, FileAccess.Write))
                {
                    var streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(s);
                    streamWriter.Close();
                }
            }
            catch
            {
            }

            //Console.WriteLine(s);
            //Patholab_Common.Logger.WriteLogFile(s);
        }
        public static void log(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Patholab_Common.Logger.WriteLogFile(ex);

            if (ex.InnerException != null)
            {
                Console.WriteLine(ex.InnerException);
            }
        }




    }



}
