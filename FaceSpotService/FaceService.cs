
using System;
using System.Drawing;
using System.Threading;

using FSpot.Extensions;
using FSpot.Utils;
using FSpot;
using Gtk;
using Mono.Unix;

using Emgu.CV.Structure;
using FaceSpot;
using FaceSpot.Db;

namespace FaceSpot
{
	public class FaceService : IService
	{
		public FaceService (){}
		
		public bool Start ()
		{
			uint timer = Log.InformationTimerStart ("Starting FaceService");
			string msg = Catalog.GetString ("Face Service Start");
			string desc = Catalog.GetString ("Sample Alert Box");
			
			FSpot.UI.Dialog.HigMessageDialog md = new FSpot.UI.Dialog.HigMessageDialog (MainWindow.Toplevel.Window, DialogFlags.DestroyWithParent, Gtk.MessageType.Error, ButtonsType.Ok, msg, desc);
			md.Run ();
			md.Destroy ();
			FaceScheduler.Instance.Execute();
			Log.DebugTimerPrint (timer, "FaceService startup took {0}");
			
			//TestDetect(timer);
			
			return true;
		}
		
		public bool Stop ()
		{
			uint timer = Log.InformationTimerStart ("Stopping FaceService");
			Log.DebugTimerPrint (timer, "FaceService shutdown took {0}");
			return true;
		}
		
		public void TestDetect(uint timer){
			for(int i=0;i<4;i++){
				Photo p = MainWindow.Toplevel.Database.Photos.Get((uint)(i+201));
				
//				FacePixbufPos[] faces = FaceDetector.DetectToPixbuf(p);
								         	
//				for(int j=0;j<faces.Length;j++)
//					faces[j].pixbuf.Save("out/job_"+i+"_"+j+".jpeg","jpeg");
				
//				FaceStore faceStore = FaceSpotDb.Instance.Faces;
//				
//				foreach(FacePixbufPos f in faces){
//					Face face = faceStore.CreateFace(f.photo, f.leftX, f.topY, (uint)f.pixbuf.Width, f.pixbuf, null, false, true, false);
//					faceStore.Commit(face);
//					FaceSpotDb.Instance.PhotosAddOn.SetIsDetected(f.photo.DefaultVersion, true);
//				}
			}
		}
		public void F(uint timer){
			Photo p = MainWindow.Toplevel.Database.Photos.Get(186);
			Log.DebugTimerPrint(timer,"path = " + p.DefaultVersionUri.AbsolutePath);
			Gdk.Pixbuf p186 = new Gdk.Pixbuf(p.DefaultVersionUri.AbsolutePath);
			p186.Save("jump","jpeg");
			
			
			Gdk.Pixbuf pixbuf = new Gdk.Pixbuf("/home/hyperjump/faces/testDetect/cp33.jpg");
			Log.DebugTimerPrint (timer, "================");															
			byte [] testdata = pixbuf.SaveToBuffer("jpeg", new string [] {"quality" }, new string [] { "100" });
			Log.DebugTimerPrint (timer, "test data length = " +testdata.Length);			
			
			pixbuf.Save("temp","jpeg");
			
			byte [] pixbuf_bmpbyte = pixbuf.SaveToBuffer("bmp");
			System.IO.MemoryStream m = new System.IO.MemoryStream(pixbuf_bmpbyte);
			
			
			
			Bitmap bmpt = new Bitmap(m);
			Emgu.CV.Image<Bgr, byte> blankImage = new Emgu.CV.Image<Bgr, byte>(pixbuf.Width,pixbuf.Height);
			
			blankImage.Bitmap = bmpt;			
			blankImage.Save("haha");
			
			//Emgu.CV.Image<Bgr, Byte> ccc = new Emgu.CV.Image<Bgr, Byte>(100,100);
			//Emgu.CV.Image<Gray, Byte> img = new Emgu.CV.Image<Gray, Byte>("/home/hyperjump/faces/testDetect/cp33.jpg");
			//Emgu.CV.Image<Bgr, Byte> cPicture = new Emgu.CV.Image<Bgr, Byte>("/home/hyperjump/faces/testDetect/cp33.jpg");
			
			
			Emgu.CV.Image<Bgr, byte> ccc = new Emgu.CV.Image<Bgr, byte>("temp");

			
			ccc.Save("abc.jpg");
			System.Drawing.Bitmap bmp = ccc.Bitmap;
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
			
			
				
			//ccc.Bytes = testdata;
//			for(int i=0;i<100;i++)
//				Log.DebugTimerPrint( timer, ccc.Bytes[i].ToString());
//			
//				Log.DebugTimerPrint( timer, "---------------------");
//			for(int i=0;i<100;i++)
//				Log.DebugTimerPrint( timer, testdata[i].ToString());
//			
//			Log.DebugTimerPrint( timer, testdata[testdata.Length-1].ToString());
//			Log.DebugTimerPrint( timer, testdata[testdata.Length-2].ToString());
			
			
			Gdk.Pixbuf loadedPix = new Gdk.Pixbuf("abc.jpg");
			
			loadedPix.Save("zzz.jpg","jpeg");
			
						

			//loadedPix = new Gdk.Pixbuf(intPtr,
			loadedPix = new Gdk.Pixbuf(ms.GetBuffer());
				
			loadedPix.Save("buffer.jpg","jpeg");
		}
	}
	
	//public lass
}
