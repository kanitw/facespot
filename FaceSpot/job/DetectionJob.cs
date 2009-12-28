
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
			Log.Debug("Detection Job Called #"+photo.Id);
			
			for(int i=0;i<4;i++){
				Photo p = MainWindow.Toplevel.Database.Photos.Get((uint)(i+201));
				
				FacePixbufPos[] faces = FaceDetector.DetectToPixbuf(p);
								         	
				for(int j=0;j<faces.Length;j++)
					faces[j].pixbuf.Save("out/job_"+i+"_"+j+".jpeg","jpeg");
				
//				FaceStore faceStore = FaceSpotDb.Instance.Faces;				
//				foreach(FacePixbufPos f in faces){
//					Face face = faceStore.CreateFace(f.photo, f.leftX, f.topY, (uint)f.pixbuf.Width, f.pixbuf, null, false, true, false);
//					faceStore.Commit(face);
//					FaceSpotDb.Instance.PhotosAddOn.SetIsDetected(f.photo.DefaultVersion, true);
//				}
				
				
			}
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
