
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
		Face[] SelectedFaces{
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
	}
}
