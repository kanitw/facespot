
using System;
using FSpot;
using Gdk;
using FSpot.Widgets;
using FSpot.Utils;
	
namespace FaceSpot.Db
{
	public class Face : DbItem, IDisposable
		//TODO Decide whether should it implement IComparable
	{
		uint faceID;		
		
		uint leftX,topY,width;	
		#region Position/Width Encapsulation
		public uint LeftX {
			get {
				return leftX;
			}
			set {
				leftX = value;
			}
		}

		public uint TopY {
			get {
				return topY;
			}
			set {
				topY = value;
			}
		}

		public uint Width {
			get {
				return width;
			}
			set {
				width = value;
			}
		}
		#endregion
		//TODO design how to add thumbnails
		const int faceDefaultWidth = 100;
		public Pixbuf faceImage;
		
		bool manuallyDetected, manuallyRecognized;
		
		string photo_md5;
		
		public Photo photo;
		
		public Pixbuf pixbuf;
		/// <summary>
		/// 
		/// </summary>
		public Tag tag;
		/// <summary>
		/// 
		/// </summary>
		public bool tagConfirmed;
		
		public Face (uint id,uint leftX,uint topY,uint width,Photo photo) 
			: base (id)
		{
			this.faceID = id;
			this.leftX = leftX;
			this.topY = topY;
			this.width = width;
			this.photo = photo;
			//TODO Possible Error
			photo_md5 = photo.MD5Sum;
			
			PhotoImageView view = MainWindow.Toplevel.PhotoView.View;
			Pixbuf input = view.Pixbuf;
			Log.Debug("Face: Creating Pixbuf");
			Rectangle selection = FSpot.Utils.PixbufUtils.TransformOrientation ((int)view.PixbufOrientation <= 4 ? input.Width : input.Height,
											    (int)view.PixbufOrientation <= 4 ? input.Height : input.Width,
											    view.Selection, view.PixbufOrientation);
			pixbuf = new Pixbuf(input.Colorspace,input.HasAlpha,input.BitsPerSample,
				                            selection.Width,selection.Height);
			input.CopyArea (selection.X, selection.Y,
					selection.Width, selection.Height, pixbuf, 0, 0);
		}
		
		//TODO Add Function for move/scale(1:1) Face
		
		public void Dispose()
		{
			//TODO Add required child item dispose
			
			System.GC.SuppressFinalize(this);
		}
		public static PixbufCache getFaceImageFromPhoto(uint left,uint top,uint width, Photo photo)
		{
			throw new NotImplementedException();	
		}
	}
}
