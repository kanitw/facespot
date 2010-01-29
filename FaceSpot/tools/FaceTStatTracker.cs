
using System;
using System.Threading;
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
		
		public void Execute(){										
			Log.Debug(">>> TrackFacesStatus started");
			bool validToTrain = false;
			//bool classNumsChanged = false;
			bool diffNumsInstaceExceed = false;
			
			int diffPercnt = 20;
			int MINFACES = 3;
			
			List<Tstate> tstat = FaceSpotDb.Instance.TrainingData.Trainstat;
			List<Tstate> faceDbStat = GetFaceDbStat();				
			Log.Debug("tstat.Count = {0}, faceDbStat.Count = {1}", tstat.Count, faceDbStat.Count);
			
			Log.Debug("face DbStat...");
			foreach(Tstate t in faceDbStat){				
				Log.Debug("name = {0}, num = {1}", t.name, t.num);
			}
			
			Log.Debug("face tStat...");
			foreach(Tstate t in tstat){				
				Log.Debug("name = {0}, num = {1}", t.name, t.num);
			}			
			
			int sumTstat = 0;
			List<Tstate> shorter;
			List<Tstate> longer = tstat.Count > faceDbStat.Count ? tstat : faceDbStat;
			if(longer == tstat)
				shorter = faceDbStat;
			else shorter = tstat;
			
			foreach(Tstate t in longer){	
//				Log.Debug("name  = {0}", t.name);
								
				if(diffNumsInstaceExceed) break;
				bool found = false;
				foreach(Tstate t_ in shorter){
					
					if(t_.name.Equals(t.name)){						
						found = true;
						Log.Debug("name = {0}, diff = {1}",t_.name, Math.Abs(t_.num-t.num));
						
						//if((float)Math.Abs(t_.num-t.num)/(float)t.num  >= (float)diffPercnt/100f){
						if(Math.Abs(t.num - t_.num) >= 2){
							diffNumsInstaceExceed = true;							
							break;
						}
					}
				}
				if(!found && t.num >= MINFACES-1 ) diffNumsInstaceExceed = true;
			}
			
//			if(tstat.Count != faceDbStat.Count)
//				classNumsChanged = true;
//			
//			int sumfaceDbStat = 0;
//			foreach(Tstate t in faceDbStat){
//				sumfaceDbStat+=t.num;
//			}
			

						
//			Log.Debug("#tstat = {0}, #faceDbStat = {1}", sumTstat, sumfaceDbStat);			
//			if(Math.Abs(sumTstat - sumfaceDbStat) > 2){
			
//			Log.Debug("classNumsChanged = {0}, diffNumsInstanc = {1}", classNumsChanged, diffNumsInstaceExceed);
			
			validToTrain =  diffNumsInstaceExceed;
			
			if(validToTrain){					
				
				Log.Debug("======================== new train ======================");
				
				TrainingJob job = TrainingJob.Create();
				
//				while(job.Status != FSpot.Jobs.JobStatus.Finished)
//					System.Threading.Thread.Sleep(100);			
				
				FaceSpotDb.Instance.Faces.ClearAutoRecognized();				
//				FaceSpotDb.Instance.Faces.RemoveNotConfirmTag();
				FaceScheduler.Instance.Execute();
			}
			
			Log.Debug(">>> TrackFacesStatus ended");	
		}			
	}
}
