
using System;
using Gdk;
using FSpot.Utils;
using FaceSpot.Db;
using Gtk;
using FSpot.Widgets;
using FSpot;

namespace FaceSpot
{
	
	/// <summary>
	/// Rectangle Face Overlay For Facespot
	/// 3 Actions
	/// - Image Selection
	/// - Face Selection
	/// - Browser Zoom / Move
	/// </summary>
	public class FaceOverlay : Gtk.Window //Widget
	{
		static FaceOverlay instance;

		static bool added = false;

		public static FaceOverlay Instance {
			get {
				Log.Debug("FaceOverlay Instance Called");
				if (instance == null) {
					instance = new FaceOverlay ();
				}
				if (!added) {
					try {
						MainWindow.Toplevel.Window.Add(instance);
						//MainWindow.Toplevel.PhotoView.View.Add(instance);
						added = true;
					} catch (Exception ex) {
						Log.Exception (ex);
					}
				}
				return instance;
			}
		}
		
		private PhotoImageView View{
			get {
				return 	MainWindow.Toplevel.PhotoView.View;
			}
		}
		
		//Cairo.Context ctx;
		Point old_winpos;
		private FaceOverlay () : base("FaceOverlay")
		{
			Decorated = false;
			Visible = false;
			Opacity = 0.0;
			//ModifyBg(StateType.
			Gtk.Window win = (Gtk.Window) View.Toplevel;
			win.GetPosition(out old_winpos.X,out old_winpos.Y);
			this.Move(old_winpos.X,old_winpos.Y);
			
			win.ConfigureEvent += WinConfigureEvent;
			
			TransientFor = win;
			DestroyWithParent = true;
			SkipPagerHint = true;
			SkipTaskbarHint = true;
			
			View.Item.Changed += ViewItemChanged;
			View.ZoomChanged += ViewZoomChanged;
			View.ExposeEvent += ViewExposeEvent;
			View.Hidden += ViewHidden;
			View.Shown += ViewShown;
			//TODO Add MouseOver Event
			
			AddEvents( (int) (Gdk.EventMask.ButtonReleaseMask | Gdk.EventMask.ButtonPressMask
			                  | Gdk.EventMask.PointerMotionMask ));
			ButtonPressEvent += HandleButtonPressEvent;
			ButtonReleaseEvent += HandleButtonReleaseEvent;
			MotionNotifyEvent += HandleMotionNotifyEvent;
			//ctx = CairoHelper.Create ( MainWindow.Toplevel.PhotoView.View.GdkWindow);
			//MainWindow.Toplevel.Window.ExposeEvent += MainWindowToplevelWindowExposeEvent;
			//MainWindow.Toplevel.PhotoView.View.ExposeEvent += MainWindowToplevelPhotoViewViewExposeEvent;
		}

		void HandleMotionNotifyEvent (object o, MotionNotifyEventArgs args)
		{
			//Log.Debug("HandleMotionNotifyEvent FaceOverlay ");
		}

		void HandleButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			Log.Debug(" HandleButtonReleaseEvent FaceOverlay");
		}

