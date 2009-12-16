
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
//		[Widget]
//		HBox mainHBox;
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
			//FIXME Fix entryCompletion Bug!!
			
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

		bool newFace;
		
		public FaceEditorDialog (Face face, Widget parent, bool newFace) 
			: base("FaceEditorDialog", "FaceSpot.ui.FaceBrowser.glade")
		{
			this.face =face;
			this.newFace = newFace;
			entryCompletion = new EntryCompletion();
			
			//entryCompletion.MatchFunc = entryCompletionMatchFunc 
			Dialog.Parent = parent;
			Dialog.Modal  = true;
			//Dialog.TransientFor = parent;
				
			Gdk.Pixbuf pix = face.iconPixbuf;
			//TODO Determine Resize Method
			faceImage.Pixbuf = pix.ScaleSimple (100, 100, FaceSpot.IconResizeInterpType);
			ok_button.Clicked += OkButtonClicked;
			cancel_button.Clicked += CancelButtonClicked;
			this.Dialog.Destroyed += DialoghandleDestroyed;
			PopulateCategoryComboBoxEntry();
			
			if(face.tag != null){
				Log.Debug("Tag"+face.tag.Name+" for this face yet");
				peopleComboBoxEntry.Entry.Text = face.tag.Name;
			}else 
				Log.Debug("No Tag for this face yet"+face.Id);
			
			Dialog.ShowAll ();
		}

		void DialoghandleDestroyed (object sender, EventArgs e)
		{
			if(!transactionCleared)
				FaceSpotDb.Instance.RollbackTransaction ();	
		}

		void CancelButtonClicked (object sender, EventArgs e)
		{
			if(newFace)
				FaceSpotDb.Instance.RollbackTransaction ();
			ClearEditor();
		}

		void OkButtonClicked (object sender, EventArgs e)
		{
			if(newFace)
				HandleOkNewFace ();
			else
				HandleOkOldFace ();
			ClearEditor ();
		}
		private void HandleOkOldFace ()
		{
			Tag selectedTag = MainWindow.Toplevel.Database.Tags.GetTagByName (peopleComboBoxEntry.ActiveText.Trim());
			face.tag = selectedTag;
			FaceSpotDb.Instance.Faces.Commit(face);
		}
		
		private void HandleOkNewFace ()
		{
			Tag selectedTag = MainWindow.Toplevel.Database.Tags.GetTagByName (peopleComboBoxEntry.ActiveText.Trim ());
			if (selectedTag != null) {
				Log.Debug ("FaceEditor OK : Found Tag" + peopleComboBoxEntry.ActiveText);
				//FaceSpotDb.Instance.Faces.AddTag (face, selectedTag, true);
				face.tag = selectedTag;
				FaceSpotDb.Instance.Faces.Commit(face);
			} else {
				if (peopleComboBoxEntry.ActiveText.Trim ().Length > 0) {
					//FIX ME - fix bug around here
					Log.Debug ("FaceEditor OK : New Tag Tag" + peopleComboBoxEntry.ActiveText);
					TagCommands.Create createCom = new TagCommands.Create (
						MainWindow.Toplevel.Database.Tags, MainWindow.Toplevel.GetToplevel (this));
					selectedTag = createCom.Execute (TagCommands.TagType.Tag, null);
				} else {
					Log.Debug ("FaceEditor OK : No Tag" + peopleComboBoxEntry.ActiveText);
				}
			}
			FaceSpotDb.Instance.CommitTransaction ();
		}


		private void ClearEditor ()
		{
			MainWindow.Toplevel.PhotoView.View.Selection = Rectangle.Zero;
			transactionCleared = true;
			//MainWindow.Toplevel.UpdateQuery();
			this.Dialog.Destroy ();
			this.Dialog.Dispose ();
		}

		
	}
}
