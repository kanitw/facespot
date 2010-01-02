
using System;
using System.Collections.Generic;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

using FaceSpot.Db;

namespace FaceSpot
{	
	/// <summary>
	/// Note that face image loaded in gray scale not bgr. Conversion may be needed.
	/// </summary>
	public class FaceTag
	{
		public string tag;		
		public Image<Gray, Byte> faceImage;
		
		public FaceTag(string tag, Image<Gray, Byte> faceImage)
		{
			this.tag = tag;			
			this.faceImage = faceImage;			
		}
		public FaceTag(Face f){
			this.tag = f.tag.Name;			
			this.faceImage = ImageTypeConverter.ConvertPixbufToCVImage(f.iconPixbuf).Convert<Gray, byte>();
		}
	}
}
