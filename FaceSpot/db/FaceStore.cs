
using System;
using Banshee.Database;

namespace FaceSpot.Db
{


	public class FaceStore : DbStore<Face>
	{

		public FaceStore (QueuedSqliteDatabase database, bool is_new)
			:base(database,false)
		{
			//TODO Add Ensure FaceThumbnailDirectory ?? (Similar to PhotoStore.cs)(
			if ( ! is_new) return;
			//TODO Add Database Initialization
			//Database.ExecuteNonQuery(
			
			//TODO Add Database Index / Links
		}
		
		public override Face Get(uint id)
		{
			//TODO Add this
			return null;
		}
		public override void Remove (Face item)
		{
			//TODO Add this
			throw new System.NotImplementedException ();
		}
 		public override void Commit (Face item)
 		{
			//TODO Add this
 			throw new System.NotImplementedException ();
 		}
		//TODO Add more Query
	}
}
