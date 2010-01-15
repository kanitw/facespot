
using System;
using Gtk;
using FaceSpot.Db;
using FSpot.Utils;
using Mono.Unix;
using FSpot.UI.Dialog;
using FaceSpot;
using FSpot;

namespace FaceSpot
{
	public class FaceIconViewPopupMenu : Menu
	{
		//Face face; Face[] faces;
		public Face[] SelectedFaces{
			get { return iconView.SelectedFaces.ToArray(); }	
		}
		FaceIconView iconView;
		public event EventHandler ActionActivated;
		public FaceIconViewPopupMenu () : base(){
			ActionActivated += HandleActionActivated;
			
		}

		void HandleActionActivated (object sender, EventArgs e)
		{
			FaceIconView.UpdateAll();
		}
		void EmitActionActivated(){
			iconView.UnselectAll();
			ActionActivated(this,null);	
		}
		
		private bool IsAllSelectionSuggested(){
			if(SelectedFaces == null || SelectedFaces.Length == 0) return false;
			foreach (Face face in SelectedFaces)
			{
				if(face.TagConfirmed  || face.Tag == null) return false;
			}
			return true;
		}
		
		public void Activate(Gdk.EventButton eb, 
		                     //Face face, Face[] faces,
		                     FaceIconView iconView)
		{
			//this.face = face; this.faces= faces; 
			this.iconView = iconView;
			if(iconView.type == FaceIconView.Type.SuggestedFaceBrowser
			   || IsAllSelectionSuggested()
			   ){
				GtkUtil.MakeMenuItem(this,"Confirm Person",new EventHandler(ConfirmActivated),SelectedFaces.Length>0);
				GtkUtil.MakeMenuItem(this,"Decline Person",new EventHandler(DeclineActivated),SelectedFaces.Length>0);
				GtkUtil.MakeMenuSeparator(this);
			}
			
//			if(SelectedFaces.Length == 1 && iconView.IsBrowserType)
//			{
//				GtkUtil.MakeMenuItem(this,"Show In Main Window",new EventHandler(ShowImageActivated),SelectedFaces.Length>0);
//				GtkUtil.MakeMenuSeparator(this);	
//			}
			   
			if(SelectedFaces.Length == 1 && iconView.IsSideBarType)
				GtkUtil.MakeMenuItem(this,"Move Face",new EventHandler(MoveActivated),true);
			
			if(SelectedFaces.Length == 1)
				GtkUtil.MakeMenuItem(this,"Change Person",new EventHandler(EditActivated),true);
			
			MenuItem ChangePersonTo = GtkUtil.MakeMenuItem(this, "Change Person to",null,true);
			
			if(ChangePersonTo != null){
				PopulatePeopleCategories (ChangePersonTo,People.Tag);	
				if  ( ChangePersonTo.Submenu != null )
					GtkUtil.MakeMenuSeparator((Menu)ChangePersonTo.Submenu);
				else 
					ChangePersonTo.Submenu = new Menu();
				GtkUtil.MakeMenuItem((Menu)ChangePersonTo.Submenu,"-", new EventHandler(ChangePersonToNoOneActivated));
			}
			
			GtkUtil.MakeMenuItem(this,
			                     Catalog.GetPluralString("Delete Face","Delete Faces",SelectedFaces.Length),
			                     new EventHandler(DeleteActivated),true);
			//Add Confirm Popup Menu
			this.Popup(null,null,null,eb.Button,Gtk.Global.CurrentEventTime);
		}
		void ConfirmActivated (object sender, EventArgs e)
		{
			foreach(Face f in SelectedFaces)
				FaceSpotDb.Instance.Faces.ConfirmTag(f);
			//FaceIconView.UpdateAll();
			EmitActionActivated();
		}
		void DeclineActivated (object sender, EventArgs e)
		{
			foreach(Face f in SelectedFaces)
				FaceSpotDb.Instance.Faces.DeclineTag(f,true);
			//FaceIconView.UpdateAll();
			EmitActionActivated();
		}
		void ShowImageActivated (object sender, EventArgs e)
		{
			//MainWindow.Toplevel.ViewMode = MainWindow.ModeType.PhotoView;
			//MainWindow.Toplevel.Query.Terms.
		}
		void MoveActivated (object sender, EventArgs e)
		{
			MainWindow.Toplevel.PhotoView.View.Selection = iconView.SelectedFace.Selection;
				
			//iconView.SelectedFace = face;
			FaceSidebarWidget.Instance.Mode = FaceSidebarWidget.FaceEditMode.Edit;
		}

		void EditActivated (object sender, EventArgs e)
		{
			//iconView.SelectedFace = face;
			//FaceEditorDialog dialog = 
				new FaceEditorDialog ( iconView.SelectedFace,this.Toplevel,false);
			
		}
		void ChangePersonToNoOneActivated (object sender, EventArgs e)
		{
			if(SelectedFaces == null) return;
			foreach (Face face in SelectedFaces){
				FaceSpotDb.Instance.Faces.DeclineTag(face,true);
			}
			EmitActionActivated();
		}

