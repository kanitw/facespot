
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace FaceSpot
{


	public class SerializeUtil
	{

		public static void Serialize(string path, Object obj)
		{						
			Stream s = File.OpenWrite(path);
			BinaryFormatter b = new BinaryFormatter();			
			b.Serialize(s,obj);
			s.Close();						
		}
		
		public static Object DeSerialize(string path)
		{							
			Stream s = File.OpenRead(path);
			BinaryFormatter b = new BinaryFormatter();			
			Object obj = b.Deserialize(s);			
			s.Close();			
			return obj;
		}
	}
}
