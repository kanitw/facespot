using System;
using System.IO;
using Gtk;

using Mono.Unix;
using Glade;
using FSpot;
using FSpot.Extensions;
using FSpot.Utils;
using FSpot.Jobs;
using FaceSpot.Db;

namespace FaceSpot
{
	public class FaceBrowser : ICommand
	{
		protected string dialog_name = "browserWindow";
		protected Glade.XML xml;
		private void initGladeXML()
		{
			xml = new Glade.XML(null,"FaceSpot.ui.FaceSpot.glade",dialog_name,"f-spot");
			xml.Autoconnect(this);
		}
		
		const int KnownFacePage = 0;
		const int UnknownFacePage= 1;
		
		private Gtk.Window browserWindow;
		[Widget] Button SuggestionConfirmButton;
		[Widget] Button SuggestionDeclineButton;
		[Widget] Button UnknownFaceButton;
		[Widget] ScrolledWindow KnownFacePhotoScrolledWindow;
		[Widget] ScrolledWindow SuggestedFacePhotoScrolledWindow;
		[Widget] ScrolledWindow UnknownFaceScrolledWindow;
		[Widget] ScrolledWindow KnownFaceScrolledWindow;
		
		[Widget] Notebook MainNotebook;
		
		//Image yesImage,noImage;
		FaceIconView knownFaceIconView, suggestFaceIconView, unknownFaceIconView;
		PeopleTreeView peopleTreeView;
		
		public void Run (object o, EventArgs e)
		{
			Log.Debug ("Executing FaceSpotBrowser");
			initGladeXML();
			browserWindow = (Gtk.Window) xml.GetWidget(dialog_name);
			browserWindow.Title = "F-Spot : FaceSpot FaceBrowser";
			//builder = new GtkBeans.Builder("FaceSpot.ui.FaceSpot.ui");
			//builder.Autoconnect(this)
			//menuitem_preference.Activated += Menuitem_preferenceActivated;
			
//			yesImage = new Image("Yes",IconSize.Button);
//			noImage = new Image("No",IconSize.Button);
//			SuggestionConfirmButton.Image = yesImage;
//			SuggestionDeclineButton.Image = noImage;
//			SuggestionConfirmButton.ImagePosition = PositionType.Left;
			
			SuggestionConfirmButton.Sensitive = false;
			SuggestionConfirmButton.Label = "Yes";
			SuggestionConfirmButton.Clicked += SuggestionConfirmButtonClicked;
			
			SuggestionDeclineButton.Sensitive = false;
			SuggestionDeclineButton.Label = "No";
			SuggestionDeclineButton.Clicked += SuggestionDeclineButtonClicked;
			
			peopleTreeView = new PeopleTreeView();
			KnownFaceScrolledWindow.Add(peopleTreeView);
			peopleTreeView.Selection.Changed += PeopleTreeViewSelectionChanged;
			
			knownFaceIconView = new FaceIconView(FaceIconView.Type.KnownFaceBrowser,null);
			KnownFacePhotoScrolledWindow.Add(knownFaceIconView);
			knownFaceIconView.SelectionChanged += KnownFaceIconViewSelectionChanged;
			
			suggestFaceIconView = new FaceIconView(FaceIconView.Type.SuggestedFaceBrowser,null);
			SuggestedFacePhotoScrolledWindow.Add(suggestFaceIconView);
			suggestFaceIconView.SelectionChanged += SuggestFaceIconViewSelectionChanged;
			
			unknownFaceIconView = new FaceIconView(FaceIconView.Type.UnknownFaceBrowser,null);
			UnknownFaceScrolledWindow.Add(unknownFaceIconView);
			FaceSpotDb.Instance.Faces.ItemsAdded += FaceSpotDbInstanceFacesItemsChanged;
			FaceSpotDb.Instance.Faces.ItemsChanged += FaceSpotDbInstanceFacesItemsChanged;
			FaceSpotDb.Instance.Faces.ItemsRemoved += FaceSpotDbInstanceFacesItemsChanged;
			
			UnknownFaceButton.Clicked += UnknownFaceButtonClicked;
			
			browserWindow.ShowAll();
		}

