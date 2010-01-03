using System;
using Gtk;
using Gdk;
using Mono.Unix;
using FSpot;
using FSpot.Widgets;
using FSpot.Utils;
using FaceSpot;
using FaceSpot.Db;
using Banshee.Kernel;

namespace FaceSpot
{
//TODO Add Checkbox to Show Rectangle	
public class FaceSidebarWidget : ScrolledWindow {
		static FaceSidebarWidget instance;
		FaceEditMode mode = FaceEditMode.Add;
		
		public bool selected;
		
		public enum FaceEditMode {
			Add,
			Edit
		}
	//	Delay updateDelay;
		
		VBox mainVBox;//,faceVBox;
		VPaned faceVPane;
		Button addFaceButton;
		Button detectFaceButton;
	
		Label pleaseSelectPictureLabel;
		
		Expander knownFaceExpander, unknownFaceExpander;
		//HandleBox faceHandleBox;
		ScrolledWindow knownFaceScrolledWindow, unknownFaceScrolledWindow;
		public FaceIconView knownFaceIconView,unknownFaceIconView;
	
		//PhotoList knownFaceList,unknownFaceList;
			
		public SidebarPage Page;
		const string SelectImageMarkup = "<span weight=\"bold\">" +"Please Select an Image" + "</span>";
		string manualAddFaceString = Catalog.GetString("Manually Add New Face");
		string moveFaceString = Catalog.GetString("Move Selected Face");
		public FaceSidebarWidget ()
		{
			instance = this;
			
			mainVBox = new VBox();
			//mainVBox.Spacing = 6;
			//faceVBox = new VBox();
			faceVPane = new VPaned();
			
			pleaseSelectPictureLabel = new Label ();
			pleaseSelectPictureLabel.Markup = SelectImageMarkup;
			//headerLabel =  new Label (Catalog.GetString ("Not Implemented Yet"));
			mainVBox.PackStart(pleaseSelectPictureLabel,false,false,0);
			
			knownFaceExpander = new Expander("In this photo:");
			
			//faceVBox.PackStart(knownFaceExpander,true,true,0);
			//faceVPane.Add(knownFaceExpander);
			knownFaceScrolledWindow = new ScrolledWindow();
			knownFaceExpander.Add(knownFaceScrolledWindow);
			faceVPane.Pack1(knownFaceExpander,true,true);
			//knownFaceExpander.HeightRequest = 30;
//			faceHandleBox = new HandleBox();
//			faceHandleBox.HandlePosition = PositionType.Top;
//			faceVBox.PackStart(faceHandleBox,false,false,0);
			
			unknownFaceExpander = new Expander("Who's also in this photo");
			//faceVBox.PackStart(unknownFaceExpander,true,true,0);
			//faceVPane.Add(unknownFaceExpander);
			
			unknownFaceScrolledWindow = new ScrolledWindow();
			unknownFaceExpander.Add(unknownFaceScrolledWindow);
			faceVPane.Pack2(unknownFaceExpander,true,true);
			//unknownFaceExpander.HeightRequest = 30;
			mainVBox.PackStart(faceVPane,true,true,0);
			
			detectFaceButton = new Button(Catalog.GetString("Re-Detect Face From This Picture"));
			mainVBox.PackEnd(detectFaceButton,false,false,0);
			detectFaceButton.Clicked += DetectFaceButtonClicked;
			
			addFaceButton = new Button(manualAddFaceString);
			mainVBox.PackEnd(addFaceButton,false,false,0);
			addFaceButton.Clicked += AddFaceButtonClicked;
			
			knownFaceScrolledWindow.Visible = false;
			unknownFaceScrolledWindow.Visible = false;
			
			knownFaceExpander.ResizeMode = ResizeMode.Parent;
			unknownFaceExpander.ResizeMode = ResizeMode.Parent;
			Log.Debug("HeightR");
			
			
			ShadowType = ShadowType.None;
			BorderWidth = 0;
			//AddWithViewport(pleaseSelectPictureLabel);
			AddWithViewport (mainVBox);
			//mainVBox.Visible = false;
			ShowAll();
		}

		void DetectFaceButtonClicked (object sender, EventArgs e)
		{
			if(SelectedItem != null && SelectedItem is Photo)
				DetectionJob.Create((Photo)SelectedItem,JobPriority.AboveNormal);
		}

