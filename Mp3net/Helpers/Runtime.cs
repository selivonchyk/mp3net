using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Text;
using System.Threading;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace Mp3net.Helpers
{
    public class Runtime
	{
		private static Runtime instance;

		internal int AvailableProcessors ()
		{
			return Environment.ProcessorCount;
		}

		internal static long CurrentTimeMillis ()
		{
			return DateTime.UtcNow.ToMillisecondsSinceEpoch ();
		}

        internal static string Getenv (string var)
		{
			return Environment.GetEnvironmentVariable (var);
		}

		internal static IDictionary<string, string> GetEnv ()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string> ();
			foreach (DictionaryEntry v in Environment.GetEnvironmentVariables ()) {
				dictionary[(string)v.Key] = (string)v.Value;
			}
			return dictionary;
		}

		internal static IPAddress GetLocalHost ()
		{
			return Dns.GetHostEntry (Dns.GetHostName ()).AddressList[0];
		}
		
		static Hashtable properties;
		
		public static Hashtable GetProperties ()
		{
			if (properties == null) {
				properties = new Hashtable ();
				properties ["jgit.fs.debug"] = "false";
				var home = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile).Trim ();
				if (string.IsNullOrEmpty (home))
					home = Environment.GetFolderPath (Environment.SpecialFolder.Personal).Trim ();
				properties ["user.home"] = home;
				properties ["java.library.path"] = Environment.GetEnvironmentVariable ("PATH");
				if (Path.DirectorySeparatorChar != '\\')
					properties ["os.name"] = "Unix";
				else
					properties ["os.name"] = "Windows";
			}
			return properties;
		}

		public static string GetProperty (string key)
		{
			return ((string) GetProperties ()[key]);
		}
		
		public static void SetProperty (string key, string value)
		{
			GetProperties () [key] = value;
		}

		public static Runtime GetRuntime ()
		{
			if (instance == null) {
				instance = new Runtime ();
			}
			return instance;
		}

		internal static int IdentityHashCode (object ob)
		{
			return RuntimeHelpers.GetHashCode (ob);
		}

		internal long MaxMemory ()
		{
			return int.MaxValue;
		}
		
		internal static void DeleteCharAt (StringBuilder sb, int index)
		{
			sb.Remove (index, 1);
		}
		
		internal static byte[] GetBytesForString (string str)
		{
			return Encoding.UTF8.GetBytes (str);
		}

		internal static byte[] GetBytesForString (string str, string encoding)
		{
			return Encoding.GetEncoding (encoding).GetBytes (str);
		}

		internal static FieldInfo[] GetDeclaredFields (Type t)
		{
			return t.GetFields (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		}

		internal static void NotifyAll (object ob)
		{
			Monitor.PulseAll (ob);
		}

		internal static void PrintStackTrace (Exception ex)
		{
			Console.WriteLine (ex);
		}

		internal static void PrintStackTrace (Exception ex, TextWriter tw)
		{
			tw.WriteLine (ex);
		}

		internal static string Substring (string str, int index)
		{
			return str.Substring (index);
		}

		internal static string Substring (string str, int index, int endIndex)
		{
			return str.Substring (index, endIndex - index);
		}

		internal static void Wait (object ob)
		{
			Monitor.Wait (ob);
		}

		internal static bool Wait (object ob, long milis)
		{
			return Monitor.Wait (ob, (int)milis);
		}
		
		internal static Type GetType (string name)
		{
			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies ()) {
				Type t = a.GetType (name);
				if (t != null)
					return t;
			}
			throw new InvalidOperationException ("Type not found: " + name);
		}
		
		internal static void SetCharAt (StringBuilder sb, int index, char c)
		{
			sb [index] = c;
		}
		
		internal static bool EqualsIgnoreCase (string s1, string s2)
		{
			return s1.Equals (s2, StringComparison.CurrentCultureIgnoreCase);
		}
		
		internal static long NanoTime ()
		{
			return Environment.TickCount * 1000 * 1000;
		}
		
		internal static int CompareOrdinal (string s1, string s2)
		{
			return string.CompareOrdinal (s1, s2);
		}

		internal static string GetStringForBytes (byte[] chars)
		{
			return Encoding.UTF8.GetString (chars);
		}

		internal static string GetStringForBytes (byte[] chars, string encoding)
		{
			return GetEncoding (encoding).GetString (chars);
		}

		internal static string GetStringForBytes (byte[] chars, int start, int len)
		{
			return Encoding.UTF8.GetString (chars, start, len);
		}

		internal static string GetStringForBytes (byte[] chars, int start, int len, string encoding)
		{
			return GetEncoding (encoding).Decode (chars, start, len);
		}
		
		internal static Encoding GetEncoding (string name)
		{
//			Encoding e = Encoding.GetEncoding (name, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
			Encoding e = Encoding.GetEncoding (name.Replace ('_','-'));
			if (e is UTF8Encoding)
				return new UTF8Encoding (false, true);
			return e;
		}
	}
}
