
using System;
using Banshee.Database;
using FSpot.Utils;
using FSpot;

namespace FaceSpot.Db
{

	/// <summary>
	/// Facespot's Database Extension from F-Spot's Db
	/// </summary>
	public class FaceSpotDb : IDisposable
	{
		private static FaceSpotDb instance;
		public static FaceSpotDb Instance {
			get {
				if (instance == null) {
					FSpot.Utils.Log.Debug (">>Init FaceSpotDb");
					instance = new FaceSpotDb ();	
				}
				return instance; 
			}
			//set { instance = value; }
		}
		
		FaceStore face_store;
		PhotoStoreAddOn photo_store_addon;
		TrainingStore training_data;
		//bool empty;
		//string path;
		
		bool alreadyDisposed;

		public FaceStore Faces {
			get {
				return face_store;
			}
		}
		public TrainingStore TrainingData{
			get { return training_data; }	
		}
		
		public PhotoStoreAddOn PhotosAddOn
		{
			get { return photo_store_addon; }	
		}
		QueuedSqliteDatabase Database {
			get { return Core.Database.Database; }	
		}
		public void BeginTransaction(){
			Database.BeginTransaction();
		}
		public void RollbackTransaction(){
			if(Database.InTransaction)
			Database.RollbackTransaction();	
		}
		public void CommitTransaction(){
			if(Database.InTransaction)
			Database.CommitTransaction();	
		}
		
		private FaceSpotDb ()
		{
			uint timer = Log.DebugTimerStart ();
			Database.BeginTransaction ();
			//FIXME Decide whether to use true/false value
			face_store = new FaceStore (Database, false);
			photo_store_addon = new PhotoStoreAddOn(Database);
			training_data = new TrainingStore(Database);
			Database.CommitTransaction ();
			Log.DebugTimerPrint (timer, "FaceSpot Db Initialization took {0}");
		}
		#region Dispose
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}
		protected virtual void Dispose(bool isDisposing)
		{
			if(alreadyDisposed)return;
			if(isDisposing){//Free managed resources
			//	FaceDatabase.Dispose();
			}
			//Free eunmanaged resources
			alreadyDisposed = true;
		}
		#endregion
		~FaceSpotDb ()
		{
			Log.Debug ("Finalizer called on {0}. Should be Disposed", GetType ());
			Dispose (false);
		}
		
	}
}
