
using FSpot.UI.Dialog;
using Glade;
using Gtk;
using FaceSpot.Db;
using Gdk;
using System;

namespace FaceSpot
{
	public class FaceEditorDialog : GladeDialog
	{
		[Widget] HBox mainHBox;
		[Widget] Gtk.Image faceImage;
		[Widget] Button cancelButton;
		[Widget] Button okButton;
		[Widget] ComboBox personComboBox;
		
		protected string dialog_name = "FaceEditorDialog";

		public FaceEditorDialog (Face face) : base("FaceEditorDialog","FaceSpot.ui.FaceBrowser.glade")
		{
			Gdk.Pixbuf pix = face.pixbuf;
			//TODO Determine Resize Method
			faceImage.Pixbuf = pix.ScaleSimple(100,100,InterpType.Hyper); 
			okButton.Clicked += OkButtonClicked;
			cancelButton.Clicked += CancelButtonClicked;
			//TODO Add Code to load People
			Dialog.ShowAll();
		}

		void CancelButtonClicked (object sender, EventArgs e)
		{
			FaceSpotDb.Instance.Database.RollbackTransaction();
			this.Dialog.Dispose();
		}
		
		void OkButtonClicked (object sender, EventArgs e)
		{
			//TODO Add UPDATE TAG Code
			
			FaceSpotDb.Instance.Database.CommitTransaction();
			this.Dialog.Dispose();
		}
	}
}
