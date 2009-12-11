using FSpot.UI.Dialog;
using System;
using Glade;
using Gtk;
using FaceSpot.Db;

namespace FaceSpot
{


	public class FacePreference : GladeDialog
	{
		[Widget] Button cleardb_button;
		public FacePreference ()
		{
			cleardb_button.Clicked += Cleardb_buttonClicked;
		}

		void Cleardb_buttonClicked (object sender, EventArgs e)
		{
			FaceSpotDb.Instance.Faces.clearDatabase();
		}
	}
}
