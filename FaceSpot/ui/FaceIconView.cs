
using System;
using Gtk;
using FSpot.Utils;
using Mono.Posix;
using FSpot;
using FaceSpot.Db;
using Gdk;
namespace FaceSpot
{
	public class FaceIconView : Gtk.IconView
	{
		ListStore listStore;
		
		public FaceIconView(Face[] faces) : base()
		{
			listStore = new ListStore(typeof(string),typeof(Pixbuf),typeof(Face));
			int i=0;
			foreach( Face face in faces){
				Log.Debug("Append Face#"+(i++)+"  ");
				if( face !=null)
				{
					string name = face.Name == null? "" : face.Name;
					Pixbuf pixbuf = face.iconPixbuf != null ? face.iconPixbuf.ScaleSimple(100,100,FaceSpot.IconResizeInterpType) : null ;
					if(pixbuf ==null) 
						Log.Exception(new Exception("Allowed null Face Pixbuf to the faceiconview"));
					listStore.AppendValues(name,pixbuf,face);
				}else 
					Log.Exception(new Exception("Allowed null Face input to the faceiconview"));
			}
			
			this.Model =  listStore;
			this.TextColumn = 0;
			this.PixbufColumn =1;
			
		}
	}

//	public class FaceIconView: FSpot.Widgets.IconView
//	{
//		public FaceIconView(IBrowsableCollection collection) : base(collection){
//			this.DisplayTags = true;
//			this.DisplayDates = false;
//		}
//		
//		protected override void ContextMenu(ButtonPressEventArgs args, int cell_num)
//		{
//			uint timer = Log.InformationTimerStart ("Starting FaceService");
//			string msg = Catalog.GetString ("Right Click!");
//			string desc = Catalog.GetString ("Right Click!");
//			
//			FSpot.UI.Dialog.HigMessageDialog md = new FSpot.UI.Dialog.HigMessageDialog (MainWindow.Toplevel.Window, DialogFlags.DestroyWithParent, Gtk.MessageType.Error, ButtonsType.Ok, msg, desc);
//			
//			md.Run ();
//			md.Destroy ();
//			Log.DebugTimerPrint (timer, "FaceService startup took {0}");
//		}
//	}
}
