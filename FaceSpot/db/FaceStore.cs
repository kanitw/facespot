using System;
using Banshee.Database;
using Mono.Data.SqliteClient;
using FSpot.Utils;
using FSpot;
using FSpot.Widgets;
using FSpot.Extensions;
using Gdk;
using System.Collections.Generic;
namespace FaceSpot.Db
{
	public class FaceStore : DbStore<Face>
	{
		const string ALL_FIELD_NAME = "id, photo_id, photo_version_id, tag_id, tag_confirm, auto_detected, auto_recognized, left_x, top_y, width, photo_md5, time, icon ";
		public FaceStore (QueuedSqliteDatabase database, bool is_new)
			: base(database, false)
		{
			if ( ! is_new && Database.TableExists("faces")) return;
			Log.Debug("Facestore Db Init");
			//Add Database Initialization
			try{
			Database.ExecuteNonQuery(
				"CREATE TABLE faces (\n"+
			    "	id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, \n" +
			    "	photo_id INTEGER NOT NULL, \n"+
				"	photo_version_id INTEGER NOT NULL, \n"+
				" 	tag_id INTEGER NULL, \n" +
				"   tag_confirm BOOLEAN NOT NULL, \n"+
				"   auto_detected BOOLEAN NOT NULL, \n"+
				"   auto_recognized BOOLEAN NOT NULL, \n"+
			    "	left_x INTEGER NOT NULL, \n"+
			    "	top_y INTEGER NOT NULL, \n"+
			    "	width INTEGER NOT NULL, \n"+
			    " 	photo_md5 STRING NOT NULL, \n"+
				"	time INTEGER NOT NULL \n,"+
				"   icon TEXT NULL"+
				")"  );
			}catch ( Mono.Data.SqliteClient.SqliteSyntaxException ex){
				Log.Exception(ex);	
			}
			try{
				Database.ExecuteNonQuery("CREATE INDEX idx_photo_id ON faces(photo_id)");
			}catch ( Mono.Data.SqliteClient.SqliteSyntaxException ex){
				Log.Exception(ex);	
			}
			try{
				Database.ExecuteNonQuery("CREATE INDEX idx_photo_version_id ON faces(photo_version_id)");
			}catch ( Mono.Data.SqliteClient.SqliteSyntaxException ex){
				Log.Exception(ex);	
			}
			try{
				Database.ExecuteNonQuery("CREATE INDEX idx_tag_id ON faces(tag_id)");
			}
			catch ( Mono.Data.SqliteClient.SqliteSyntaxException ex){
				Log.Exception(ex);	
			}
			//FIXME Add "Alter" Table Query
		}
		
		public override Face Get (uint id)
		{
			Face face = LookupInCache (id);
			if (face != null)
				return face;
			SqliteDataReader reader = Database.Query (
				new DbCommand ("SELECT " + ALL_FIELD_NAME + 
					      "FROM faces " + 
					      "WHERE id = :id", "id", id)
			);
			if (reader.Read ())
			{
				face = AddFaceFromReader (reader);
			}
			reader.Close();
			//TODO consider whether to use Unsafed Add (Compared to PhotoStore Class)
			return face;
		}

		private Face AddFaceFromReader (SqliteDataReader reader)
		{
			Face face;
			Photo photo = Core.Database.Photos.Get (Convert.ToUInt32 (reader["photo_id"]));
			Tag tag = null;
			try { 
				tag = Core.Database.Tags.GetTagById(Convert.ToInt32 (reader["tag_id"]));
			}finally {}
			if( tag ==null) 
				Log.Debug("Null Tag of Face#"+Convert.ToUInt32 (reader["id"]));
			
			
			
			Pixbuf iconPixbuf = null;
			if (reader["icon"] != null){
				try {
					iconPixbuf = GetIconFromString (reader["icon"].ToString ());
				} catch (Exception ex) {
					Log.Exception ("Unable to load icon for Face#" + Convert.ToUInt32 (reader["id"]), ex);
				}
			}
			face = new Face (Convert.ToUInt32 (reader["id"]), Convert.ToUInt32 (reader["left_x"]), 
			                 Convert.ToUInt32 (reader["top_Y"]), Convert.ToUInt32 (reader["width"]), 
			                 photo,tag,Convert.ToBoolean(reader["tag_confirm"]),
			                 Convert.ToBoolean(reader["auto_detected"]),
			                 Convert.ToBoolean(reader["auto_recognized"]),
			                 iconPixbuf,Convert.ToInt64(reader["time"]));
			AddToCache (face);
			return face;
		}

		public Face[] GetKnownFaceByPhoto(Photo photo){
			return GetByPhoto(photo,"AND tag_confirm = 1 AND NOT tag_id IS NULL ");
		}
		
		public Face[] GetNotKnownFaceByPhoto(Photo photo){
			return GetByPhoto(photo," AND ( tag_id IS NULL OR tag_confirm = 0 )");	
		}
		
		public Face[] GetByPhoto(Photo photo, string addWhereClause){
			List<Face> faces = new List<Face>();
			SqliteDataReader reader = Database.Query (
				new DbCommand ("SELECT " + ALL_FIELD_NAME + 
				       "FROM faces " + 
				       "WHERE photo_id = :photo_id " + addWhereClause,
				       "photo_id",photo.Id
				      )
			);
			while(reader.Read()){
				Face face = LookupInCache (Convert.ToUInt32 (reader["id"]));
				if(face==null){
					face = AddFaceFromReader(reader);
				}
				faces.Add(face);
			}
			reader.Close();
			//TODO consider whether to use Unsafed Add (Compared to PhotoStore Class)
			return faces.ToArray();
		}
				
