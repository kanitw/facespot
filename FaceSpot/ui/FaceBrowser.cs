using System;
using System.IO;
using Gtk;

using Mono.Unix;
using Glade;
using FSpot;
using FSpot.Extensions;
using FSpot.Utils;
using FSpot.Jobs;

namespace FaceSpot
{
	public class FaceBrowser : ICommand
	{
		protected string dialog_name = "browserWindow";
		protected Glade.XML xml;
		private void initGladeXML()
		{
			xml = new Glade.XML(null,"FaceSpot.ui.FaceBrowser.glade",dialog_name,"f-spot");
			xml.Autoconnect(this);
		}
		
		private Gtk.Window browserWindow;
		
		[Glade.Widget] HPaned MainHPaned;
		[Glade.Widget] VBox MainVBox;
		[Glade.Widget] Statusbar StatusBar;
		[Glade.Widget] MenuItem	 menuitem_preference;
//		[Glade.Widget]
//		[Glade.Widget]
		
		public void Run (object o, EventArgs e)
		{
			//TODO change Console to appropriate FSpot Debugger
			Console.WriteLine ("Executing FaceSpotBrowser");
			Log.Trace ("FaceBrowser", "Executing FaceSpotBrowser");
			initGladeXML();
			
			menuitem_preference.Activated += Menuitem_preferenceActivated;
			browserWindow = (Gtk.Window) xml.GetWidget(dialog_name);
			//TestDialog dialog = new TestDialog ();  
			//dialog.ShowDialog ();
			browserWindow.ShowAll();
		}

		void Menuitem_preferenceActivated (object sender, EventArgs e)
		{
			
		}
	}
	
	public class TestDialog : Dialog
	{
		private Gtk.Label status_label;
		public void ShowDialog()
		{
			// This query is not very fast, but it's a 'one-time' so don't care much...
			VBox.Spacing = 6;
			
			Label l = new Label (Catalog.GetString ("In order to detect duplicates on pictures you imported before 0.5.0, " +
					"F-Spot needs to analyze your image collection. This is not done by default as it's time consuming. " +
					"You can Start or Pause this update process using this dialog."));
			l.LineWrap = true;
			VBox.PackStart (l);

			Label l2 = new Label (Catalog.GetString ("Test"));
			l2.LineWrap = true;
			VBox.PackStart (l2);

			Button execute = new Button (Stock.Execute);
			//execute.Clicked += HandleExecuteClicked;
			VBox.PackStart (execute);

			Button stop = new Button (Stock.Stop);
			//stop.Clicked += HandleStopClicked;
			VBox.PackStart (stop);

			status_label = new Label ();
			VBox.PackStart (status_label);

			this.AddButton ("_Close", ResponseType.Close);
			//this.Response += HandleResponse;
			ShowAll ();
		}
	}
}
