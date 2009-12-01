
using System;
using FSpot.Editors;
using Mono.Unix;

namespace FaceSpot
{
	public class FaceEditor : Editor
	{

		public FaceEditor () : base (Catalog.GetString ("Face"), null)
		{
			CanHandleMultiple = false;
			NeedsSelection = true;
			// figure this out!
			HasSettings = true;
			
			ApplyLabel = Catalog.GetString("Apply");
			Initialized += delegate { State.PhotoImageView.PhotoChanged += delegate { UpdateSelectionCombo (); }; };
		}
		
		protected override Gdk.Pixbuf Process (Gdk.Pixbuf input, Cms.Profile input_profile)
		{
			//Implement this
			throw new System.NotImplementedException ();
		}
		
		public override Gtk.Widget ConfigurationWidget ()
		{
			VBox vbox = new VBox();
			//Label faceFound = 
			return vbox;
		}


	}
}
