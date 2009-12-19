
using System;
using FSpot;
using Banshee.Database;
using FSpot.Utils;
using Mono.Data.SqliteClient;

namespace FaceSpot
{


	public class PhotoStoreAddOn
	{
		QueuedSqliteDatabase Database;
		public PhotoStoreAddOn (QueuedSqliteDatabase database)
		{
			//Log.Debug("Try to Alter PhotoVersion");
			this.Database = database;
			try{
				Database.ExecuteNonQuery(
					new DbCommand("ALTER TABLE photo_versions ADD auto_detected BOOLEAN NOT NULL DEFAULT 0")
			   	);
			}
			catch(Exception ex){
				
				Log.Exception(ex);	
			}
		}
		
		public bool IsDetected(PhotoVersion version){
			SqliteDataReader reader = Database.Query (
				new DbCommand ("SELECT auto_detected FROM photo_versions " + 
					      "WHERE photo_id = :photo_id, version_id = :version_id",
						  "photo_id", version.Photo.Id,
			              "version_id",version.VersionId)
			);
			if (reader.Read ())
			{
				return Convert.ToBoolean( reader["auto_detected"] );
			}
			reader.Close();
			//TODO consider whether to throw exception
			throw new Exception("Version Not Found!");
			//return false;
		}
		
		public void SetDetected(PhotoVersion version,bool val){
			
			try {
	        	Database.ExecuteNonQuery(
    				new DbCommand("UPDATE photo_version SET auto_detected = :auto_detected" +
    			    	"WHERE photo_id = :photo_id AND version_id = :version_id",
				        "auto_detected",val,
    			        "photo_id",version.Photo.Id,
    			        "version_id",version.VersionId));
	        } catch (Exception ex){
				Log.Exception(ex);
			}
		}
	}
}
