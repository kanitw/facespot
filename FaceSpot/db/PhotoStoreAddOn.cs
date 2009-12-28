
using System;
using FSpot;
using Banshee.Database;
using FSpot.Utils;
using Mono.Data.SqliteClient;
using System.Collections.Generic;

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
				//SqliteDataReader reader = 
					Database.Query (
				new DbCommand ("SELECT is_auto_detected FROM photo_versions "));
				
			}
			catch(Exception ex){
				Log.Debug("Try Alter Table Failed");
				Log.Exception(ex);	
				Database.ExecuteNonQuery(
					new DbCommand("ALTER TABLE photo_versions ADD is_auto_detected BOOLEAN NOT NULL DEFAULT 0")
			   	);
			}
		}
		
		public bool IsDetected(PhotoVersion version){
			SqliteDataReader reader = Database.Query (
				new DbCommand ("SELECT is_auto_detected FROM photo_versions " + 
					      "WHERE photo_id = :photo_id, version_id = :version_id",
						  "photo_id", version.Photo.Id,
			              "version_id",version.VersionId)
			);
			if (reader.Read ())
			{
				return Convert.ToBoolean( reader["is_auto_detected"] );
			}
			reader.Close();
			//TODO consider whether to throw exception
			throw new Exception("Version Not Found!");
			//return false;
		}
		
		public void SetIsDetected(PhotoVersion version,bool val){
			Log.Debug("SetIsDetected isnull :: " + (version==null));
			try {
	        	Database.ExecuteNonQuery(
    				new DbCommand("UPDATE photo_versions SET is_auto_detected = :is_auto_detected " +
    			    	"WHERE photo_id = :photo_id AND version_id = :version_id",
				        "is_auto_detected",val,
    			        "photo_id",version.Photo.Id,
    			        "version_id",version.VersionId));
	        } catch (Exception ex){
				Log.Exception(ex);
			}
		}
		
		public Photo[] GetUnDetectedPhoto(){
				List<Photo> photo_list = new List<Photo>();
			try{
				SqliteDataReader reader = Database.Query(
					new DbCommand("SELECT photos.id AS pid "+
						"FROM photos INNER JOIN photo_versions ON "+
						"photos.default_version_id = photo_versions.version_id AND "+
						"photos.id = photo_versions.photo_id "+ 	
				        "WHERE photo_versions.is_auto_detected = 0 " ));
				Log.Debug("Get Undetected Photo");
				while(reader.Read()){
					Photo photo = MainWindow.Toplevel.Database.Photos.Get(
					      Convert.ToUInt32( reader["pid"]));
					if(photo != null)
						photo_list.Add(photo);
				}
				reader.Close();
			} catch (Exception ex){
				Log.Exception(ex);
			}
			return photo_list.ToArray();
		}
	}
}
