
using System;
using Banshee.Kernel;
using FSpot.Utils;
using FSpot;
using FaceSpot.Db;
using System.Collections.Generic;

namespace FaceSpot
{


	public class TrainingJob : FaceJob
	{
		
		protected override bool Execute ()
		{
			Log.Debug("Training Job Called # " + priority.ToString()
			           +" ("+Scheduler.ScheduledJobsCount+" Job(s) Left");
			//fix me here, now do a training process every time
			FaceStore faceStore = FaceSpotDb.Instance.Faces;
			Face[] faces = faceStore.GetAllFaces();			
			List<Face> faceList = new List<Face>();			
			
			foreach(Face f in faces){
				if(f.tagConfirmed)
					faceList.Add(f);
			}	
			if(faceList.Count>0)
				FaceTrainer.Train(faceList.ToArray());
			
			Log.Debug("Training Job Finished #");		
			return true;
		}
		//TODO Decide Whether to use persistent job on this type
		const bool persistent = false;
		
		public static TrainingJob Create()//,JobPriority priority)
		{
			Log.Debug("TrainingJob .Create ");
			uint id = 0;	
			JobPriority priority = JobPriority.Highest;
			TrainingJob job = new TrainingJob(id,"",
			                                    DateTime.Now,
			                                    priority,
			                                    persistent);
			Scheduler.Schedule(job,priority);
			job.Status = FSpot.Jobs.JobStatus.Scheduled;
			return job;
		}
		
		public TrainingJob(uint id, string job_options, DateTime run_at, JobPriority job_priority, bool persistent) 
			: base (id, job_options, job_priority, run_at, persistent)
		{
			this.priority = job_priority;
		}
	}
}
