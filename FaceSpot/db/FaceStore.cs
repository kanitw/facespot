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
	{	//TODO Consider whether to override Emit Add, Change, Removed
		
		const string ALL_FIELD_NAME = "id, photo_id, tag_id, tag_confirm, left_x, top_y, width, photo_md5, time, icon ";
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
				" 	tag_id INTEGER NULL, \n" +
				"   tag_confirm BOOLEAN NULL, \n"+
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
				      "WHERE id = :id", "id", id
				     )
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
			Photo photo = Core.Database.Photos.Get ((uint)Convert.ToUInt32 (reader["photo_id"]));
			Pixbuf iconPixbuf = null;
			if (reader["icon"] != null)
				try {
					iconPixbuf = GetIconFromString (reader["icon"].ToString ());
				} catch (Exception ex) {
					Log.Exception ("Unable to load icon for Face#" + Convert.ToUInt32 (reader["id"]), ex);
				}
			face = new Face (Convert.ToUInt32 (reader["id"]), Convert.ToUInt32 (reader["left_x"]), Convert.ToUInt32 (reader["top_Y"]), Convert.ToUInt32 (reader["width"]), photo, iconPixbuf,Convert.ToInt64(reader["time"]));
			AddToCache (face);
			return face;
		}

		public Face[] GetKnownFaceByPhoto(Photo photo){
			return GetByPhoto(photo," AND NOT tag_id IS NULL");
		}
		
		public Face[] GetNotKnownFaceByPhoto(Photo photo){
			return GetByPhoto(photo," AND tag_id IS NULL");	
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
			if(photo == null) Log.Exception(new Exception( "BUGG Null"));
			//FIXME Face here are always create from view / selection
			PhotoImageView view = MainWindow.Toplevel.PhotoView.View;
			Pixbuf input = view.Pixbuf;
			Log.Debug("Face: Creating Pixbuf From View");
			Rectangle selection = FSpot.Utils.PixbufUtils.TransformOrientation ((int)view.PixbufOrientation <= 4 ? input.Width : input.Height,
											    (int)view.PixbufOrientation <= 4 ? input.Height : input.Width,
											    view.Selection, view.PixbufOrientation);
			Pixbuf iconPixbuf = new Pixbuf(input.Colorspace,input.HasAlpha,input.BitsPerSample,
				                            selection.Width,selection.Height);
			input.CopyArea (selection.X, selection.Y,
					selection.Width, selection.Height, iconPixbuf, 0, 0);
			
			return CreateFace(photo,leftX,topY,width,iconPixbuf);
		}
		public Face CreateFace (Photo photo, uint leftX, uint topY, uint width, Pixbuf iconPixbuf){
			long unix_time = DbUtils.UnixTimeFromDateTime( photo.Time);
			
			Log.Debug("CreateFace : Db Exec Query");
			//FIXME Check Whether MD5 Sum of Photo has been generated
			DbCommand dbcom = new DbCommand (
					"INSERT INTO faces (photo_id, left_x," +
					"top_y, width, photo_md5, time"+
			        ",icon"+                         
			        ")" +
					"VALUES (:photo_id, :left_x," +
					":top_y, :width, :photo_md5, :time"+
			        ", :icon"+
			        ")",
					":photo_id", photo.Id,
					":left_x", leftX,
					":top_y", topY,
					":width", width,
					":photo_md5", "aaa",//photo.MD5Sum,
					":time", unix_time
			        ,":icon",GetIconString(iconPixbuf)
			        );
			Log.Debug(dbcom.ToString());
			uint id = (uint)Database.Execute (					dbcom				);
			Face face = new Face (id, leftX, topY, width, photo,iconPixbuf,unix_time);
			Log.Debug("Finished createFace : Db Exec Query");
			return face;
		}
 		public override void Commit (Face item)
 		{
			Commit(new Face[]{item});
		}
		
		public void Commit (Face[] items){
			uint timer = Log.DebugTimerStart ();
			// TODO consider whether we need to change any more information
			// Only use a transaction for multiple saves. Avoids recursive transactions.
 			bool use_transactions = !Database.InTransaction && items.Length > 1;
			if(use_transactions) Database.BeginTransaction();
			foreach (Face face in items){
				Database.ExecuteNonQuery(
					new DbCommand("UPDATE faces SET photo_id = :photo_id"+
					", tag_id = :tag_id, tag_confirm = :tag_confirm, left_x = :left_x, top_y = :top_y,"+
					"width = :width , photo_md5 = :photo_md5,  icon = :icon  WHERE id= :id",
						"photo_id", face.photo.Id,
						"tag_id",face.tag.Id,
						"tag_confirm", face.tagConfirmed,
						"left_x",face.LeftX,
						"top_y",face.TopY,
						"width",face.Width,
						"photo_md5",face.photo.MD5Sum,
				        "icon", GetIconString(face.iconPixbuf),                    
				        "id",face.Id));
			}
			if(use_transactions) Database.CommitTransaction();
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
		//TODO Study "Commit" Flow - This action should be included with Commit??
		public void AddTag (Face face,Tag tag, bool confirmed)
		{
			face.tag = tag;
			face.tagConfirmed = confirmed;
			Database.ExecuteNonQuery(new DbCommand("UPDATE faces SET tag_id = :tag_id, tag_confirm = :tag_confirm WHERE id = :id",
			                         "tag_id",tag.Id,
			                         "tag_confirm",confirmed,
			                         "id",face.Id));
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
