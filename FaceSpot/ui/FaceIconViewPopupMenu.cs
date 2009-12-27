
using System;
using Gtk;
using FaceSpot.Db;
using FSpot.Utils;
using Mono.Unix;
using FSpot.UI.Dialog;
using FaceSpot;

namespace FaceSpot
{
	public class FaceIconViewPopupMenu : Menu
	{
		Face face; Face[] faces;
		FaceIconView iconView;
		public FaceIconViewPopupMenu () : base(){}
		
		public void Activate(Gdk.EventButton eb, Face face, Face[] faces,FaceIconView iconView)
		{
			this.face = face; this.faces= faces; this.iconView = iconView;
			if(iconView.type == FaceIconView.Type.SuggestedFaceBrowser){
				GtkUtil.MakeMenuItem(this,"Confirm Person",new EventHandler(ConfirmActivated),faces.Length>0);
				GtkUtil.MakeMenuItem(this,"Decline Person",new EventHandler(DeclineActivated),faces.Length>0);
				GtkUtil.MakeMenuSeparator(this);
			}
			
			GtkUtil.MakeMenuItem(this,"Change Person",new EventHandler(EditActivated),true);
			if(faces.Length == 1 && 
			   (iconView.type == FaceIconView.Type.KnownFaceSidebar
			   || iconView.type == FaceIconView.Type.UnknownFaceSidebar)
			   )
				GtkUtil.MakeMenuItem(this,"Move Face",new EventHandler(MoveActivated),true);
			GtkUtil.MakeMenuItem(this,
			                     Catalog.GetPluralString("Delete Face","Delete Faces",faces.Length),
			                     new EventHandler(DeleteActivated),true);
			//Add Confirm Popup Menu
			this.Popup(null,null,null,eb.Button,Gtk.Global.CurrentEventTime);
		}
		void ConfirmActivated (object sender, EventArgs e)
		{
			foreach(Face f in faces)
				FaceSpotDb.Instance.Faces.ConfirmTag(f);
		}
		void DeclineActivated (object sender, EventArgs e)
		{
			foreach(Face f in faces)
				FaceSpotDb.Instance.Faces.DeclineTag(f);
		}
		
		void MoveActivated (object sender, EventArgs e)
		{
			MainWindow.Toplevel.PhotoView.View.Selection = face.Selection;
				
			iconView.SelectedFace = face;
			FaceSidebarWidget.Instance.Mode = FaceSidebarWidget.FaceEditMode.Edit;
		}

		void EditActivated (object sender, EventArgs e)
		{
			iconView.SelectedFace = face;
			//FaceEditorDialog dialog = 
				new FaceEditorDialog (face,this.Toplevel,false);
		}

		void DeleteActivated (object sender, EventArgs e)
		{
			string header = Catalog.GetPluralString ("Delete the selected face permanently?", 
									    "Delete the {0} selected faces permanently?", 
									    faces.Length);
			header = String.Format (header, faces.Length);
			string msg = Catalog.GetString("This cannot be undone");
			string ok_caption = Catalog.GetPluralString ("_Delete photo", "_Delete photos", faces.Length);
			
			if (ResponseType.Ok == HigMessageDialog.RunHigConfirmation(MainWindow.Toplevel.Window, 
										   DialogFlags.DestroyWithParent, 
										   MessageType.Warning, 
										   header, msg, ok_caption)){
				uint timer = Log.DebugTimerStart ();
				
				FaceSpotDb.Instance.Faces.Remove(faces);
				//TODO Decide whether UpdateQuery is the appropriate command
				MainWindow.Toplevel.UpdateQuery ();
				Log.DebugTimerPrint (timer, "HandleDeleteCommand took {0}");
			}
		}
	}
}
