
using System;
using Gtk;
using FSpot.Utils;
using Mono.Posix;
using FSpot;
using FaceSpot.Db;
using Gdk;
using System.Collections.Generic;
namespace FaceSpot
{
	//TODO Add support for deselection of another one
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
					string name = face.Name == null? face.Id.ToString() : face.Name;
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
			this.ButtonPressEvent += HandleButtonPressEvent;
			this.AddEvents((int)EventMask.ButtonPressMask | (int)EventMask.ButtonReleaseMask);
			this.SelectionChanged += HandleSelectionChanged;
			this.SelectionMode = SelectionMode.Multiple;
//			this.ButtonReleaseEvent += HandleButtonReleaseEvent;
			//this.
		}

//		void HandleButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
//		{
//			Log.Debug("Button Released on Face Iconview");
//			if(args.Event.Button == 3){
//				FacePopupMenu popup = new FacePopupMenu();
//				popup.Activate();
//			}
//		}

		void HandleSelectionChanged (object sender, EventArgs args)
		{
			
		}

		void HandleButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if(args.Event.Button == 3){
				
				TreePath facePath;
				CellRenderer faceCell;
				TreeIter faceIter;
				this.GetItemAtPos((int)args.Event.X,(int)args.Event.Y,out facePath,out faceCell);
				listStore.GetIter(out faceIter,facePath);
				Face face = (Face) listStore.GetValue(faceIter,2);
				
				Log.Debug("Button Pressed on Face :"+face.Id);
				
				TreePath[] paths = this.SelectedItems;
				List< Face> faces = new List<Face>(); // for group selection
				
				bool isInSelection = false;
				foreach(TreePath path in paths){
					if(path.Equals(facePath)){
						isInSelection = true;
					}
					TreeIter iter;
					listStore.GetIter(out iter,path);
					Face f = (Face) listStore.GetValue(iter,2);
					faces.Add(f);
				}
				if(!isInSelection){
					this.UnselectAll();
					this.SelectPath(facePath);	
					faces.Clear();
					faces.Add(face);
				}
				String fids = "";
				foreach(Face f in faces){
					fids += f.Id + " ";
				}
				
				Log.Debug("With+"+ faces.Count + " Selection :"+fids);
				FacePopupMenu popup = new FacePopupMenu();
				popup.Activate(args.Event,face,faces.ToArray());
				
				args.RetVal =true;
			}
		}
		
		//TODO if have time - Add Keyboard Support
		//TODO Add Menu to Edit Menu Bar for this
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
