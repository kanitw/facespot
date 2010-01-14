
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
	//TODO Add support for Multiple Version Photo
	public class FaceIconView : Gtk.IconView
	{
		public enum Type{
			KnownFaceSidebar,
			UnknownFaceSidebar,
			KnownFaceBrowser,
			SuggestedFaceBrowser,
			UnknownFaceBrowser
		}
		
		public bool IsBrowserType{
			get {return type == Type.KnownFaceBrowser ||
				type== Type.UnknownFaceBrowser ||
				type == Type.SuggestedFaceBrowser;}
		}
		public bool IsSideBarType {
			get {
				return type == Type.KnownFaceSidebar || 
				 type==	Type.UnknownFaceSidebar;
			}
		}
		
		private bool isShowFullImage = false;
		
		public Type type{
			get	{return _type;}
		}
			
		Type _type;
		ListStore listStore;
		public Face[] faces;
		
		public FaceIconView(Type type) : base()
		{
			switch (type){
				case Type.KnownFaceSidebar:
				case Type.UnknownFaceSidebar :
					listStore = new ListStore(typeof(string),typeof(Pixbuf),typeof(Face),typeof(string));
					break;
				case Type.KnownFaceBrowser:
				case Type.SuggestedFaceBrowser:
				case Type.UnknownFaceBrowser:
					listStore = new ListStore(typeof(string),typeof(Pixbuf),typeof(Face),typeof(string),typeof(Pixbuf));
					break;
			}
			this._type = type;
			
			//this.ModifierStyle 
			//this.Style = 
			this.Model =  listStore;
			this.TextColumn = 0;
			this.PixbufColumn =1;
			this.TooltipColumn = 3;
			//this.
			this.ButtonPressEvent += HandleButtonPressEvent;
			this.AddEvents((int)EventMask.ButtonPressMask | (int)EventMask.ButtonReleaseMask);
			this.SelectionChanged += HandleSelectionChanged;
			this.SelectionMode = SelectionMode.Multiple;
			Log.Debug("New Face IconView "+type.ToString());
			UpdateFaces();
			
//				Spacing = 0;
//				RowSpacing =0 ;
//				ColumnSpacing =0;
//				Margin =0;
			FaceSpotDb.Instance.Faces.ItemsChanged += FaceSpotDbInstanceFacesItemsChanged;
			//this.SelectionMode = SelectionMode.
//			this.ButtonReleaseEvent += HandleButtonReleaseEvent;
		}

		void FaceSpotDbInstanceFacesItemsChanged (object sender, DbItemEventArgs<Face> e)
		{
			//Log.Debug(type.ToString() + "Handle FaceDbInstanceChanged from " + sender.ToString() + "  " + e.ToString());
			UpdateFaces();
		}
		
		Tag tag;
		
		public FaceIconView(Type type,Tag tag) : this(type)
		{
			this.tag = tag;
		}
		
		public void UpdateFaces(){
			Face[] faces = null;
			//Log.Debug(this.type.ToString() + " : Update Faces");
			FSpot.Photo photo = null;
			if(FaceSidebarWidget.Instance != null)
			photo = (FSpot.Photo) FaceSidebarWidget.Instance.SelectedItem;
			//Log.Debug("Get Faces");
			if(!isShowFullImage){
				ItemWidth =50;
				//Font
			}
			try {
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
					case Type.SuggestedFaceBrowser:
						faces = FaceSpotDb.Instance.Faces.GetNotConfirmedFaceByTag(Tag);
						break;
					case Type.UnknownFaceBrowser:
						faces = FaceSpotDb.Instance.Faces.GetUntaggedFace("");
						break;	
				}
			} catch(Exception ex) {
				Log.Exception(ex);
			}
			if(faces!=null && faces.Length > 0)
				SetListStoreFaces(faces);
		}

		void SetListStoreFaces (Face[] faces)
		{						
			
			try {
				Log.Debug(">>> SetListStoreFaces Called");
				if(listStore != null)
					listStore.Clear ();
				
				this.faces = faces;
				
				int i=0;
				foreach (Face face in faces) {
					Log.Debug ("Append Face#" + (i++) + "  ");
					if (face != null && listStore != null) {
						string name = face.Name == null ? "?" /*+" : #"+face.Id.ToString ()*/ : face.Name;
						string nameWithIdIfUnknown = face.Name == null ? "?" +" : #"+face.Id.ToString () : face.Name;
						
						Pixbuf facePixbuf = face.iconPixbuf != null ? face.iconPixbuf.ScaleSimple (FaceSpot.THUMBNAIL_SIZE, FaceSpot.THUMBNAIL_SIZE, FaceSpot.IconResizeInterpType) : null;
						//Pixbuf facePixbuf = null;
						//int thmSize = FaceSpot.THUMBNAIL_SIZE;
						//Pixbuf facePixbuf = face.iconPixbuf != null ? ImageTypeConverter.ConvertCVImageToPixbuf(ImageTypeConverter.ConvertPixbufToCVImage(face.iconPixbuf).Resize(thmSize,thmSize)) : null;
						
						if (facePixbuf == null)
							Log.Exception (new Exception ("Allowed null Face Pixbuf to the faceiconview"));
						if(IsBrowserType && IsShowFullImage){
							Pixbuf fullPixbuf = ThumbnailCache.Default.GetThumbnailForUri(face.photo.DefaultVersionUri);
							if(fullPixbuf == null){
								fullPixbuf = ThumbnailGenerator.Create(face.photo.DefaultVersionUri);
								ThumbnailCache.Default.AddThumbnail(face.photo.DefaultVersionUri,fullPixbuf);
							}
							listStore.AppendValues (name, facePixbuf, face,nameWithIdIfUnknown,fullPixbuf);
						}
						else
							listStore.AppendValues (name, facePixbuf, face,nameWithIdIfUnknown);
					} else
						Log.Exception (new Exception ("Allowed null Face input to the faceiconview"));
				}
				
				Log.Debug(">>> SetListStoreFaces Ended");
			} catch (Exception e) {				
				Log.Exception(e);
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
//				TreeIter iter;
//				//listStore
//				listStore.GetIterFromString(out iter,value.Name);
//				if( !TreeIter.Zero.Equals(iter) ){
//					TreePath path = listStore.GetPath(iter);
//					if(path!=null)
//						this.SelectPath(path);
//				}
				List<Face> list = new List<Face>();
				list.Add(value);
				SelectedFaces = list;
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
					if(f.Name != null){
						//this.Model.
						listStore.GetIterFromString(out iter,f.Name);
						if( !TreeIter.Zero.Equals(iter) ){
							TreePath path = listStore.GetPath(iter);
							if(path!=null)
								this.SelectPath(path);
						}
					}
				}
			}
		}

		public Tag Tag {
			get {
				return tag;
			}
			set {
				tag = value;
				Log.Debug("FaceIconView "+type.ToString()+"Tag Assigned");
				UpdateFaces();
			}
		}

		public bool IsShowFullImage {
			get {
				return isShowFullImage;
			}
			set {
				isShowFullImage = value;
				if(isShowFullImage)
					PixbufColumn = 4;
				else 
					PixbufColumn = 1;
				
			}
		}
		
		void HandleButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if ( args.Event.Type == EventType.TwoButtonPress && args.Event.Button == 1){
				HandleDoubleClick(args);
			}
			else if( args.Event.Type == EventType.ButtonPress && args.Event.Button == 3){
				HandleRightClick (args);
			}
		}

		void HandleDoubleClick (ButtonPressEventArgs args)
		{
			Log.Debug("Double Click at Face#"+SelectedFace.Id);
		}
		
		void HandleRightClick (ButtonPressEventArgs args)
		{
			TreePath facePath;
			CellRenderer faceCell;
			Face selectedFace = null;
			TreeIter faceIter;
			this.GetItemAtPos ((int)args.Event.X, (int)args.Event.Y, out facePath, out faceCell);
			if (facePath == null)
				return;
			listStore.GetIter (out faceIter, facePath);
			try {
				selectedFace = (Face)listStore.GetValue (faceIter, 2);
			} finally {
			}
			Log.Debug ("Button Pressed on Face :" + selectedFace.Id);
			bool isInSelection = false;
			List<Face> selectedFaces = SelectedFaces;
			foreach (Face f in selectedFaces) {
				if (f.Equals (selectedFace)) {
					isInSelection = true;
				}
			}
			if (!isInSelection) {
				this.UnselectAll ();
				this.SelectPath (facePath);
				selectedFaces.Clear ();
				selectedFaces.Add (selectedFace);
			}
			String fids = "";
			foreach (Face f in selectedFaces) {
				fids += f.Id + " ";
			}
			Log.Debug ("With+" + selectedFaces.Count + " Selection :" + fids);
			FaceIconViewPopupMenu popup = new FaceIconViewPopupMenu ();
			popup.Activate (args.Event, this);
			args.RetVal = true;
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
