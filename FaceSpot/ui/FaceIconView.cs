
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
		public enum Type{
			KnownFaceSidebar,
			UnknownFaceSidebar,
			KnownFaceBrowser,
			UnknownFaceBrowser
		}
		public Type type{
			get	{return _type;}
		}
			
		Type _type;
		ListStore listStore;
		
		public FaceIconView(Type type) : base()
		{
			switch (type){
				case Type.KnownFaceSidebar:
				case Type.UnknownFaceSidebar :
					listStore = new ListStore(typeof(string),typeof(Pixbuf),typeof(Face));
					break;
				case Type.KnownFaceBrowser:
				case Type.UnknownFaceBrowser:
					listStore = new ListStore(typeof(string),typeof(Pixbuf),typeof(Face),typeof(Pixbuf));
					break;
			}
			this._type = type;
			
			//this.ModifierStyle 
			//this.Style = 
			this.Model =  listStore;
			this.TextColumn = 0;
			this.PixbufColumn =1;
			//this.
			this.ButtonPressEvent += HandleButtonPressEvent;
			this.AddEvents((int)EventMask.ButtonPressMask | (int)EventMask.ButtonReleaseMask);
			this.SelectionChanged += HandleSelectionChanged;
			this.SelectionMode = SelectionMode.Multiple;
			
			UpdateFaces();
			//this.SelectionMode = SelectionMode.
//			this.ButtonReleaseEvent += HandleButtonReleaseEvent;
		}
		
		Tag tag;
		
		public FaceIconView(Type type,Tag tag) : this(type)
		{
			this.tag = tag;
		}
		
		public void UpdateFaces(){
			Face[] faces = null;
			FSpot.Photo photo = (FSpot.Photo) FaceSidebarWidget.Instance.SelectedItem;
			switch (type){
				case Type.KnownFaceSidebar:
					faces = FaceSpotDb.Instance.Faces.GetKnownFacesByPhoto(photo);
					break;
				case Type.UnknownFaceSidebar:
					faces = FaceSpotDb.Instance.Faces.GetNotKnownFacesByPhoto(photo);
					break;
				case Type.KnownFaceBrowser:
					faces = FaceSpotDb.Instance.Faces.GetConfirmedFaceByTag(Tag);
					break;
				case Type.UnknownFaceBrowser:
					faces = FaceSpotDb.Instance.Faces.GetNotConfirmedFaceByTag(Tag);
					break;
			}
			SetListStoreFaces(faces);
		}

		void SetListStoreFaces (Face[] faces)
		{
			listStore.Clear ();
			if(faces == null) return;
			foreach (Face face in faces) {
				//Log.Debug ("Append Face#" + (i++) + "  ");
				if (face != null) {
					string name = face.Name == null ? "? : #"+face.Id.ToString () : face.Name;
					Pixbuf pixbuf = face.iconPixbuf != null ? face.iconPixbuf.ScaleSimple (FaceSpot.THUMBNAIL_SIZE, FaceSpot.THUMBNAIL_SIZE, FaceSpot.IconResizeInterpType) : null;
					if (pixbuf == null)
						Log.Exception (new Exception ("Allowed null Face Pixbuf to the faceiconview"));
					listStore.AppendValues (name, pixbuf, face);
				} else
					Log.Exception (new Exception ("Allowed null Face input to the faceiconview"));
			}
		}
//		void HandleButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
//		{
//			Log.Debug("Button Released on Face Iconview");
//			if(args.Event.Button == 3){
//				FacePopupMenu popup = new FacePopupMenu();
//				popup.Activate();
//			}
//		}

		void HandleSelectionChanged (object sender, EventArgs args) {}
		
		public Face SelectedFace{
			get {
				List<Face> selectedFaces = SelectedFaces;
				return selectedFaces.Count == 1 ? selectedFaces[0] : null;
			}
			set {
				TreeIter iter;
				listStore.GetIterFromString(out iter,value.Name);
				this.SelectPath(listStore.GetPath(iter));
			}
		}
		
		public List<Face> SelectedFaces{
			get {
				TreePath[] paths = this.SelectedItems;
				List< Face> selectedFaces = new List<Face>(); // for group selection
				
				foreach(TreePath path in paths){
					TreeIter iter;
					listStore.GetIter(out iter,path);
					Face f = (Face) listStore.GetValue(iter,2);
					selectedFaces.Add(f);
				}
				return selectedFaces;
			}
			set{
				List<Face> faceList = value;
				this.UnselectAll();
				foreach( Face f in faceList){
					TreeIter iter;
					listStore.GetIterFromString(out iter,f.Name);
					this.SelectPath(listStore.GetPath(iter));
				}
			}
		}

		public Tag Tag {
			get {
				return tag;
			}
			set {
				tag = value;
				UpdateFaces();
			}
		}
		
		void HandleButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if(args.Event.Button == 3){
				
				TreePath facePath;
				CellRenderer faceCell;
				Face selectedFace = null;
				TreeIter faceIter;
				this.GetItemAtPos((int)args.Event.X,(int)args.Event.Y,out facePath,out faceCell);
				if(facePath==null) return;
	
				listStore.GetIter(out faceIter,facePath);
				try{
					selectedFace = (Face) listStore.GetValue(faceIter,2);
				}finally{}
				
				Log.Debug("Button Pressed on Face :"+selectedFace.Id);
				
				bool isInSelection = false;
				
				List<Face> selectedFaces = SelectedFaces;
				foreach(Face f in selectedFaces){
					if(f.Equals(selectedFace)){
							isInSelection = true;
						}
				}
				
				if(!isInSelection){
					this.UnselectAll();
					this.SelectPath(facePath);	
					selectedFaces.Clear();
					selectedFaces.Add(selectedFace);
				}
				String fids = "";
				foreach(Face f in selectedFaces){
					fids += f.Id + " ";
				}
				Log.Debug("With+"+ selectedFaces.Count + " Selection :"+fids);
				FaceIconViewPopupMenu popup = new FaceIconViewPopupMenu();
				popup.Activate(args.Event,selectedFace,selectedFaces.ToArray(),this);
				
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
