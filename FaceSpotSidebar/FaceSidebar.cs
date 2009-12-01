using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Gtk;
using Mono.Unix;
using FSpot;
using FSpot.Utils;
using FSpot.Widgets;

namespace FaceSpot
{
	public class FaceSidebarPage : SidebarPage{
		
		public FaceSidebarPage () : base(new FaceSidebarWidget (),
		                                 Catalog.GetString("Face"),
		                                 "gtk-index") 
		{
			(SidebarWidget as FaceSidebarWidget).Page = this;
		}
		
		protected override void AddedToSidebar()
		{
			FaceSidebarWidget widget = SidebarWidget as FaceSidebarWidget;
			Sidebar.SelectionChanged += widget.HandleSelectionChanged;
		}
	}
	public class FaceSidebarWidget : ScrolledWindow {
		Delay updateDelay;
		
		VBox mainVBox;
		Button addFaceButton;
		Button detectFaceButton;
		Label headerLabel;
		Label pleaseSelectPictureLabel;
		
		Expander knownFaceExpander, unknownFaceExpander;
		ScrolledWindow knownFaceScrolledWindow, unknownFaceScrolledWindow;
		FSpot.Widgets.IconView knownFaceIconView,unknownFaceIconView;
		PhotoList knownFaceList,unknownFaceList;
			
		//TreeView knowFaceTreeView;
		
		
		public FaceSidebarPage Page;
	
		public FaceSidebarWidget ()
		{
			mainVBox = new VBox();
			mainVBox.Spacing = 6;
			
			headerLabel =  new Label (Catalog.GetString ("Not Implemented Yet"));
			mainVBox.PackStart(headerLabel,false,false,0);
			
			knownFaceExpander = new Expander("Faces");
			mainVBox.PackStart(knownFaceExpander,false,false,0);
			knownFaceScrolledWindow = new ScrolledWindow();
			knownFaceExpander.Add(knownFaceScrolledWindow);
			
			unknownFaceExpander = new Expander("Unknown Faces");
			mainVBox.PackStart(unknownFaceExpander,false,false,0);
			unknownFaceScrolledWindow = new ScrolledWindow();
			unknownFaceExpander.Add(unknownFaceScrolledWindow);
			
			
			
			pleaseSelectPictureLabel = new Label ();
			pleaseSelectPictureLabel.Markup = "<span weight=\"bold\">" +
				Catalog.GetString("Please Select an Image") + "</span>";
			
			detectFaceButton = new Button(Catalog.GetString("Re-Detect Face From This Picture"));
			mainVBox.PackEnd(detectFaceButton,false,false,0);
			
			addFaceButton = new Button(Catalog.GetString("Manually Add New Face"));
			mainVBox.PackEnd(addFaceButton,false,false,0);
			
			ShadowType = ShadowType.None;
			BorderWidth = 0;
			Add(pleaseSelectPictureLabel);
			ShowAll();
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
				Remove(pleaseSelectPictureLabel);
				Add(mainVBox);
				//get knownFaceList
				
				//TODO Add knownFaceList Code Fetch
				IBrowsableItem[] knownFaceItems = new IBrowsableItem[0];
				
				knownFaceList = new PhotoList(knownFaceItems);
				knownFaceIconView = new FSpot.Widgets.IconView(knownFaceList);
				knownFaceScrolledWindow.Add(knownFaceIconView);
				
				//get unknownFaceList	
				
				//TODO Add unknownFaceList Code
				IBrowsableItem[] unknownFaceItems = new IBrowsableItem[0];
				
				unknownFaceList = new PhotoList(unknownFaceItems);
				unknownFaceIconView = new FSpot.Widgets.IconView(unknownFaceList);
				unknownFaceScrolledWindow.Add(unknownFaceIconView);
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
