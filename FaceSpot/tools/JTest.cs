
using Gdk;
using Gtk;

using System;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using System.Reflection;


using FSpot;


namespace FaceSpot
{


	public class JTest
	{
		const string facedbPath = "/home/hyperjump/faces/";
		public JTest ()
		{
											
		}
		
		/// <summary>
		/// Face Detection Tester
		/// </summary>
		public static void Main(string[] arg){
			//TestDetect();
			//Assembly _assembly = Assembly.GetExecutingAssembly();
			
			//Stream _xmlStream = _assembly.GetManifestResourceStream("FaceSpot.tools.haarcascade.haarcascadde_eye.xml");			
			
			
//				Assembly _assembly = Assembly.GetExecutingAssembly();
//			ManifestResourceInfo info =  _assembly.GetManifestResourceInfo("FaceSpot.tools.haarcascade.haarcascade_eye.xml");
//			Console.WriteLine("---"+info.FileName);
//			String location = _assembly.CodeBase;
//			UriBuilder uri = new UriBuilder(location);
//            string path = Uri.UnescapeDataString(uri.Path);
//
//			String xmlDirpath = Path.GetDirectoryName(path);
//			Console.WriteLine("============== "+ xmlDirpath);
			//Console.WriteLine("{0},{1}",1,2);
			//ImageFile imageFile = new ImageFile(facedbPath + "testDetect/cp33.jpg");
			//Console.WriteLine( FSpot.Utils.UriUtils.PathToFileUri(facedbPath + "testDetect/cp33.jpg") );
			//byte [] image_data = PixbufUtils.Save (pixbuf, "jpeg", new string [] {"quality" }, new string [] { quality.ToString () });
			
			//new Pixbuf(facedbPath + "testDetect/cp33.jpg");
			//Console.WriteLine(PhotoStore.
			//Emgu.CV.Image<Gray, Byte> mm = (new Emgu.CV.Image<Gray, Byte>("cp33.jpg"));						
			//Gdk.Pixbuf pb = new Gdk.Pixbuf("cp33.jpg");
			
			//FSpot.Photo photo = new PhotoStore(MainWindow.Toplevel.Database,false).Get(16);
			//TestDetect ();		
			
			//Photo s = MainWindow.Toplevel.Database.Photos.Get(186);			
			//PhotoStore s = MainWindow.Toplevel.Database.Photos;
			//Tag peopleTag = MainWindow.Toplevel.Database.Tags.GetTagByName ("People");						
				
			//Emgu.CV.Image<Bgr, byte> ccc = new Emgu.CV.Image<Bgr, byte>(new System.Drawing.Size(100,100));
		}

		static void TestDetect ()
		{
			string[] testSet = {
				"girlgen",
				"cp33",				
				"girlgen2",
				"cp33all"
			};
			string testPath = facedbPath + "testDetect/";
			string testSavePath = testPath + "output/";
			if (Directory.Exists (testSavePath))
				Directory.Delete (testSavePath, true);
			Directory.CreateDirectory (testSavePath);
			foreach (string s in testSet) {
				int sec = DateTime.Now.Second;
				//Image<Bgr, Byte> aaa = new Image<Bgr, Byte> (testPath + s + ".jpg");
				
				FaceImagePos[] faceImagePos = FaceDetector.DetectFace (new Image<Bgr, Byte> (testPath + s + ".jpg"));
				Image<Bgr, Byte>[] testDetect = new Image<Bgr, Byte>[faceImagePos.Length];
				for(int i=0;i<faceImagePos.Length;i++)
					testDetect[i] = faceImagePos[i].image;
				
				int interval = DateTime.Now.Second - sec;
				if (interval < 0)
					interval += 60;
				Console.WriteLine ("time = " + interval);
				for (int i = 0; i < testDetect.Length; i++)
					testDetect[i].Save (testSavePath + s + "_" + i + ".jpg");
			}
			Console.WriteLine ("end of TestDetect");
		}

	}
}
