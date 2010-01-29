
using System;
using System.Drawing;
using System.Threading;

using FSpot.Extensions;
using FSpot.Utils;
using FSpot;
using Gtk;
using Mono.Unix;

using FaceSpot;
using FaceSpot.Db;
using System.Collections.Generic;

namespace FaceSpot
{
	public class FaceService : IService
	{
		public FaceService (){}
		
		public bool Start ()
		{
			uint timer = Log.InformationTimerStart ("Starting FaceService");
		
			//FIXME : if not declared, this can't be compiled
			TrainingJob.Equals("","");
			
			FaceTStatTracker.Instance.Execute();
			FaceScheduler.Instance.Execute();
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