		void HandleButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			Log.Debug("HandleButtonPressEvent FaceOverlay");
		}

		void ViewShown (object sender, EventArgs e)
		{
			this.Show();
		}

		void ViewHidden (object sender, EventArgs e)
		{
			this.Hide();
		}

		void ViewZoomChanged (object sender, EventArgs e)
		{
			
		}

		void ViewExposeEvent (object o, ExposeEventArgs args)
		{
			
			Log.Debug("FaceOverlay Allocation Changed");
			this.Allocation = View.Allocation;
		}

		void ViewItemChanged (object sender, BrowsablePointerChangedEventArgs e)
		{
			
		}
		
		void BuildUI(){
			
			
		}

		void WinConfigureEvent (object o, ConfigureEventArgs args)
		{
			//TODO Finished This
		}

		void MainWindowToplevelWindowExposeEvent (object o, ExposeEventArgs args)
		{
			ShowOverlayFaces ();
		}

		void MainWindowToplevelPhotoViewViewExposeEvent (object o, ExposeEventArgs args)
		{
			ShowOverlayFaces ();
		}
		bool show = true;
		public void ClearOverlayFaces ()
		{
			Log.Debug("Clear Overlay Face");
			show = false;	
		}
		public void ShowOverlayFaces ()
		{
			Log.Debug("Show Overlay Face");
			show = true;
		//	this.Show();
			DrawFaceRectangles();
			//MainWindow.Toplevel.Window.ShowAll();
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose args)
		{
			ShowOverlayFaces ();
			return true;
		}
		
		private void DrawFaceRectangles(){
		//	Layout(
			this.Show();
			if (show && MainWindow.Toplevel.ViewMode == MainWindow.ModeType.PhotoView) {
				//MainWindow.Toplevel.PhotoView.View.GdkWindow.Clear();
				//MainWindow.Toplevel.PhotoView.View.O
				using (Cairo.Context ctx = CairoHelper.Create ( GdkWindow)) {
				//using (Cairo.Context ctx = CairoHelper.Create ( this.GdkWindow)) {
					//ctx.ResetClip();
					foreach (Face face in FaceSidebarWidget.Instance.knownFaceIconView.faces)
					{
						//FaceSidebarWidget.Instance.knownFaceIconView
						bool selected = FaceSidebarWidget.Instance.knownFaceIconView.SelectedFace == face;
						FillRectangle (face.Selection,ctx, true,selected );
					}
					foreach( Face face in FaceSidebarWidget.Instance.unknownFaceIconView.faces){
						bool selected = FaceSidebarWidget.Instance.unknownFaceIconView.SelectedFace == face;
						FillRectangle(face.Selection,ctx, false,selected );
					}
					//CairoUtils.CreateSurface
				}
			}
		}

		public void FillRectangle (Rectangle rect,Cairo.Context ctx,bool known,bool selected)
		{
				if(known)
					ctx.SetSourceRGBA (0, 0,0.7, 0.3);
				else 
					ctx.SetSourceRGBA (0.7, 0,0, 1.3);
			
				ctx.LineWidth = selected ? 8.2 : 0.8 ;
				Rectangle r = ImageCoordsToWindow (rect);
				Log.Debug("Fill Rectangle"+rect.ToString()+ "  >>>  "+r.ToString()  + "  ["+XOffset +","+YOffset+"]" );
				ctx.MoveTo (r.X, r.Y);
				ctx.LineTo (r.X + r.Width, r.Y);
				ctx.LineTo (r.X + r.Width, r.Y + r.Height);
				ctx.LineTo (r.X, r.Y + r.Height);
				ctx.LineTo (r.X, r.Y);
				ctx.Stroke ();
		}

		Pixbuf Pixbuf {
			get { return MainWindow.Toplevel.PhotoView.View.Pixbuf; }
		}

		double Zoom {
			get { return MainWindow.Toplevel.PhotoView.Zoom; }
		}		
		PixbufOrientation pixbuf_orientation {
			get { return MainWindow.Toplevel.PhotoView.View.PixbufOrientation; }
		}
		int XOffset {
			get { return (int)MainWindow.Toplevel.PhotoView.View.Hadjustment.Value; }
		}
		int YOffset {
			get { return (int)MainWindow.Toplevel.PhotoView.View.Vadjustment.Value; }
		}
		#region Copied From Image View
		protected Point ImageCoordsToWindow (Point image)
		{
			if (this.Pixbuf == null)
				return Point.Zero;
			ComputeScaledSize ();
			//PixbufUtils.
			
			int allocWidth = MainWindow.Toplevel.PhotoView.View.Allocation.Width;
			int allocHeight = MainWindow.Toplevel.PhotoView.View.Allocation.Height;
			
			image = FSpot.Utils.PixbufUtils.TransformOrientation (Pixbuf.Width, Pixbuf.Height, image, pixbuf_orientation);
			int x_offset = scaled_width < allocWidth ? (int)(allocWidth - scaled_width) / 2 : -XOffset;
			int y_offset = scaled_height < allocHeight ? (int)(allocHeight - scaled_height) / 2 : -YOffset;
			
			return new Point ((int)Math.Floor (image.X * (double)(scaled_width - 1) / (((int)pixbuf_orientation <= 4 ? Pixbuf.Width : Pixbuf.Height) - 1) + 0.5) + x_offset, (int)Math.Floor (image.Y * (double)(scaled_height - 1) / (((int)pixbuf_orientation <= 4 ? Pixbuf.Height : Pixbuf.Width) - 1) + 0.5) + y_offset);
		}

		protected Rectangle ImageCoordsToWindow (Rectangle image)
		{
			if (this.Pixbuf == null)
				return Gdk.Rectangle.Zero;
			ComputeScaledSize ();
			
			int allocWidth = MainWindow.Toplevel.PhotoView.View.Allocation.Width;
			int allocHeight = MainWindow.Toplevel.PhotoView.View.Allocation.Height;
			
			
			image = FSpot.Utils.PixbufUtils.TransformOrientation (Pixbuf.Width, Pixbuf.Height, image, pixbuf_orientation);
			int x_offset = scaled_width < allocWidth ? (int)(allocWidth - scaled_width) / 2 : -XOffset;
			int y_offset = scaled_height < allocHeight ? (int)(allocHeight - scaled_height) / 2 : -YOffset;
			
			Gdk.Rectangle win = Gdk.Rectangle.Zero;
			win.X = (int)Math.Floor (image.X * (double)(scaled_width - 1) / (((int)pixbuf_orientation <= 4 ? Pixbuf.Width : Pixbuf.Height) - 1) + 0.5) + x_offset;
			win.Y = (int)Math.Floor (image.Y * (double)(scaled_height - 1) / (((int)pixbuf_orientation <= 4 ? Pixbuf.Height : Pixbuf.Width) - 1) + 0.5) + y_offset;
			win.Width = (int)Math.Floor ((image.X + image.Width) * (double)(scaled_width - 1) / (((int)pixbuf_orientation <= 4 ? Pixbuf.Width : Pixbuf.Height) - 1) + 0.5) - win.X + x_offset;
			win.Height = (int)Math.Floor ((image.Y + image.Height) * (double)(scaled_height - 1) / (((int)pixbuf_orientation <= 4 ? Pixbuf.Height : Pixbuf.Width) - 1) + 0.5) - win.Y + y_offset;
			
			return win;
		}

		uint scaled_width;
		uint scaled_height;

		void ComputeScaledSize ()
		{
			if (Pixbuf == null)
				scaled_width = scaled_height = 0;
			else {
				double width;
				double height;
				if ((int)pixbuf_orientation <= 4) {
					//TopLeft, TopRight, BottomRight, BottomLeft
					width = Pixbuf.Width;
					height = Pixbuf.Height;
				} else {
					//LeftTop, RightTop, RightBottom, LeftBottom
					width = Pixbuf.Height;
					height = Pixbuf.Width;
				}
				scaled_width = (uint)Math.Floor (width * Zoom + 0.5);
				scaled_height = (uint)Math.Floor (height * Zoom + 0.5);
			}
			
			//Hadjustment.Upper = scaled_width;
			//Vadjustment.Upper = scaled_height;
		}
		#endregion
	}
}
