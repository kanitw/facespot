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
			Scheduler.JobFinished += SchedulerJobFinished;	
			//Scheduler.J
			MainWindow.Toplevel.Database.Photos.ItemsAdded += MainWindowToplevelDatabasePhotosItemsAdded;
			//Scheduler.JobStarted += SchedulerJobStarted;
		}

		void MainWindowToplevelDatabasePhotosItemsAdded (object sender, DbItemEventArgs<Photo> e)
		{
			Execute();
		}

		void SchedulerJobFinished (IJob job)
		{
			Log.Debug("Scheduler Job Finished");
			if( job is DetectionJob){
				DetectionJob dJob = (DetectionJob) job;
				Log.Debug("DJob Finished Event "+dJob.JobOptions);
				foreach(Face face in dJob.DetectedFaces){
					RecognitionJob.Create(face,dJob.priority);
				}
			}
			if( job is RecognitionJob){				
				
			}
			if( job is TrainingJob){
				Log.Debug(">>> SchedulerJobFinished : TrainingJob");				
			}
			Execute();
		}
		
		public void Execute(){
			if(Scheduler.ScheduledJobsCount == 0)
			{
				Log.Debug("No Job Try to Add New Job !!");
				//MainWindow.Toplevel.PhotoView.
				QueueAnyUncheckedPhoto();	
			}else {
				
			}
		}
		const int QUEUE_ENTRY_LIMIT = 2;
		public void QueueAnyUncheckedPhoto()
		{
			Photo[] undetectedPhotos = FaceSpotDb.Instance.PhotosAddOn.GetUnDetectedPhoto();
			int i=0;
			foreach( Photo photo in undetectedPhotos)	
			{
				//DetectionJob job = 
				DetectionJob.Create(photo);
				if(i++ == QUEUE_ENTRY_LIMIT) break;
			}
			
			
			if(FaceSpotDb.Instance.Faces.GetTaggedFace().Length == 0)
				return;
			
			i=0;
			if(FaceSpotDb.Instance.Faces.GetConfirmedFace().Length > FaceSpot.MIN_CONFIRMED_FACE_TO_RECOG){
				Face[] unRecognizedFace = FaceSpotDb.Instance.Faces.GetNotRecognizedFace();
				Log.Debug("Unrecognized Face : "+unRecognizedFace.Length);
				foreach( Face face in unRecognizedFace){
					RecognitionJob.Create(face);
					if(i++ == QUEUE_ENTRY_LIMIT) break;
				}
			}
		}
	}
}
