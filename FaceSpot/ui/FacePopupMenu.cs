
using System;
using Gtk;
using FaceSpot.Db;
using FSpot.Utils;
using Mono.Unix;
using FSpot.UI.Dialog;

namespace FaceSpot
{


	public class FacePopupMenu : Menu
	{
		Face face; Face[] faces;
		public FacePopupMenu () : base()
		{
		}
		
		public void Activate(Gdk.EventButton eb, Face face, Face[] faces)
		{
			this.face = face; this.faces= faces;
			GtkUtil.MakeMenuItem(this,"Change Person",new EventHandler(EditActivated),true);
			if(faces.Length == 1)
				GtkUtil.MakeMenuItem(this,"Move",new EventHandler(MoveActivated),true);
			GtkUtil.MakeMenuItem(this,"Delete",new EventHandler(DeleteActivated),true);
			
			this.Popup(null,null,null,eb.Button,Gtk.Global.CurrentEventTime);
		}

		void MoveActivated (object sender, EventArgs e)
		{
			
		}

		void EditActivated (object sender, EventArgs e)
		{
			FaceEditorDialog dialog = new FaceEditorDialog (face,this.Toplevel,false);
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
