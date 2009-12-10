
using FSpot.UI.Dialog;
using Glade;
using Gtk;
using FaceSpot.Db;

namespace FaceSpot
{
	public class FaceEditorDialog : GladeDialog
	{
		[Widget] HBox mainHBox;
		[Widget] Image faceImage;
		[Widget] Button cancelButton;
		[Widget] Button okButton;
		[Widget] ComboBox personComboBox;
		
		Face face;
		
		public FaceEditorDialog (Face face) : base("FaceEditorDialog","FaceSpot.ui.FaceBrowser.glade")
		{
			
		}
	}
}
