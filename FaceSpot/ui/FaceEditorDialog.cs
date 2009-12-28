
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
		Label PersonErrorLabel;
		const string PersonErrorLabelMarkup = "Entered Person Not Found\r\n"+
					"Pressing OK Will Create New Person";
		[Widget]
		ComboBoxEntry peopleComboBoxEntry;
		EntryCompletion entryCompletion;
		//EntryCompletionMatchFunc entryCompletionMatchFunc;
		
		Face face;
		
		#region Category
		bool transactionCleared = false;
		 TreeStore peopleTreeStore;
		void PopulateCategoryComboBoxEntry ()
		{
			//categories
			Log.Debug("PeopleArrayListCreated");
			
			peopleTreeStore = new TreeStore(typeof(String),typeof(Tag));
			//TreeIter iter = peopleTreeStore.AppendValues(People.Tag.Name,People.Tag);
			PopulatePeopleCategories(peopleTreeStore,People.Tag,TreeIter.Zero,0);
			peopleComboBoxEntry.Model = peopleTreeStore;
			
			peopleComboBoxEntry.Changed += PeopleComboBoxEntryChanged;
			
			
		}
		void InitializeEntryCompletion ()
		{
			entryCompletion = new EntryCompletion();
			
			peopleComboBoxEntry.Entry.Completion = entryCompletion;
			entryCompletion.Model = peopleTreeStore;
			entryCompletion.TextColumn = 0;
			//entryCompletion.PopupCompletion = true;
			entryCompletion.InlineCompletion = true;
			
		}
		Tag SelectedTag{
			get {  return MainWindow.Toplevel.Database.Tags.GetTagByName (
					peopleComboBoxEntry.ActiveText.Trim());  }	
		}
		void PeopleComboBoxEntryChanged (object sender, EventArgs e)
		{
			if( SelectedTag == null && 
			   peopleComboBoxEntry.ActiveText.Trim().Length !=0)
			{
				PersonErrorLabel.Markup = PersonErrorLabelMarkup;
			}else {
				PersonErrorLabel.Text ="";
			}	
			//entryCompletion.Complete();
		}
		
		

		void PopulatePeopleCategories (TreeStore treeStore ,Tag parent,TreeIter parentIter,int level)
		{
			foreach (Tag tag in (parent as Category).Children) {
				if (tag is Category) {
					Log.Debug("Append  : "+tag.Name + " to "+parent.Name);
					TreeIter iter = 
						(parentIter.Equals(TreeIter.Zero) ?
						treeStore.AppendValues(tag.Name,/*parent,*/tag):
							treeStore.AppendValues(parentIter,tag.Name,/*parent,*/tag)) ;
					PopulatePeopleCategories (treeStore,tag,iter,level+1);
				}
			}
		}
		#endregion

		protected new string dialog_name = "FaceEditorDialog";

		bool newFace;
		
		public FaceEditorDialog (Face face, Widget parent, bool newFace) 
			: base("FaceEditorDialog", "FaceSpot.ui.FaceSpot.glade")
		{
			this.face =face;
			this.newFace = newFace;
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
			InitializeEntryCompletion ();
			PersonErrorLabel.Text = "";
			
			if(face.tag != null){
				Log.Debug("Tag "+face.tag.Name+" for this face yet");
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
			FaceSidebarWidget.Instance.UpdateFaceIconView();
			ClearEditor ();
		}
		private void HandleOkOldFace ()
		{
			face.tag = SelectedTag;
			face.tagConfirmed = SelectedTag != null;
			FaceSpotDb.Instance.Faces.Commit(face);
		}
		
		private void HandleOkNewFace ()
		{
			if (SelectedTag != null) {
				Log.Debug ("FaceEditor OK : Found Tag" + peopleComboBoxEntry.ActiveText);
				//FaceSpotDb.Instance.Faces.AddTag (face, selectedTag, true);
				face.tag = SelectedTag;
				face.tagConfirmed = SelectedTag != null;
				FaceSpotDb.Instance.Faces.Commit(face);
			} else {
				if (peopleComboBoxEntry.ActiveText.Trim ().Length > 0) {
					//FIX ME - fix bug around here
					Log.Debug ("FaceEditor OK : New Tag" + peopleComboBoxEntry.ActiveText);
					TagCommands.Create createCom = new TagCommands.Create (
						MainWindow.Toplevel.Database.Tags, MainWindow.Toplevel.GetToplevel (this));
					//SelectedTag = 
						createCom.Execute (TagCommands.TagType.Tag, null);
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
