
using System;
using FSpot.Widgets;
using Gtk;
using FSpot.Utils;
using Mono.Posix;
using FSpot;
namespace FaceSpot
{


	public class FaceIconView: FSpot.Widgets.IconView
	{
		public FaceIconView(IBrowsableCollection collection) : base(collection){
			this.DisplayTags = true;
			this.DisplayDates = false;
		}
		
		protected override void ContextMenu(ButtonPressEventArgs args, int cell_num)
		{
			uint timer = Log.InformationTimerStart ("Starting FaceService");
			string msg = Catalog.GetString ("Right Click!");
			string desc = Catalog.GetString ("Right Click!");
			
			FSpot.UI.Dialog.HigMessageDialog md = new FSpot.UI.Dialog.HigMessageDialog (MainWindow.Toplevel.Window, DialogFlags.DestroyWithParent, Gtk.MessageType.Error, ButtonsType.Ok, msg, desc);
			
			md.Run ();
			md.Destroy ();
			Log.DebugTimerPrint (timer, "FaceService startup took {0}");
		}
	}
}