		void AddFaceButtonClicked (object sender, EventArgs e)
		{
			Log.Debug ("Add/Edit Face Button Clicked");
			if(mode == FaceEditMode.Add){
				AddFace ();
			}
			else if (mode == FaceEditMode.Edit)
			{
				EditFace();
			}
		}
		
//		public void UpdateFaceIconView(){
//			if(knownFaceIconView != null)
//				knownFaceIconView.UpdateFaces();
//			if(unknownFaceIconView !=null)
//				unknownFaceIconView.UpdateFaces();
//		}
		
		public Face SelectedFace{
			get { 
				Face face = knownFaceIconView.SelectedFace;
				if(face==null) face = unknownFaceIconView.SelectedFace;
				return face;  
			}		
		}
			
		private void EditFace () {
			PhotoImageView view = MainWindow.Toplevel.PhotoView.View;
			if (Rectangle.Zero == view.Selection) {
				//TODO Add some alert
				AlertNoMove();
			}else {
				view.SelectionXyRatio = 1;
				Face face =SelectedFace;
				if(face!=null){
					face.iconPixbuf = FaceSpotDb.Instance.Faces.GetFacePixbufFromView();
					FaceSpotDb.Instance.Faces.Commit(face);
					AlertMove();
				}
				else 
					Log.Exception(new Exception("Bug at EditFace"));
			}
			Mode = FaceEditMode.Add;
		}

		private void AddFace ()
		{
			PhotoImageView view = MainWindow.Toplevel.PhotoView.View;
			if (Rectangle.Zero == view.Selection) {
				AlertNoSelection ();
				return;
			} else {
//				if (view.Selection.Height != view.Selection.Width) {
//					view.SelectionXyRatio = 1;
//					//view.SelectionXyRatio = 0;
//				}
				view.SelectionXyRatio = 1;
				Log.Debug ("Create Face");
				FaceSpotDb.Instance.BeginTransaction ();
				Face face = FaceSpotDb.Instance.Faces.CreateFaceFromView ((FSpot.Photo)SelectedItem, (uint)view.Selection.Left, (uint)view.Selection.Top, (uint)view.Selection.Width);
				Log.Debug ("New Dialog");
				try {
					//FaceEditorDialog dialog =
						new FaceEditorDialog (face, this, true);
					Log.Debug ("Before Show All");
				} catch (Exception ex) {
					Log.Exception (ex);
				}
			}
		}


		private static void AlertNoSelection ()
		{
			string msg = Catalog.GetString ("No selection available");
			string desc = Catalog.GetString ("This tool requires an active selection. Please select a region of the photo and try the operation again");
			FSpot.UI.Dialog.HigMessageDialog md = new FSpot.UI.Dialog.HigMessageDialog (MainWindow.Toplevel.Window, DialogFlags.DestroyWithParent, Gtk.MessageType.Error, ButtonsType.Ok, msg, desc);
			md.Run ();
			md.Destroy ();
		}
		private static void AlertMove()
		{
			string msg = Catalog.GetString ("Face Moved");
			string desc = Catalog.GetString ("The selected Face's position has been moved.");
			FSpot.UI.Dialog.HigMessageDialog md = new FSpot.UI.Dialog.HigMessageDialog (MainWindow.Toplevel.Window, DialogFlags.DestroyWithParent, Gtk.MessageType.Error, ButtonsType.Ok, msg, desc);
			md.Run ();
			md.Destroy ();
		}
		private static void AlertNoMove()
		{
			string msg = Catalog.GetString ("Face Not Moved");
			string desc = Catalog.GetString ("Because you select nothing");
			FSpot.UI.Dialog.HigMessageDialog md = new FSpot.UI.Dialog.HigMessageDialog (MainWindow.Toplevel.Window, DialogFlags.DestroyWithParent, Gtk.MessageType.Error, ButtonsType.Ok, msg, desc);
			md.Run ();
			md.Destroy ();
		}
		//TODO Ham : revise this code part
		#region to revise
		
		IBrowsableItem item;
		public IBrowsableItem SelectedItem {
			get {
				return item;
			}
			set {
				item = value;
				ShowAll();
				if (item != null) {
					ShowPhotoFaces();
				} else {
					ClearPhotoFaces();
				}
			}
		}
		
		public void HandleSelectionChanged (IBrowsableCollection collection) {
			Log.Debug("Face Sidebar Handle Selection Change");
			if (collection != null && collection.Count == 1)
				SelectedItem = collection [0];
			else
				SelectedItem = null;
		}
		
		#endregion

