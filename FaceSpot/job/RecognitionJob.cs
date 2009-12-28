
using System;
using Banshee.Kernel;
using FSpot.Utils;

namespace FaceSpot
{


	public class RecognitionJob : FaceJob
	{
		protected override bool Execute ()
		{
			throw new System.NotImplementedException ();
		}
		//TODO Decide Whether to use persistent job on this type
		const bool persistent = false;
		public static DetectionJob Create(Photo photo){
			return (DetectionJob) job_store.Create(typeof(DetectionJob),photo.Id.ToString());
		}
		
		public static DetectionJob Create(Photo photo,JobPriority priority)
		{
			uint id = 0;	
			DetectionJob job = new DetectionJob(id,photo.Id.ToString(),
			                                    DateTime.Now,
			                                    priority,
			                                    persistent);
			Scheduler.Schedule(job,priority);
			job.Status = FSpot.Jobs.JobStatus.Scheduled;
			return job;
		}
		
		public RecognitionJob(uint id, string job_options, int run_at, JobPriority job_priority, bool persistent) 
			: this (id, job_options, DbUtils.DateTimeFromUnixTime (run_at), job_priority, persistent)
		{
		}
	}
}
