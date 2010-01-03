
using System;
using Emgu.CV;
using Emgu.CV.Structure;
using Gdk;
using System.IO;
using FSpot;

namespace FaceSpot
{


	public class ImageTypeConverter
	{

		public static Pixbuf ConvertCVImageToPixbuf(Emgu.CV.Image<Bgr, byte> img){
			System.Drawing.Bitmap bmp = img.Bitmap;
			MemoryStream ms = new MemoryStream();
			bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);			
			return new Pixbuf(ms.GetBuffer());
		}
		public static Emgu.CV.Image<Bgr, byte> ConvertPixbufToCVImage(Pixbuf pixbuf){			
			MemoryStream  stream = new MemoryStream();
			PixbufUtils.Save(pixbuf, stream, "jpeg", new string [] {"quality" }, new string [] { "90" });
			System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(stream);			                                                      
			Emgu.CV.Image<Bgr,byte> cvimg = new Emgu.CV.Image<Bgr, byte>(bmp);			
			return cvimg;
		}
		public static Emgu.CV.Image<Gray, byte> ConvertPixbufToGrayCVImage(Pixbuf pixbuf){			
			return ConvertPixbufToCVImage(pixbuf).Convert<Gray, byte>();
		}
	}
}
