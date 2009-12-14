
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
		EntryCompletion entryCompletion;
		EntryCompletionMatchFunc entryCompletionMatchFunc;
		
		Face face;
		
		#region Category
		Category people;
		TreeStore peopleTreeStore;
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
			//categories
			Log.Debug("PeopleArrayListCreated");
			
			peopleTreeStore = new TreeStore(typeof(String),typeof(Tag));
			TreeIter it;
			peopleTreeStore.GetIterFirst(out it);
			PopulatePeopleCategories(peopleTreeStore,PeopleTag,it,0);
			
			peopleComboBoxEntry.Model = peopleTreeStore;
			entryCompletion.Model = peopleTreeStore;
			peopleComboBoxEntry.Entry.Completion = entryCompletion;
			//peopleComboBoxEntry.TextColumn = 1;
			
			//peopleComboBox.Model = peopleTreeStore;
			//peopleComboBoxEntry.TextColumn =0;
		}
		int i=0;
		
		private string space(int level){
			return level == 0 ? "" : space(level-1) + "  ";	
		}
		
		void PopulatePeopleCategories (TreeStore treeStore ,Tag parent,TreeIter it,int level)
		{
			foreach (Tag tag in (parent as Category).Children) {
				if(++i==100)break;
				if (tag is Category) {
					Log.Debug("Append "+i+" : "+tag.Name);
					treeStore.AppendValues(space(level)+tag.Name,parent,tag);
					PopulatePeopleCategories (treeStore,tag,it,level+1);
				}
			}
		}
		#endregion

		protected string dialog_name = "FaceEditorDialog";

		public FaceEditorDialog (Face face) : base("FaceEditorDialog", "FaceSpot.ui.FaceBrowser.glade")
		{
			this.face =face;
			entryCompletion = new EntryCompletion();
			
			//entryCompletion.MatchFunc = entryCompletionMatchFunc 
			
			
			
			
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
				FaceSpotDb.Instance.RollbackTransaction ();	
		}

		void CancelButtonClicked (object sender, EventArgs e)
		{
			FaceSpotDb.Instance.RollbackTransaction ();
			transactionCleared = true;
			this.Dialog.Destroy ();
			this.Dialog.Dispose ();
		}

		void OkButtonClicked (object sender, EventArgs e)
		{
			//TODO Add UPDATE TAG 
			Log.Debug("OK : "+ peopleComboBoxEntry.ActiveText);
			Tag selectedTag = MainWindow.Toplevel.Database.Tags.GetTagByName(peopleComboBoxEntry.ActiveText.Trim());
			if(selectedTag != null){
				FaceSpotDb.Instance.Faces.AddTag(face,selectedTag,true);
			}else {
				if(peopleComboBoxEntry.ActiveText.Trim().Length > 0){
						
				}else {
					//Dialog noFaceDialog = new Dialog("No Face Dialog",this,DialogFlags.DestroyWithParent,
					//                                 "cancel","ok");
					//noFaceDialog.ShowAll();
				}
			}
			FaceSpotDb.Instance.CommitTransaction ();
			transactionCleared = true;
			this.Dialog.Destroy ();
			this.Dialog.Dispose ();
		}
	}
}
