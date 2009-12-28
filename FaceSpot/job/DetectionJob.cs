
using System;
using FSpot;
using FSpot.Utils;
using Banshee.Kernel;
using FaceSpot.Db;

namespace FaceSpot
{


	public class DetectionJob : FaceJob
	{
		Photo photo;
		
//		public override void Run ()
//		{
//			throw new System.NotImplementedException ();
//		}

		static JobStore job_store{
			get { return MainWindow.Toplevel.Database.Jobs; }	
		}
		
		protected override bool Execute ()
		{
			//if(photo.Id!=202)return false;
			Log.Debug("Detection Job Called #"+photo.Id);
									
			FacePixbufPos[] faces = FaceDetector.DetectToPixbuf(photo);
			
							         	
			for(int j=0;j<faces.Length;j++)
				faces[j].pixbuf.Save("out/job_"+j+photo.Name,"jpeg");
			
			FaceStore faceStore = FaceSpotDb.Instance.Faces;			
			Log.Debug("#faces = {0}",faces.Length);
			
			for(int j=0;j<faces.Length;j++){
				Console.WriteLine("#"+j);
				FacePixbufPos f = faces[j];
				Log.Debug("left = {0}, right = {1}, width = {2},size = {3}",f.leftX, f.topY, f.pixbuf.Width,f.pixbuf.SaveToBuffer("jpeg").Length);
				Face face = faceStore.CreateFace(photo, f.leftX, f.topY, (uint)f.pixbuf.Width, f.pixbuf, null, false, true, false);												
			}
			FaceSpotDb.Instance.PhotosAddOn.SetIsDetected(photo.DefaultVersion,true);
			Log.Debug("Detection Job Finished #"+photo.Id);
			
			
			return true;
		}
		
		//Decide Whether to use persistent job on this type
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
		
		public DetectionJob (uint id, string job_options, int run_at, JobPriority job_priority, bool persistent) 
			: this (id, job_options, DbUtils.DateTimeFromUnixTime (run_at), job_priority, persistent)
		{}

		public DetectionJob (uint id, string job_options, DateTime run_at, JobPriority job_priority, bool persistent) 
			: base (id, job_options, job_priority, run_at, persistent)
		{
			this.photo = MainWindow.Toplevel.Database.Photos.Get(Convert.ToUInt32(job_options));
		}
	}
}
