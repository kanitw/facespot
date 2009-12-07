
using System;
using Banshee.Database;
using FSpot.Utils;
namespace FaceSpot.Db
{

	/// <summary>
	/// Facespot's Database Extension from F-Spot's Db
	/// </summary>
	public class FaceSpotDb : IDisposable
	{
		FaceStore face_store;
		
		bool empty;
		string path;
		
		bool alreadyDisposed;
		QueuedSqliteDatabase faceDatabase;
		public QueuedSqliteDatabase FaceDatabase{
			get { return faceDatabase;}	
		}
		
		public FaceSpotDb ()
		{
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
				FaceDatabase.Dispose();
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
		
		public Face CreateFace()
		{
			
		}
	}
}
