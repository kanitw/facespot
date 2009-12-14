
using FSpot.UI.Dialog;
using Glade;
using Gtk;
using FaceSpot.Db;
using Gdk;
using System;
using FSpot;
using FSpot.Database;
using System.Collections;
using FSpot.Utils;

namespace FaceSpot
{
	public class FaceEditorDialog : GladeDialog
	{
		[Widget]
		HBox mainHBox;
		[Widget]
		Gtk.Image faceImage;
		[Widget]
		Button cancel_button;
		[Widget]
		Button ok_button;
		[Widget]
		ComboBoxEntry peopleComboBoxEntry;
		ArrayList categories;
		#region Category
		Category people;
		Tag peopleTag;
		bool transactionCleared = false;
		Category People {
			get { return PeopleTag.Category; }
		}
		Tag PeopleTag {
			get {
				if (peopleTag == null) {
					peopleTag = MainWindow.Toplevel.Database.Tags.Get (3);
					if (!peopleTag.Name.Equals ("People")) {
						peopleTag = MainWindow.Toplevel.Database.Tags.GetTagByName ("People");
					}
				}
				return peopleTag;
			}
		}
		void PopulateCategoryComboBoxEntry ()
		{
			int history = 0 , i =0;
			//categories
			categories = new ArrayList(100);
			Log.Debug("PeopleArrayListCreated");
			categories.Add(People);
			PopulatePeopleCategories(categories,People);
			
			//TreeStore peopleTreeStore = new TreeStore(typeof(Tag));
			//peopleTreeStore.InsertWithValues(0,PeopleTag);
			
			//peopleComboBoxEntry.Model = peopleTreeStore;
			//peopleComboBoxEntry.TextColumn = 0;
			peopleComboBoxEntry.AppendText("Smile");
		}
		void PopulatePeopleCategories (ArrayList categories, Category parent)
		{
//			foreach (Tag tag in parent.Children) {
//				if (tag is Category && tag != this.tag && !this.tag.IsAncestorOf (tag)) {
//					categories.Add (tag);
//					PopulatePeopleCategories (categories, tag as Category);
//				}
//			}
		}
		#endregion

		protected string dialog_name = "FaceEditorDialog";

		public FaceEditorDialog (Face face) : base("FaceEditorDialog", "FaceSpot.ui.FaceBrowser.glade")
		{
			Gdk.Pixbuf pix = face.pixbuf;
			//TODO Determine Resize Method
			faceImage.Pixbuf = pix.ScaleSimple (100, 100, InterpType.Hyper);
			ok_button.Clicked += OkButtonClicked;
			cancel_button.Clicked += CancelButtonClicked;
			this.Dialog.Destroyed += DialoghandleDestroyed;
			PopulateCategoryComboBoxEntry();
			Dialog.ShowAll ();
		}

		void DialoghandleDestroyed (object sender, EventArgs e)
		{
			if(!transactionCleared)
				FaceSpotDb.Instance.Database.RollbackTransaction ();	
		}

		void CancelButtonClicked (object sender, EventArgs e)
		{
			FaceSpotDb.Instance.Database.RollbackTransaction ();
			transactionCleared = true;
			this.Dialog.Destroy ();
			this.Dialog.Dispose ();
		}

		void OkButtonClicked (object sender, EventArgs e)
		{
			//TODO Add UPDATE TAG Code
			
			FaceSpotDb.Instance.Database.CommitTransaction ();
			transactionCleared = true;
			this.Dialog.Destroy ();
			this.Dialog.Dispose ();
		}
	}
}
