using System;
using FSpot;
using FSpot.Utils;
using Banshee.Kernel;
using FaceSpot.Db;
using System.Collections.Generic;

namespace FaceSpot
{
	public class DetectionJob : FaceJob
	{
		Photo photo;
		Face[] resultFaces;
		static JobStore job_store{
			get { return MainWindow.Toplevel.Database.Jobs; }	
		}

		public Photo Photo {
			get {
				return photo;
			}
		}

		public Face[] DetectedFaces {
			get {
				return resultFaces;
			}
		}
		
		protected override bool Execute ()
		{
			//if(photo.Id!=202)return false;
			Log.Debug("Detection Job Called #"+photo.Id + " " + priority.ToString()
			           +" ("+Scheduler.ScheduledJobsCount+" Job(s) Left)");
			FacePixbufPos[] facesPixbufPos = null;
			try {
				facesPixbufPos = FaceDetector.DetectFaceToPixbuf(photo);
			} catch (Exception ex) {
				Log.Exception(ex);
			}		
			FaceStore faceStore = FaceSpotDb.Instance.Faces;			
			Log.Debug("#faces = {0}",facesPixbufPos.Length);
			List<Face> faces = new List<Face>();
			for(int j=0;j<facesPixbufPos.Length;j++){
				Console.WriteLine("#"+j);
				FacePixbufPos f = facesPixbufPos[j];
				Log.Debug("left = {0}, right = {1}, width = {2},size = {3}",f.leftX, f.topY, f.pixbuf.Width,f.pixbuf.SaveToBuffer("jpeg").Length);
				Face face = faceStore.CreateFace(photo, f.leftX, f.topY, f.width, f.pixbuf, null, false, true, false);												
				faces.Add(face);
			}
			FaceSpotDb.Instance.PhotosAddOn.SetIsDetected(photo.DefaultVersion,true);
			resultFaces = faces.ToArray();
			Log.Debug("Detection Job Finished #"+photo.Id);	
			return true;
		}
		//TODO Decide Whether to use persistent job on this type
		const bool persistent = false;
		public static DetectionJob Create(Photo photo){
			//return (DetectionJob) job_store.Create(typeof(DetectionJob),photo.Id.ToString());
			return Create(photo,JobPriority.Lowest);
		}
		
		public static DetectionJob Create(Photo photo,JobPriority priority)
		{
			Log.Debug("DetectionJob .Create "+photo.Id);
			uint id = 0;	
			DetectionJob job = new DetectionJob(id,photo.Id.ToString(),
			                                    DateTime.Now,
			                                    priority,
			                                    persistent);
			Scheduler.Schedule(job,priority);
			job.Status = FSpot.Jobs.JobStatus.Scheduled;
			return job;
		}
		
		JobPriority priority;
//		public DetectionJob (uint id, string job_options, int run_at, JobPriority job_priority, bool persistent) 
//			: this (id, job_options, DbUtils.DateTimeFromUnixTime (run_at), job_priority, persistent)
//		{}

		public DetectionJob (uint id, string job_options, DateTime run_at, JobPriority job_priority, bool persistent) 
			: base (id, job_options, job_priority, run_at, persistent)
		{
			this.priority = job_priority;
			this.photo = MainWindow.Toplevel.Database.Photos.Get(Convert.ToUInt32(job_options));
		}
	}
}
