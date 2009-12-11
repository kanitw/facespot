
using FSpot.UI.Dialog;
using Glade;
using Gtk;
using FaceSpot.Db;

namespace FaceSpot
{
	public class FaceEditorDialog //: GladeDialog
	{
		[Widget] HBox mainHBox;
		[Widget] Image faceImage;
		[Widget] Button cancelButton;
		[Widget] Button okButton;
		[Widget] ComboBox personComboBox;
		
		private Gtk.Dialog faceEditorDialog ;
		
		Face face;
		protected Glade.XML xml;
		protected string dialog_name = "FaceEditorDialog";
		//TODO if bug fixed - arrange this part of code
		private void initGladeXML()
		{
			xml = new Glade.XML(null,"FaceSpot.ui.FaceBrowser.glade",dialog_name,"f-spot");
			xml.Autoconnect(this);
			faceEditorDialog = (Gtk.Dialog) xml.GetWidget(dialog_name);
		}
		public FaceEditorDialog (Face face) //: base("FaceEditorDialog","FaceSpot.ui.FaceBrowser.glade")
		{
			initGladeXML();
			faceEditorDialog.ShowAll();
		}
	}
}