		public Face CreateFaceFromView (Photo photo, uint leftX, uint topY, uint width)
		{
			if(photo == null) Log.Exception(new Exception( "BUG Null"));
			Log.Debug("Face: Creating Pixbuf From View");
			Pixbuf pixbuf = GetFacePixbufFromView ();
			return CreateFace(photo,leftX,topY,width,pixbuf,null,false,false,false);
		}

		public Pixbuf GetFacePixbufFromView ()
		{
			PhotoImageView view = MainWindow.Toplevel.PhotoView.View;
			Pixbuf input = view.Pixbuf;
			Rectangle selection = FSpot.Utils.PixbufUtils.TransformOrientation ((int)view.PixbufOrientation <= 4 ? input.Width : input.Height, (int)view.PixbufOrientation <= 4 ? input.Height : input.Width, view.Selection, view.PixbufOrientation);
			Pixbuf iconPixbuf = new Pixbuf (input.Colorspace, input.HasAlpha, input.BitsPerSample, selection.Width, selection.Height);
			input.CopyArea (selection.X, selection.Y, selection.Width, selection.Height, iconPixbuf, 0, 0);
			return iconPixbuf;
		}
		public Face CreateFace (Photo photo, uint leftX, uint topY, uint width, Pixbuf iconPixbuf,Tag tag
		                        ,bool tagConfirmed,bool autoDetected,bool autoRecognized){
			long unix_time = DbUtils.UnixTimeFromDateTime( photo.Time);
			
			Log.Debug("CreateFace : Db Exec Query");
			//FIXME Check Whether MD5 Sum of Photo has been generated
			DbCommand dbcom = new DbCommand (
					"INSERT INTO faces (photo_id,photo_version_id, left_x," +
					"top_y, width, tag_id, tag_confirm, auto_detected, auto_recognized,"+
			        "photo_md5, time"+ ",icon"+ ")" +
					"VALUES (:photo_id,:photo_version_id, :left_x," +
					":top_y, :width, :tag_id, :tag_confirm, :auto_detected, :auto_recognized,"
					+":photo_md5, :time, :icon"+ ")",
					":photo_id", photo.Id,
			        ":photo_version_id",photo.DefaultVersionId,
					":left_x", leftX,
					":top_y", topY,
					":width", width,
			        ":tag_id", (( tag == null ) ? null :  (Object)tag.Id ),
			        ":tag_confirm", tagConfirmed,
			       	":auto_detected",autoDetected,
			       	":auto_recognized",autoRecognized,
					":photo_md5", "aaa",//photo.MD5Sum,
					":time", unix_time
			        ,":icon",GetIconString(iconPixbuf)
			        );
			Log.Debug(dbcom.ToString());
			uint id = (uint)Database.Execute (					dbcom				);
			Face face = new Face (id, leftX, topY, width, photo,tag,tagConfirmed,autoDetected,autoRecognized,iconPixbuf,unix_time);
			Log.Debug("Finished createFace : Db Exec Query");
			return face;
		}
 		public override void Commit (Face item)
 		{
			Commit(new Face[]{item});
		}
		
		public void Commit (Face[] items){
			uint timer = Log.DebugTimerStart ();
			Log.Debug("Face Commit Called");

			foreach (Face face in items){
				Database.ExecuteNonQuery(
					new DbCommand("UPDATE faces SET photo_id = :photo_id"+
					", tag_id = :tag_id, tag_confirm = :tag_confirm,"+
					"auto_detected = :auto_detected, auto_recognized = :auto_recognized, "+				                                       
				    "left_x = :left_x, top_y = :top_y,"+
					"width = :width , photo_md5 = :photo_md5,  icon = :icon  WHERE id= :id",
						"photo_id", face.photo.Id,
						"tag_id",face.tag !=null ? (Object) face.tag.Id : null,
						"tag_confirm", face.tagConfirmed,
				        "auto_detected",face.autoDetected,
				        "auto_recognized",face.autoRecognized,
						"left_x",face.LeftX,
						"top_y",face.TopY,
						"width",face.Width,
						"photo_md5",face.photo.MD5Sum,
				        "icon", GetIconString(face.iconPixbuf),                    
				        "id",face.Id));
			}
			
			Log.DebugTimerPrint (timer, "Face Commit took {0}");
 		}
		
		private string GetIconString(Pixbuf pixbuf){
			//TODO Test this logic
			if(pixbuf == null){
//				if ( face.iconWasCleared)
//					return String.Empty;
				return null;
			}
			byte[] data = GdkUtils.Serialize(pixbuf);
			return Convert.ToBase64String(data);
		}
		private Pixbuf GetIconFromString(string icon_string){
			try {
				return GdkUtils.Deserialize(Convert.FromBase64String(icon_string));
			}catch (Exception ex){
				Log.Exception(	ex);
				return null;
			}
		}
		
		public void Remove(Face[] faces){
			foreach(Face face in faces)	
				Remove(face);
		}
		
		public override void Remove (Face item)
		{
			RemoveFromCache (item);
			Database.ExecuteNonQuery (
				new DbCommand ("DELETE FROM faces WHERE id = :id", "id", item.Id));
			//TODO Study What is EmitRemoved ();
		}
		
		public void clearDatabase(){
			Log.Debug("DROP TABLE faces");
			Database.ExecuteNonQuery(new DbCommand("DROP TABLE faces"));
		}
		
		//TODO Add more Query
		//TODO Add "Emit" classes if necessary
	}
}
