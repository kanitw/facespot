
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
		public FaceIconViewPopupMenu () : base(){}
		
		public void Activate(Gdk.EventButton eb, 
		                     //Face face, Face[] faces,
		                     FaceIconView iconView)
		{
			//this.face = face; this.faces= faces; 
			this.iconView = iconView;
			if(iconView.type == FaceIconView.Type.SuggestedFaceBrowser){
				GtkUtil.MakeMenuItem(this,"Confirm Person",new EventHandler(ConfirmActivated),SelectedFaces.Length>0);
				GtkUtil.MakeMenuItem(this,"Decline Person",new EventHandler(DeclineActivated),SelectedFaces.Length>0);
				GtkUtil.MakeMenuSeparator(this);
			}
			
			if(SelectedFaces.Length == 1 && iconView.IsBrowserType)
			{
				GtkUtil.MakeMenuItem(this,"Show In Main Window",new EventHandler(ShowImageActivated),SelectedFaces.Length>0);
				GtkUtil.MakeMenuSeparator(this);	
			}
			   
			if(SelectedFaces.Length == 1 && iconView.IsSideBarType)
				GtkUtil.MakeMenuItem(this,"Move Face",new EventHandler(MoveActivated),true);
			
			if(SelectedFaces.Length == 1)
				GtkUtil.MakeMenuItem(this,"Change Person",new EventHandler(EditActivated),true);
			
			MenuItem ChangePersonTo = GtkUtil.MakeMenuItem(this, "Change Person to",null);
			PopulatePeopleCategories (ChangePersonTo,People.Tag);
			GtkUtil.MakeMenuSeparator((Menu)ChangePersonTo.Submenu);	
			MakeTagMenuItem((Menu)ChangePersonTo.Submenu,null,true);
			
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
		}
		void DeclineActivated (object sender, EventArgs e)
		{
			foreach(Face f in SelectedFaces)
				FaceSpotDb.Instance.Faces.DeclineTag(f);
		}
		void ShowImageActivated (object sender, EventArgs e)
		{
			//Just Hide Photo photo = iconView.SelectedFace.photo;
			//TODO Finish this Function
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
				//TODO Decide whether UpdateQuery is the appropriate command
				MainWindow.Toplevel.UpdateQuery ();
				Log.DebugTimerPrint (timer, "HandleDeleteCommand took {0}");
			}
		}
		
		void PopulatePeopleCategories (MenuItem menu ,Tag parent)
		{
			if( (parent as Category).Children.Count > 0 )
			{
				menu.Submenu = new Menu();
				//if (parent != People.Tag){
					ImageMenuItem item = MakeTagMenuItem((Menu)menu.Submenu,parent,true);
					GtkUtil.MakeMenuSeparator((Menu)menu.Submenu);
				//}
			}
			foreach (Tag tag in (parent as Category).Children) {
				if (tag is Category) {
					Log.Debug("Append  : "+tag.Name + " to "+parent.Name);
					ImageMenuItem item = MakeTagMenuItem((Menu)menu.Submenu,tag,false);
//						GtkUtil.MakeMenuItem((Menu)menu.Submenu,tag.Name,new EventHandler(ApplyPerson)
//						                     ,true);
					PopulatePeopleCategories (item,tag);
				}
			}
		}
		
		ImageMenuItem MakeTagMenuItem(Menu menu,Tag tag,bool force_enabled){
			ImageMenuItem img_item = new PersonMenuItem(this,tag,force_enabled);
			menu.Append (img_item);
			img_item.Show ();
			return img_item;
		}
	}
	
	
	class PersonMenuItem : ImageMenuItem
	{
		public Tag tag;
		FaceIconViewPopupMenu menu;
		public PersonMenuItem(FaceIconViewPopupMenu menu,Tag tag,bool force_enabled) 
			: base(tag != null ? tag.Name : "-" ){
			if(tag !=null && tag.Icon != null){
				Image = new Image(tag.Icon);
			}	
			if(force_enabled ||( tag as Category).Children.Count == 0)
				this.Activated += new EventHandler(ApplyPerson);
			Sensitive = force_enabled;
			this.tag = tag;
			this.menu = menu;
		}
		void ApplyPerson (object sender, EventArgs e)
		{
			if( sender is PersonMenuItem ){
				PersonMenuItem item = (PersonMenuItem) sender;
				Log.Debug("ApplyPerson"+ (item.tag != null && item.tag.Name != null ? item.tag.Name : "-"));
				foreach (Face face in menu.SelectedFaces){
					if(item.tag != null)
						FaceSpotDb.Instance.Faces.SetTag(face,item.tag);
					else 
						FaceSpotDb.Instance.Faces.DeclineTag(face);
				}
			}
		}
		
	}
}
