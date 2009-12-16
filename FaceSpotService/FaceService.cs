
using System;
using FSpot.Extensions;
using FSpot.Utils;
using FSpot;
using Gtk;
using Mono.Unix;
namespace FaceSpot
{
	public class FaceService : IService
	{
		public FaceService (){}
		
		public bool Start ()
		{
			uint timer = Log.InformationTimerStart ("Starting FaceService");
			string msg = Catalog.GetString ("Face Service Start");
			string desc = Catalog.GetString ("Sample Alert Box");
			
			FSpot.UI.Dialog.HigMessageDialog md = new FSpot.UI.Dialog.HigMessageDialog (MainWindow.Toplevel.Window, DialogFlags.DestroyWithParent, Gtk.MessageType.Error, ButtonsType.Ok, msg, desc);
			md.Run ();
			md.Destroy ();
			Log.DebugTimerPrint (timer, "FaceService startup took {0}");
			return true;
		}
		
		public bool Stop ()
		{
			uint timer = Log.InformationTimerStart ("Stopping FaceService");
			Log.DebugTimerPrint (timer, "FaceService shutdown took {0}");
			return true;
		}
		
	}
}
