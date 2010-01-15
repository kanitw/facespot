
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
		const string NoPersonErrorLabelMarkup = "Empty Text\r\n"+
					"Pressing OK will match this image with no person";
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
			peopleTreeStore = new PeopleTreeStore();
			//TreeIter iter = peopleTreeStore.AppendValues(People.Tag.Name,People.Tag);
			peopleComboBoxEntry.Model = peopleTreeStore;
			
			peopleComboBoxEntry.Changed += PeopleComboBoxEntryChanged;
		}
		void InitializeEntryCompletion ()
		{
			entryCompletion = new EntryCompletion();
			
			peopleComboBoxEntry.Entry.Completion = entryCompletion;
			entryCompletion.Model = peopleTreeStore;
			entryCompletion.TextColumn = 0;
			entryCompletion.InlineCompletion = true;
			
		}
		Tag SelectedTag{
			get {  return MainWindow.Toplevel.Database.Tags.GetTagByName (
					peopleComboBoxEntry.ActiveText.Trim());  }	
		}
		void PeopleComboBoxEntryChanged (object sender, EventArgs e)
		{
			if( SelectedTag == null )
			{
				if (peopleComboBoxEntry.ActiveText.Trim().Length !=0)
					PersonErrorLabel.Markup = PersonErrorLabelMarkup;
				else
					PersonErrorLabel.Markup = NoPersonErrorLabelMarkup;
			}else {
				PersonErrorLabel.Text ="";
			}	
			//entryCompletion.Complete();
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
			Gdk.Pixbuf pix = null;
			if(face.iconPixbuf != null){
				pix = face.iconPixbuf;
				//TODO Determine Resize Method
				faceImage.Pixbuf = pix.ScaleSimple (100, 100, FaceSpot.IconResizeInterpType);
			}else {
				
			}
			ok_button.Clicked += OkButtonClicked;
			cancel_button.Clicked += CancelButtonClicked;
			this.Dialog.Destroyed += DialoghandleDestroyed;
			PopulateCategoryComboBoxEntry();
			InitializeEntryCompletion ();
			PersonErrorLabel.Text = "";
			
			if(face.Tag != null){
				Log.Debug("Set Entry's text to Tag "+face.Tag.Name);
				peopleComboBoxEntry.Entry.Text = face.Tag.Name;
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
			//FaceSidebarWidget.Instance.UpdateFaceIconView();
			ClearEditor ();
		}
		private void HandleOkOldFace ()
		{
			HandleOk();
		}
		
		private void HandleOk(){
			if (peopleComboBoxEntry.ActiveText.Trim ().Length > 0) {
				if (SelectedTag != null) {
					Log.Debug ("FaceEditor OK : Found Tag" + peopleComboBoxEntry.ActiveText);
					FaceSpotDb.Instance.Faces.SetTag(face, SelectedTag);
				} else {
					//Create new Tag
					Log.Debug ("FaceEditor OK : New Tag" + peopleComboBoxEntry.ActiveText);
					Category cat= MainWindow.Toplevel.Database.Tags.CreateCategory( 
						People.Category,
					    peopleComboBoxEntry.ActiveText.Trim (),
						true);                        
					FaceSpotDb.Instance.Faces.SetTag(face, cat);
				}
				
			} else {
				Log.Debug ("FaceEditor OK : No Tag" + peopleComboBoxEntry.ActiveText);
				if(face.Tag != null){
					FaceSpotDb.Instance.Faces.DeclineTag(face,true);
				}
			}
			
		}
		
		private void HandleOkNewFace ()
		{
			HandleOk();
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
