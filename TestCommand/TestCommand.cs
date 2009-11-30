using System;
using System.IO;
using Gtk;

using Mono.Unix;

using FSpot;
using FSpot.Extensions;
using FSpot.Utils;
using FSpot.Jobs;

namespace Test
{
	public class TestCommand : ICommand
	{
	
		public void Run (object o, EventArgs e)
		{
			TestDialog dialog = new TestDialog ();  
			dialog.ShowDialog ();
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