		void DeleteActivated (object sender, EventArgs e)
		{
			string header = Catalog.GetPluralString ("Delete the selected face permanently?", 
									    "Delete the {0} selected faces permanently?", 
									    SelectedFaces.Length);
			header = String.Format (header, SelectedFaces.Length);
			string msg = Catalog.GetString("This cannot be undone");
			string ok_caption = Catalog.GetPluralString ("_Delete photo", "_Delete photos", SelectedFaces.Length);
			
			if (ResponseType.Ok == HigMessageDialog.RunHigConfirmation(MainWindow.Toplevel.Window, 
										   DialogFlags.DestroyWithParent, 
										   MessageType.Warning, 
										   header, msg, ok_caption)){
				uint timer = Log.DebugTimerStart ();
				
				FaceSpotDb.Instance.Faces.Remove(SelectedFaces);
				//TODO Decide whether UpdateQuery is the appropriate command - I Think it's not - next time u can remove this 2 lines
				//MainWindow.Toplevel.UpdateQuery ();
				Log.DebugTimerPrint (timer, "HandleDeleteCommand took {0}");
			}
			EmitActionActivated();
		}
		
		void PopulatePeopleCategories (MenuItem menu ,Tag parent)
		{
			if( (parent as Category).Children.Count > 0 )
			{
				menu.Submenu = new Menu();
				if (parent != People.Tag){
					/*ImageMenuItem*/Gtk.MenuItem item = MakeTagMenuItem((Menu)menu.Submenu,parent,true);
					GtkUtil.MakeMenuSeparator((Menu)menu.Submenu);
				}
			}
			foreach (Tag tag in (parent as Category).Children) {
				if (tag is Category) {
					Log.Debug("Append  : "+tag.Name + " to "+parent.Name);
					/*ImageMenuItem*/Gtk.MenuItem item = MakeTagMenuItem((Menu)menu.Submenu,tag,false);
//					GtkUtil.MakeMenuItem((Menu)menu.Submenu,tag.Name,null//new EventHandler(ApplyPerson)
//						                     ,true);
					PopulatePeopleCategories (item,tag);
				}
			}
		}
		
		/*ImageMenuItem*/Gtk.MenuItem MakeTagMenuItem(Menu menu,Tag tag,bool force_enabled){
			ImageMenuItem img_item = new ImageMenuItem(tag !=null ?tag.Name : "-" ) ;
			if(force_enabled ||( tag as Category).Children.Count == 0){
				img_item.Activated += delegate {
					foreach (Face face in this.SelectedFaces){
						if(tag != null)
							FaceSpotDb.Instance.Faces.SetTag(face,tag);
						else 
							FaceSpotDb.Instance.Faces.DeclineTag(face,true);
					}	
					EmitActionActivated();
				};
			}
			if(tag !=null && tag.Icon != null){
				img_item.Image = new Image(tag.Icon);
			}
			img_item.Sensitive = true;
			//this..
			img_item.TooltipText = tag != null ? tag.Name : "-";
			menu.Append (img_item);
			img_item.ShowAll ();
			return img_item;
		}

	}
	
	
//	class PersonMenuItem //: MenuItem//ImageMenuItem
//	{
//		ImageMenuItem imgItem;
//		
//		public Tag tag;
//		FaceIconViewPopupMenu menu;
//
//		public ImageMenuItem ImgItem {
//					get {
//						return imgItem;
//					}
//				}		public PersonMenuItem(FaceIconViewPopupMenu menu,Tag tag,bool force_enabled) 
//			: base()
//		{
////			if(tag !=null && tag.Icon != null){
////				Image = new Image(tag.Icon);
////			}	
//			if(force_enabled ||( tag as Category).Children.Count == 0)
//				this.Activated += new EventHandler(ApplyPerson);
//			Sensitive = true;
//			this.tag = tag;
//			this.menu = menu;
//			this.HeightRequest = 30;
//			//this..
//			this.Name = tag != null ? tag.Name : "-";
//			this.TooltipText = tag != null ? tag.Name : "-";
//		//	Show(
//		}
//		void ApplyPerson (object sender, EventArgs e)
//		{
//			if( sender is PersonMenuItem ){
//				PersonMenuItem item = (PersonMenuItem) sender;
//				Log.Debug("ApplyPerson"+ (item.tag != null && item.tag.Name != null ? item.tag.Name : "-"));
//				foreach (Face face in menu.SelectedFaces){
//					if(item.tag != null)
//						FaceSpotDb.Instance.Faces.SetTag(face,item.tag);
//					else 
//						FaceSpotDb.Instance.Faces.DeclineTag(face);
//				}
//			}
//		}
//		
//	}
}
