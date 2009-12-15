
using System;
using Gtk;
using FaceSpot.Db;
using FSpot.Utils;

namespace FaceSpot
{


	public class FacePopupMenu : Menu
	{

		public FacePopupMenu () : base()
		{
		}
		
		public void Activate(Gdk.EventButton eb, Face face, Face[] faces)
		{
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
			
		}

		void DeleteActivated (object sender, EventArgs e)
		{
			
		}
	}
}