		void SuggestionDeclineButtonClicked (object sender, EventArgs e)
		{
			Face[] fs = suggestFaceIconView.SelectedFaces.ToArray();
			foreach (Face f in fs)
				FaceSpotDb.Instance.Faces.DeclineTag(f);
		}

		void SuggestionConfirmButtonClicked (object sender, EventArgs e)
		{
			Face[] fs = suggestFaceIconView.SelectedFaces.ToArray();
			foreach (Face f in fs)
				FaceSpotDb.Instance.Faces.ConfirmTag(f);
		}


		void FaceSpotDbInstanceFacesItemsChanged (object sender, DbItemEventArgs<Face> e)
		{
			if(MainNotebook.Page == UnknownFacePage){
				unknownFaceIconView.UpdateFaces();
			}
			else {
				knownFaceIconView.UpdateFaces();
				suggestFaceIconView.UpdateFaces();
			}
		}

		void SuggestFaceIconViewSelectionChanged (object sender, EventArgs e)
		{
			if(suggestFaceIconView.SelectedItems.Length > 0)
			{
				SuggestionConfirmButton.Sensitive = true;
				SuggestionDeclineButton.Sensitive = true;
				knownFaceIconView.UnselectAll();
			}else {
				SuggestionConfirmButton.Sensitive = false;
				SuggestionDeclineButton.Sensitive = false;
			}
		}

		void KnownFaceIconViewSelectionChanged (object sender, EventArgs e)
		{
			if(knownFaceIconView.SelectedItems.Length > 0){
				suggestFaceIconView.UnselectAll();
			}
		}

		void UnknownFaceButtonClicked (object sender, EventArgs e)
		{
			MainNotebook.Page = UnknownFacePage;
			unknownFaceIconView.UpdateFaces();
			peopleTreeView.Selection.UnselectAll();
		}

		void PeopleTreeViewSelectionChanged (object sender, EventArgs e)
		{
			TreeIter iter;
			if(peopleTreeView.Selection.GetSelected(out iter)){
				Tag tag = (Tag) peopleTreeView.Model.GetValue(iter,2);
				if(tag!=null){
					MainNotebook.Page = KnownFacePage;
					knownFaceIconView.Tag = tag;
					suggestFaceIconView.Tag = tag;
				}
			}
		}

		void Menuitem_preferenceActivated (object sender, EventArgs e)
		{
		}
	}
//	
//	public class TestDialog : Dialog
//	{
//		private Gtk.Label status_label;
//		public void ShowDialog()
//		{
//			// This query is not very fast, but it's a 'one-time' so don't care much...
//			VBox.Spacing = 6;
//			
//			Label l = new Label (Catalog.GetString ("In order to detect duplicates on pictures you imported before 0.5.0, " +
//					"F-Spot needs to analyze your image collection. This is not done by default as it's time consuming. " +
//					"You can Start or Pause this update process using this dialog."));
//			l.LineWrap = true;
//			VBox.PackStart (l);
//
//			Label l2 = new Label (Catalog.GetString ("Test"));
//			l2.LineWrap = true;
//			VBox.PackStart (l2);
//
//			Button execute = new Button (Stock.Execute);
//			//execute.Clicked += HandleExecuteClicked;
//			VBox.PackStart (execute);
//
//			Button stop = new Button (Stock.Stop);
//			//stop.Clicked += HandleStopClicked;
//			VBox.PackStart (stop);
//
//			status_label = new Label ();
//			VBox.PackStart (status_label);
//
//			this.AddButton ("_Close", ResponseType.Close);
//			//this.Response += HandleResponse;
//			ShowAll ();
//		}
//	}
}