		public static FaceSidebarWidget Instance {
			get {
//				if(instance == null)
//				{
//					instance = new FaceSidebarWidget();
//				}
//				Log.Debug("Created FaceSidebarWidget Instance"+ (instance ==null) );
				return instance;
			}
		}

		public FaceEditMode Mode {
					get {
						return mode;
					}
					set {
						mode = value;
						switch(mode){
							case FaceEditMode.Add:
								addFaceButton.Label = manualAddFaceString;
								break;
							case FaceEditMode.Edit:
								addFaceButton.Label = moveFaceString;
								break;
						}
					}
				}		
		bool vboxRemoved = true;
		private void ShowPhotoFaces()
		{
			try 
			{
				ClearPhotoFaces();
				if( SelectedItem == null)return;
				//FSpot.Photo photo = (FSpot.Photo) SelectedItem;
				if(vboxRemoved){
					//AddWithViewport (mainVBox); 
					//mainVBox.Visible = true;
					pleaseSelectPictureLabel.Markup = "Image Selected";
					knownFaceScrolledWindow.Visible = true;
					unknownFaceScrolledWindow.Visible = true;
					vboxRemoved = false;
				}
				
				//IBrowsableItem[] knownFaceItems = FaceSpotDb.Instance.Faces.GetKnownFaceByPhoto(photo);
				
				//knownFaceList = new PhotoList(knownFaceItems);
				knownFaceIconView = new FaceIconView(FaceIconView.Type.KnownFaceSidebar);
				knownFaceScrolledWindow.AddWithViewport(knownFaceIconView);
				knownFaceExpander.Expanded = true;
				knownFaceIconView.SelectionChanged += KnownFaceIconViewSelectionChanged;
				
				//IBrowsableItem[] unknownFaceItems = FaceSpotDb.Instance.Faces.GetNotKnownFaceByPhoto(photo);
				
				//unknownFaceList = new PhotoList(unknownFaceItems);
				unknownFaceIconView = new FaceIconView(FaceIconView.Type.UnknownFaceSidebar);
				unknownFaceScrolledWindow.AddWithViewport(unknownFaceIconView);
				unknownFaceExpander.Expanded = true;
				unknownFaceIconView.SelectionChanged += UnknownFaceIconViewSelectionChanged;
				ShowAll();
				
//				FaceOverlay.Instance.ShowOverlayFaces();
			}
			catch (Exception e){
				FSpot.Utils.Log.Exception (e);
			}
		}

		void UnknownFaceIconViewSelectionChanged (object sender, EventArgs e)
		{
			Log.Debug("Unknown Face Icon Selection Changed");
//			FaceOverlay.Instance.ShowOverlayFaces();
			if(unknownFaceIconView.SelectedFaces.Count > 0){
				knownFaceIconView.UnselectAll();
				//MainWindow.Toplevel.PhotoView.View.
			}
			if(Mode == FaceEditMode.Edit){
				Mode = FaceEditMode.Add;
				MainWindow.Toplevel.PhotoView.View.Selection = Rectangle.Zero;
			}
		}

		void KnownFaceIconViewSelectionChanged (object sender, EventArgs e)
		{
			Log.Debug("known Face Icon Selection Changed");
//			FaceOverlay.Instance.ShowOverlayFaces();
			if(knownFaceIconView.SelectedFaces.Count > 0){
				unknownFaceIconView.UnselectAll();
				//MainWindow.Toplevel.PhotoView.View.Realize();
			}
			if(Mode == FaceEditMode.Edit){
				Mode = FaceEditMode.Add;	
				MainWindow.Toplevel.PhotoView.View.Selection = Rectangle.Zero;
			}
		}
		
		/// <summary>
		/// Remove Face Detection Frame Hilight on the .....
		/// </summary>
		private void ClearPhotoFaces()
		{
			if(!vboxRemoved ){
				Log.Debug("Remove VBox");
				knownFaceScrolledWindow.Visible = false;
				unknownFaceScrolledWindow.Visible = false;
				pleaseSelectPictureLabel.Markup = SelectImageMarkup;
				vboxRemoved = true;
			}
			foreach (Widget w in knownFaceScrolledWindow){
				//Log.Debug("Remove Widget "+ w.ToString());
				knownFaceScrolledWindow.Remove(w);
			}
			foreach (Widget w in unknownFaceScrolledWindow){
				//Log.Debug("Remove Widget "+ w.ToString());
				unknownFaceScrolledWindow.Remove(w);
			}
			ShowAll();
			
			//FaceOverlay.Instance.ClearOverlayFaces();
		}
	}
}
