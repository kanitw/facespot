
using System;
using FSpot.Utils;
using Banshee.Database;
using Mono.Data.SqliteClient;
using FSpot;
using System.Collections.Generic;
using FaceSpot.Db;

namespace FaceSpot
{


	public class TrainingStore
	{
		private Banshee.Database.QueuedSqliteDatabase database;
		private List<Tstate> trainstat;
		public List<Tstate> Trainstat{
			get{				
				trainstat = GetTrainstat();
				return trainstat;
			}
			set{
				
				trainstat = value;
				SaveTrainState(value);
			}
		}
		
		public TrainingStore (QueuedSqliteDatabase database)
		{		
			Log.Debug("TrainingStore Constuctor called");
			this.database = database;
			if (!database.TableExists("trainstat")) {
				Log.Debug("Trainstat Db Init");
				InitTranstatTable();
			}
			Log.Debug("TrainingStore Constuctor ended");
		}
		
		void InitTranstatTable(){
			Log.Debug("InitTraningTable called");
					
			try{
				database.ExecuteNonQuery(
					"CREATE TABLE trainstat (\n"+
				    "	id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, \n" +
					" 	tag_id INTEGER NOT NULL, \n" +				                         
					"   num INTEGER NOT NULL\n,"+
				    " UNIQUE (tag_id)"+                     
					")"  );
			}catch ( Mono.Data.SqliteClient.SqliteSyntaxException ex){
				Log.Exception(ex);	
			}
			
			Log.Debug("InitTraningTable ended");
		}
		
		public List<Tstate> GetTrainstat(){
			Log.Debug("GetTrainstat called");
			List<Tstate> tstate = new List<Tstate>();
			
			SqliteDataReader reader = database.Query (
				new DbCommand ("SELECT tag_id " +
			       "FROM trainstat "
				)
			);
			List<Tag> tagList= new List<Tag> ();
			while (reader.Read ()) {
				Tag tag = MainWindow.Toplevel.Database.Tags.Get(Convert.ToUInt32( reader["tag_id"]));
				if(tag!=null)
					tagList.Add (tag);
			}
			// now we got all tag
			reader = database.Query (
				new DbCommand ("SELECT num FROM trainstat"));
			
			int i=0;
			while(reader.Read()){
				tstate.Add(new Tstate(tagList[i++].Name, Convert.ToInt32(reader["num"])));
			}
			reader.Close ();
			Log.Debug("GetTrainstat ended");
			return tstate;
		}
		
		public void SaveTrainState(List<Tstate> tstate){
			Log.Debug("** SaveTrainState called **");
			//while(true);
			try {
				database.ExecuteNonQuery(new DbCommand("DELETE FROM trainstat"));
			} catch (Exception ex){
					Log.Exception(ex);
			}
			Log.Debug("DELETE FROM trainstat ... done");
			//while(true);
			SqliteDataReader reader = null;
			foreach(Tstate t in tstate){
				uint tag_id = 0;
				
				//Log.Debug("finding tag_id of name = {0}", t.name);
				try {
					// find tag_id of this name
					reader = database.Query (
						new DbCommand ("SELECT id " +
					       "FROM tags " + 
					       "WHERE name = :name ",
					       "name", t.name
						)
					);					
					while (reader.Read ()) {
						Tag tag = MainWindow.Toplevel.Database.Tags.Get(Convert.ToUInt32( reader["id"]));						
						if(tag!=null)
							tag_id = tag.Id;
					}
					//Log.Debug("name = {0}, tag_id = {0}",t.name, tag_id);
					
					database.ExecuteNonQuery(
						new DbCommand("INSERT INTO trainstat (tag_id,num) " +
							"VALUES (:tag_id,:num)",
							"tag_id",tag_id,
					        "num",t.num));  							
		        } catch (Exception ex){
					Log.Exception(ex);
				}				
			}
			reader.Close ();
			Log.Debug("SaveTrainState ended");
		}
		
//		public bool IsDetected(PhotoVersion version){
//			SqliteDataReader reader = Database.Query (			    
//				new DbCommand ("SELECT is_auto_detected FROM photo_versions " + 
//					      "WHERE photo_id = :photo_id, version_id = :version_id",
//						  "photo_id", version.Photo.Id,
//			              "version_id",version.VersionId)
//			);
//			if (reader.Read ())
//			{
//				return Convert.ToBoolean( reader["is_auto_detected"] );
//			}
//			reader.Close();
//			//TODO consider whether to throw exception
//			throw new Exception("Version Not Found!");
//			//return false;
//		}
	}
	
	public struct Tstate{
		public string name;
		public int num;
		public Tstate(string name, int num){
			this.name = name;
			this.num = num;
		}
	}
	
		
}
