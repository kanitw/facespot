
using System;
using System.Collections.Generic;
using FaceSpot.Db;
using FSpot;
using FSpot.Utils;
namespace FaceSpot
{
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
		{}
		
		public void Execute(){
			//TODO Add check whether queue is empty
			QueueUncheckedPhoto();	
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
