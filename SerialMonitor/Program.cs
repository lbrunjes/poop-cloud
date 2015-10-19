using System;
using System.IO.Ports;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace SerialMonitor
{
	class SerialMonitorConfig:Config{
		public string portname = "/dev/ttyACM0";
		public int baudrate = 9600;
		public int readtimeout = 10000;
		public int writetimeout = 500;
		public string url = "http://127.0.0.1:8080/Service.ashx";
		public string username = "admin";
		public string password = "password";
		public Dictionary<int, string> networkCommands = new Dictionary<int, string>();
		public TimeSpan DisableTimer = new TimeSpan (0, 0, 5);

		public SerialMonitorConfig(string file, string[] args):base (file, args){

		}

		protected override void AssignValues ()
		{
			base.AssignValues ();

			//deal with the network commands...
			String join = "?type=babyevents&id="+this.ConfigFile["babyid"]
				+"&eventtype={0}&subtype={1}&details={2}";
			networkCommands.Add (2, String.Format (join,
				ConfigFile ["networkCommands:2"].Split (',')));
			networkCommands.Add (3, String.Format (join,
				ConfigFile ["networkCommands:3"].Split (',')));
			networkCommands.Add (4, String.Format (join,
				ConfigFile ["networkCommands:4"].Split (',')));
			networkCommands.Add (5, String.Format (join,
				ConfigFile ["networkCommands:5"].Split (',')));
		}

	}

	class MainClass
	{
		public static SerialMonitorConfig activeConfig;
		protected static Dictionary<int, DateTime> timeouts =new Dictionary<int, DateTime>();

		public static void Main (string[] args)
		{
			activeConfig = new SerialMonitorConfig ("Serial.conf",args);

			//assign time outs to all network commands
			foreach (int  i in activeConfig.networkCommands.Keys) {
				timeouts.Add (i, DateTime.Now);
			}

			stamp ();
		
			SerialPort serialPort = new SerialPort ();
			serialPort.BaudRate = activeConfig.baudrate;
			serialPort.PortName = activeConfig.portname;
		//serialPort.Parity = activeConfig.Parity;
			serialPort.ReadTimeout = activeConfig.readtimeout;
			serialPort.WriteTimeout = activeConfig.writetimeout;

			serialPort.Open ();
			string message;
			while (true) {
				try{
					message= serialPort.ReadLine ();

					int cmd = 0;
					if(int.TryParse(message, out cmd)){
						readCommand(cmd);
					}


				
				}
				catch(Exception ex){
					//
				}
			}
		}
		public static void stamp(){
			Console.WriteLine (@"
SERIAL MONITOR
For use with BabyData
CONFIG:
");
			Console.WriteLine (activeConfig.ToString ());
		}


		public static void readCommand (int value){
			//read in teh command.
			if (value > 1 && value < 6) {
				//we pushed teh button
				if (DateTime.Now > timeouts [value]) {
					timeouts [value] = DateTime.Now.AddSeconds (5);
					MakeRequest (activeConfig.url + activeConfig.networkCommands [value]);

				}
			} else if (value == 6) {
				Console.WriteLine ("toggle sleep state");
				if (activeConfig.networkCommands [5].Contains ("up")) {
					activeConfig.networkCommands [5] = activeConfig.networkCommands [5].Replace ("up", "down");
				} else {
					activeConfig.networkCommands [5] = activeConfig.networkCommands [5].Replace ("down", "up");
				}
			} else {
				Console.WriteLine ("invalid option Expected [2-6] got:" + value);
			}
		}


		public static void MakeRequest(string url){
			Console.WriteLine("REQUEST: "+url);
			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create (url);
			req.Method = "POST";
			req.Headers ["Authorization"] = "Basic " +
			Convert.ToBase64String (Encoding.Default.GetBytes (
					activeConfig.username+":"+activeConfig.password
			));

			StreamReader r = new StreamReader(req.GetResponse ().GetResponseStream());

			Console.WriteLine(r.ReadToEnd());
			

		}

	}
}
