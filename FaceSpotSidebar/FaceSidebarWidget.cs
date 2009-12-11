using System;
using Gtk;
using Gdk;
using Mono.Unix;
using FSpot;
using FSpot.Widgets;
using FSpot.Utils;
using FaceSpot;
using FaceSpot.Db;

namespace FaceSpot
{

public class FaceSidebarWidget : ScrolledWindow {
		Delay updateDelay;
		
		VBox mainVBox;//,faceVBox;
		VPaned faceVPane;
		Button addFaceButton;
		Button detectFaceButton;
		Label headerLabel;
		Label pleaseSelectPictureLabel;
		
		Expander knownFaceExpander, unknownFaceExpander;
		HandleBox faceHandleBox;
		ScrolledWindow knownFaceScrolledWindow, unknownFaceScrolledWindow;
		FSpot.Widgets.IconView knownFaceIconView,unknownFaceIconView;
		PhotoList knownFaceList,unknownFaceList;
			
		public FaceSidebarPage Page;
	
		public FaceSidebarWidget ()
		{
			mainVBox = new VBox();
			//mainVBox.Spacing = 6;
			//faceVBox = new VBox();
			faceVPane = new VPaned();
			
			headerLabel =  new Label (Catalog.GetString ("Not Implemented Yet"));
			mainVBox.PackStart(headerLabel,false,false,0);
			
			knownFaceExpander = new Expander("In this photo:");
			
			//faceVBox.PackStart(knownFaceExpander,true,true,0);
			//faceVPane.Add(knownFaceExpander);
			faceVPane.Pack1(knownFaceExpander,true,true);
			knownFaceScrolledWindow = new ScrolledWindow();
			knownFaceExpander.Add(knownFaceScrolledWindow);
			
//			faceHandleBox = new HandleBox();
//			faceHandleBox.HandlePosition = PositionType.Top;
//			faceVBox.PackStart(faceHandleBox,false,false,0);
			
			unknownFaceExpander = new Expander("Who's also in this photo");
			//faceVBox.PackStart(unknownFaceExpander,true,true,0);
			//faceVPane.Add(unknownFaceExpander);
			faceVPane.Pack2(unknownFaceExpander,true,true);
			unknownFaceScrolledWindow = new ScrolledWindow();
			unknownFaceExpander.Add(unknownFaceScrolledWindow);
			
			mainVBox.PackStart(faceVPane,true,true,0);
			
			pleaseSelectPictureLabel = new Label ();
			pleaseSelectPictureLabel.Markup = "<span weight=\"bold\">" +
				Catalog.GetString("Please Select an Image") + "</span>";
			
			detectFaceButton = new Button(Catalog.GetString("Re-Detect Face From This Picture"));
			mainVBox.PackEnd(detectFaceButton,false,false,0);
			
			addFaceButton = new Button(Catalog.GetString("Manually Add New Face"));
			mainVBox.PackEnd(addFaceButton,false,false,0);
			addFaceButton.Clicked += AddFaceButtonClicked;
			
			ShadowType = ShadowType.None;
			BorderWidth = 0;
			Add(pleaseSelectPictureLabel);
			ShowAll();
		}

		void AddFaceButtonClicked (object sender, EventArgs e)
		{
			Log.Debug ("Add Face Button Clicked");
			PhotoImageView view = MainWindow.Toplevel.PhotoView.View;
			if (Rectangle.Zero == view.Selection) 
			{
				string msg = Catalog.GetString ("No selection available");
				string desc = Catalog.GetString ("This tool requires an active selection. Please select a region of the photo and try the operation again");

				FSpot.UI.Dialog.HigMessageDialog md = new FSpot.UI.Dialog.HigMessageDialog (MainWindow.Toplevel.Window,
										DialogFlags.DestroyWithParent,
										Gtk.MessageType.Error, ButtonsType.Ok,
										msg,
										desc);

				md.Run ();
				md.Destroy ();
				return;
			} else {
				//TODO add 1:1 constraint to selection
				if( view.Selection.Height != view.Selection.Width){
					view.SelectionXyRatio = 1;
					view.SelectionXyRatio = 0;
				}
				Log.Debug ("Create Face");
				Face face = FaceSpotDb.Instance.Faces.CreateFace (
					(FSpot.Photo)SelectedItem, 
					(uint)view.Selection.Left,
					(uint)view.Selection.Top,
					(uint)view.Selection.Width);
				Log.Debug ("New Dialog");
				try{
					FaceEditorDialog dialog = new FaceEditorDialog (face);
					Log.Debug ("Before Show All");
					//dialog.sho
					//dialog.Dialog.ShowAll ();
				} catch (Exception ex){
					Log.Exception(ex);	
				}
			}
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
		
		internal void HandleSelectionChanged (IBrowsableCollection collection) {
			if (collection != null && collection.Count == 1)
				SelectedItem = collection [0];
			else
				SelectedItem = null;
		}
		
		#endregion
		
		/// <summary>
		/// Show all faces on the sidebar and also draw faces frame on the screen
		/// </summary>
		private void ShowPhotoFaces()
		{
			try 
			{
				ClearPhotoFaces();
				if( SelectedItem == null)return;
				
				FSpot.Photo photo = (FSpot.Photo) SelectedItem;
				#region Wrong Code just for test UI
				uint [] version_ids = photo.VersionIds;
				IBrowsableItem [] items = new IBrowsableItem [version_ids.Length];
		
				int i = items.Length;
				int selected_version_id = -1;
				foreach (uint id in version_ids) {
					i--;
					FSpot.Utils.Log.Debug (string.Format ("Adding version {0} to items[{1}].", id, i));
					items[i] = (photo.GetVersion (id));
					if (id == photo.DefaultVersionId)
						selected_version_id = i;
				}
				#endregion
				
				Remove(pleaseSelectPictureLabel);
				Add(mainVBox);
				//get knownFaceList
				
				//TODO Add knownFaceList Code Fetch
				IBrowsableItem[] knownFaceItems = items;//new IBrowsableItem[0];
				
				knownFaceList = new PhotoList(knownFaceItems);
				knownFaceIconView = new FSpot.Widgets.IconView(knownFaceList);
				knownFaceScrolledWindow.AddWithViewport(knownFaceIconView);
				knownFaceExpander.Expanded = true;
				
				//get unknownFaceList	
				
				//TODO Add unknownFaceList Code
				IBrowsableItem[] unknownFaceItems = new IBrowsableItem[0];
				
				unknownFaceList = new PhotoList(unknownFaceItems);
				unknownFaceIconView = new FSpot.Widgets.IconView(unknownFaceList);
				unknownFaceScrolledWindow.AddWithViewport(unknownFaceIconView);
				unknownFaceExpander.Expanded = true;
				ShowAll();
			}
			catch (Exception e){
				FSpot.Utils.Log.Exception (e);
			}
		}
		
		/// <summary>
		/// Remove Face Detection Frame Hilight on the .....
		/// </summary>
		private void ClearPhotoFaces()
		{
			Remove(mainVBox);
			Add(pleaseSelectPictureLabel);
			foreach (Widget w in knownFaceScrolledWindow)
				knownFaceScrolledWindow.Remove(w);
			foreach (Widget w in unknownFaceScrolledWindow)
				unknownFaceScrolledWindow.Remove(w);
			ShowAll();
		}
	}
}