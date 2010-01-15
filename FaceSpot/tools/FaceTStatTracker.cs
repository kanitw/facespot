
using System;
using FSpot;
using FaceSpot.Db;
using System.Collections.Generic;
using FSpot.Utils;

namespace FaceSpot
{


	public class FaceTStatTracker
	{
		private static FaceTStatTracker instance;
		public static FaceTStatTracker Instance{
			get{
				if(instance == null)
					instance = new FaceTStatTracker();
				
				return instance;
				
			}
		}
		public List<Tstate> GetFaceDbStat(){
			Face[] taggedFaces = FaceSpotDb.Instance.Faces.GetTaggedFace();
			List<Tag> result = new List<Tag>();			
			foreach(Face f in taggedFaces){				
				if(!result.Contains(f.Tag))
					result.Add(f.Tag);				
			}
			Tag[] tags = result.ToArray();											
			
			List<Tstate> faceDbStat = new List<Tstate>();			
			
			foreach(Tag t in tags){
				Face[] f = FaceSpotDb.Instance.Faces.GetConfirmedFaceByTag(t);
				faceDbStat.Add(new Tstate(t.Name, f.Length));
			}
			return faceDbStat;
		}
		
		public void TrackFaceDbStatus(){							

			
			List<Tstate> tstat = FaceSpotDb.Instance.TrainingData.Trainstat;
			List<Tstate> faceDbStat = GetFaceDbStat();				
			Log.Debug("tstat.Count = {0}, faceDbStat.Count = {1}", tstat.Count, faceDbStat.Count);
			
//			foreach(Tstate t in faceDbStat){
//				Log.Debug("name = {0}, num = {1}", t.name, t.num);
//			}
			int sumTstat = 0;
			foreach(Tstate t in tstat){
				sumTstat+=t.num;
			}
			
			int sumfaceDbStat = 0;
			foreach(Tstate t in faceDbStat){
				sumfaceDbStat+=t.num;
			}
			
			Log.Debug("#tstat = {0}, #faceDbStat = {1}", sumTstat, sumfaceDbStat);
			if(Math.Abs(sumTstat - sumfaceDbStat) > 2){
				
				TrainingJob.Create();
				
				while(!TrainingJob.finished);
				
				TrainingJob.finished = false;
				FaceSpotDb.Instance.Faces.ClearAutoRecognized();
				
				FaceSpotDb.Instance.Faces.RemoveNotConfirmTag();
				FaceScheduler.Instance.Execute();
			}
			
			Log.Debug("TrackFacesStatus ended");	
		}
		
		
	}
}
