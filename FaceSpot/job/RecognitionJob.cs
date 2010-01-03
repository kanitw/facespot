
using System;
using Banshee.Kernel;
using FSpot.Utils;
using FSpot;
using FaceSpot.Db;

namespace FaceSpot
{


	public class RecognitionJob : FaceJob
	{
		protected override bool Execute ()
		{
			Log.Debug("Recognition Job Called #"+face.Id + " " + priority.ToString()
			           +" ("+Scheduler.ScheduledJobsCount+" Job(s) Left");
			
			if(!face.tagConfirmed)
				FaceClassifier.Instance.Classify(face);				
			else 
				Log.Debug("Face Confirmed Why resuggest");
			
			Log.Debug("Recognition Job Finished #"+face.Id);		
			return true;
		}
		//TODO Decide Whether to use persistent job on this type
		const bool persistent = false;
		public static RecognitionJob Create(Face face){
			return Create(face,JobPriority.Lowest);
		}
		
		public static RecognitionJob Create(Face face,JobPriority priority)
		{
			Log.Debug("RecognitionJob .Create "+face.Id);
			uint id = 0;	
			RecognitionJob job = new RecognitionJob(id,face.Id.ToString(),
			                                    DateTime.Now,
			                                    priority,
			                                    persistent);
			Scheduler.Schedule(job,priority);
			job.Status = FSpot.Jobs.JobStatus.Scheduled;
			return job;
		}
		Face face;
		
		public RecognitionJob(uint id, string job_options, DateTime run_at, JobPriority job_priority, bool persistent) 
			: base (id, job_options, job_priority, run_at, persistent)
		{
			this.priority = job_priority;
			this.face = FaceSpotDb.Instance.Faces.Get(Convert.ToUInt32(job_options));
		}
	}
}
