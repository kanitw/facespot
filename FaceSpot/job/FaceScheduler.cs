
using System;
using System.Collections.Generic;
using FaceSpot.Db;
using FSpot;
using FSpot.Utils;
using Banshee.Kernel;
namespace FaceSpot
{
	//TODO Add FaceSpot Preference
	public class FaceScheduler
	{
		private static FaceScheduler instance;
		public static FaceScheduler Instance{
			get {
				if(instance == null)
				{
					instance = new FaceScheduler();
				}
				return instance;
			}
		}
		
		private FaceScheduler ()
		{
			//Scheduler.JobFinished += SchedulerJobFinished;	
			//Scheduler.JobStarted += SchedulerJobStarted;
		}


		void SchedulerJobFinished (IJob job)
		{
			Execute();
		}
		
		public void Execute(){
			if(Scheduler.ScheduledJobsCount == 0){
				QueueUncheckedPhoto();	
			}
		}
		
		public void QueueUncheckedPhoto()
		{
			Photo[] undetectedPhotos = FaceSpotDb.Instance.PhotosAddOn.GetUnDetectedPhoto();
			foreach( Photo photo in undetectedPhotos)	
			{
				Log.Debug("DetectionJob .Create "+photo.Id);
				DetectionJob job = DetectionJob.Create(photo);
			}
		}
	}
}
