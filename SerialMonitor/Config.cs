using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SerialMonitor
{
	public abstract class Config
	{

		protected Dictionary<string,string> ConfigFile= new Dictionary<string, string>();
		protected Dictionary<string,string> CommandArgs = new Dictionary<string, string>();

		public Config (string file=null, string[] cmdlineArgs = null)
		{

			//readCommand config filke
			if (file != null) {
				ConfigFile = readConfig (file);
			}
			//read args
			if (cmdlineArgs != null) {
				CommandArgs = readArgs (cmdlineArgs);
			}
			//asign values
			AssignValues ();

		}
		protected virtual void AssignValues(){
			FieldInfo[] fields = this.GetType ().GetFields();
			for (var i = 0; i < fields.Length; i++) {
				if (ConfigFile.ContainsKey (fields [i].Name)) {

				}
				if (CommandArgs.ContainsKey (fields [i].Name)) {

				}

			}
		}

		protected void AssignField(FieldInfo field, string value){
			try{
				field.SetValue (this, value);
			}
			catch(Exception ex){
				Console.WriteLine("failed to set value for "+field.Name+
					" ("+value+")\n"+ex.Message);
			}
		}

		//read the config file into a dictopary
		protected  Dictionary<string,string> readConfig (string filename)
		{
			Dictionary<string,string> dic = new Dictionary<string, string> ();
			try {
				var lines = File.ReadLines (filename);
				foreach (string line in lines) {
					string tmp = line;
					if (line.IndexOf ("#") >= 0) {
						tmp = line.Substring (0, line.IndexOf ("#"));
					}
					if (tmp.Length > 3) {
						var strings = tmp.Split ('=');
						if (strings.Length == 2) {
							dic.Add (strings [0].Trim (), strings [1].Trim ());
						}
					}

				}
			} catch (Exception ex) {
				Console.WriteLine ("ERROR: cannot read config file.");
				Console.WriteLine ("Reason:" + ex.Message);
				Console.WriteLine ("trace:" + ex.StackTrace);
			}

			return dic;

		}

		protected  Dictionary<string,string> readArgs (string[] args)
		{
			Dictionary<string,string> dic = new Dictionary<string, string> ();
				
			string key;
			string command_line_flags = "";
			FieldInfo[] fields = this.GetType ().GetFields();
			for (int i = 0; i < args.Length; i++) {

				key = args [1].TrimStart('-');
				bool oneOfUs = false;
				for (int j = 0; j < fields.Length; j++) {
					if (fields [j].Name == key) {
						oneOfUs = true;
						break;
					}
				}

				if(oneOfUs && i+1 < args.Length){
					dic.Add(key, args[i++]);
				}
				else{
					command_line_flags+= args[i];
				}


			}

			dic.Add ("command_line_flags", command_line_flags);

			return dic;
		}

		public virtual new string ToString(){
			StringBuilder sb = new StringBuilder ();
			sb.Append ("Config File:\n");
			foreach(string key in ConfigFile.Keys){
				sb.AppendFormat ("   {0}={1}\n", key, ConfigFile [key]);
			}
			sb.Append ("Command Line:\n");
			foreach(string key in CommandArgs.Keys){
				sb.AppendFormat ("   {0}={1}\n", key, CommandArgs [key]);
			}
			return sb.ToString ();
		}

	}
}

