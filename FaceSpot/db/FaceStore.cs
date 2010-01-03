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
	//FIXME Handle Edited , PhotoVersion Deleted, PhotoVersion Edited
	
	public class FaceStore : DbStore<Face>
	{
		const string ALL_FIELD_NAME = "id, photo_id, photo_version_id, tag_id, tag_confirm, auto_detected, auto_recognized, left_x, top_y, width, photo_md5, time, icon ";
		public FaceStore (QueuedSqliteDatabase database, bool is_new)
			: base(database, false)
		{
			MainWindow.Toplevel.Database.Photos.ItemsChanged += MainWindowToplevelDatabasePhotosItemsChanged;
			MainWindow.Toplevel.Database.Photos.ItemsRemoved += MainWindowToplevelDatabasePhotosItemsRemoved;
			MainWindow.Toplevel.Database.Tags.ItemsRemoved += MainWindowToplevelDatabaseTagsItemsRemoved;
			MainWindow.Toplevel.Database.Tags.ItemsChanged += MainWindowToplevelDatabaseTagsItemsChanged;
			
			if ( is_new || !Database.TableExists("faces")) {
				Log.Debug("Facestore Db Init");
				InitFaceTable();
			}
			if( is_new || !Database.TableExists("facenotags")){						
				Log.Debug("InitFaceNoTagTable()");
				InitFaceNoTagTable();
					
			}
			Log.Debug("FaceStore Constuctor ended");
		}

		void MainWindowToplevelDatabaseTagsItemsChanged (object sender, DbItemEventArgs<Tag> e)
		{
			Log.Debug("Tags Item Change Handled By FaceStore");
			//Check for Removed Tags
//			List<Tag> removed_tag = new List<Tag>();
//			foreach(Tag tag in e.Items){
//				Tag t = MainWindow.Toplevel.Database.Tags.Get(tag.Id);
//				if(t ==null)
//					removed_tag.Add(tag);
//			}
//			RemoveTag(removed_tag.ToArray());	
			
		}
		
		void MainWindowToplevelDatabasePhotosItemsChanged (object sender, DbItemEventArgs<Photo> e)
		{
			Log.Debug("Photo Item Change Handled By FaceStore");
			//Check for Removed Photo 
//			List<Photo> removed_photo = new List<Photo>();
//			foreach(Photo photo in e.Items){
//				Photo p = MainWindow.Toplevel.Database.Photos.Get(photo.Id);
//				if(p ==null)
//					removed_photo.Add(photo);
//			}
//			RemoveAllFacesOfPhotos(removed_photo.ToArray());	
		}

		void MainWindowToplevelDatabaseTagsItemsRemoved (object sender, DbItemEventArgs<Tag> e)
		{
			Log.Debug("Tags Item Removed Handled By FaceStore");
			Tag[] tags = e.Items;
			RemoveTag (tags);
		}

		void RemoveTag (Tag[] tags)
		{
			if(tags.Length ==0)return;
			Log.Debug("Remove Tags From Faces");
			foreach (Tag tag in tags) {
				Face[] faces = GetByTag (tag, "");
				foreach (Face face in faces) {
					FaceSpotDb.Instance.Faces.DeclineTag (face);
				}
			}
		}

		void MainWindowToplevelDatabasePhotosItemsRemoved (object sender, DbItemEventArgs<Photo> e)
		{
			Log.Debug("Photo Item Removed Handled By FaceStore");
			Photo[] photos = e.Items;
			RemoveAllFacesOfPhotos (photos);
		}

		void RemoveAllFacesOfPhotos (Photo[] photos)
		{
			if(photos.Length ==0)return;
			Log.Debug("Remove All Face of Photos");
			foreach (Photo photo in photos) {
				Face[] faces = GetByPhoto (photo, "");
				foreach (Face face in faces) {
					FaceSpotDb.Instance.Faces.Remove (face);
				}
			}
		}
		
		void InitFaceNoTagTable(){
			Log.Debug("InitFaceNoTagTable called");
			try {
				Database.ExecuteNonQuery(
					"CREATE TABLE facenotags(\n"+
				    "	face_id INTEGER NOT NULL,\n"+
				   	"   tag_id INTEGER NOT NULL, \n"+
				   	"   UNIQUE (face_id, tag_id) \n"+
				    ")"                     );
			}
			catch (Exception ex)
			{
				Log.Exception(ex);	
			}
			Log.Debug("InitFaceNoTagTable ended");
		}
		
		void InitFaceTable(){
			Log.Debug("InitFaceTable called");
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
					"   icon TEXT NULL\n,"+
				    " UNIQUE (photo_id, left_x,top_y,width)"+                     
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
			
			Log.Debug("InitFaceTable ended");
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
				face = AddFaceFromReaderToCache (reader);
			}
			reader.Close();
			//TODO consider whether to use Unsafed Add (Compared to PhotoStore Class)
			return face;
		}							
		public Face[] GetAllFaces(){
		//fixme
//			if (face != null)
//				return face;
			SqliteDataReader reader = Database.Query (
				new DbCommand ("SELECT " + ALL_FIELD_NAME + 
					      "FROM faces ")
			);
			return AddFacesFromReaderToCache(reader);
		}
		
		private Face[] AddFacesFromReaderToCache (SqliteDataReader reader)
		{
			List<Face> faces = new List<Face> ();
			while (reader.Read ()) {
				Face face = LookupInCache (Convert.ToUInt32 (reader["id"]));
				if (face == null) {
					face = AddFaceFromReaderToCache (reader);
				}
				faces.Add (face);
			}
			reader.Close ();
			return faces.ToArray ();
		}
		private Face AddFaceFromReaderToCache (SqliteDataReader reader)
		{
			Face face;
			Photo photo = Core.Database.Photos.Get (Convert.ToUInt32 (reader["photo_id"]));
			Tag tag = null;
			try { 
				tag = Core.Database.Tags.GetTagById(Convert.ToInt32 (reader["tag_id"]));
			}finally {}
			//if( tag ==null) 
			//	Log.Debug("Null Tag of Face#"+Convert.ToUInt32 (reader["id"]));
			
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
		public Face[] GetKnownFacesByPhoto(Photo photo){
			return GetByPhoto(photo,"AND tag_confirm = 1 AND NOT tag_id IS NULL ");
		}
		public Face[] GetNotKnownFacesByPhoto(Photo photo){
			return GetByPhoto(photo," AND ( tag_id IS NULL OR tag_confirm = 0 )");	
		}
		public Face[] GetByPhoto(Photo photo, string addWhereClause){
			SqliteDataReader reader = Database.Query (
				new DbCommand ("SELECT " + ALL_FIELD_NAME + 
				       "FROM faces " + 
				       "WHERE photo_id = :photo_id " + addWhereClause,
				       "photo_id",photo.Id
				      )
			);
			return AddFacesFromReaderToCache (reader);
		}
		public Face[] GetConfirmedFaceByTag(Tag tag){
			return GetByTag(tag,"AND tag_confirm = 1");	
		}
		public Face[] GetNotConfirmedFaceByTag(Tag tag){
			return GetByTag(tag,"AND tag_confirm = 0");	
		}
		
		public Face[] GetByTag(Tag tag,string addWhereClause){
			if(tag == null) return new Face[0]{};
			SqliteDataReader reader = Database.Query (
				new DbCommand ("SELECT " + ALL_FIELD_NAME + 
				       "FROM faces " + 
				       "WHERE tag_id = :tag_id " + addWhereClause,
				       "tag_id",tag.Id
				      )
			);
			return AddFacesFromReaderToCache(reader);
		}
		public Face[] GetUntaggedFace(string addWhereClause){
			SqliteDataReader reader = Database.Query (
				new DbCommand ("SELECT " + ALL_FIELD_NAME + 
				       "FROM faces " + 
				       "WHERE tag_id IS NULL "+addWhereClause));
			return AddFacesFromReaderToCache(reader);
		}
		public Face[] GetNotRecognizedFace(){
			return GetUntaggedFace("AND auto_recognized = 0 AND tag_confirm = 0");
		}
		public Face CreateFaceFromView (Photo photo, uint leftX, uint topY, uint width)
		{
			if(photo == null) Log.Exception(new Exception( "BUG Null"));
			Log.Debug("Face: Creating Pixbuf From View");
			Pixbuf pixbuf = GetFacePixbufFromView ();
			return CreateFace(photo,leftX,topY,width,pixbuf,null,false,false,false);
		}
		
		internal List<Tag> GetRejectedTag(Face face){
			SqliteDataReader reader = Database.Query (
					new DbCommand ("SELECT tag_id " +
				       "FROM facenotags " + 
				       "WHERE face_id = :face_id ",
				       "face_id", face.Id
					)
				);
				List<Tag> notags = new List<Tag> ();
				while (reader.Read ()) {
					Tag tag = MainWindow.Toplevel.Database.Tags.Get(Convert.ToUInt32( reader["tag_id"]));
					if(tag!=null)
						notags.Add (tag);
				}
				reader.Close ();
				return notags;
		}
		
		public void AddRejectedTag(Face face,Tag tag){
			if(face==null || tag == null)return;
			if(face.HasRejected(tag))return;
			try {
				DbCommand dbcom= new	DbCommand(
					"INSERT INTO facenotags (face_id,tag_id) " +
				    "VALUES (:face_id,:tag_id)",
				     "face_id",face.Id,
				     "tag_id",tag.Id);
				Database.Execute ( dbcom);
				face.RejectedTagList.Add(tag);
			}catch (Exception ex){
				Log.Exception(ex);
			}
		}
		
		public void RemoveRejectedTag(Face face,Tag tag){
			if(face==null || tag == null)return;
			if(!face.HasRejected(tag))return;
			try {
				DbCommand dbcom = new DbCommand (
						"DELETE FROM facenotags WHERE face_id = :face_id , tag_id = :tag_id ",
						":face_id", face.Id,
				        ":tag_id", tag.Id 
				        );
				Database.Execute (dbcom	);
				face.RejectedTagList.Remove(tag);
			} catch (Exception ex) {
				Log.Exception(ex);
			}
		}

		public Pixbuf GetFacePixbufFromView ()
		{
			PhotoImageView view = MainWindow.Toplevel.PhotoView.View;
			
			Pixbuf input = view.Pixbuf;
			//HACK Load Image Directly from uri (Because sometimes the shown image
			//is rotated
			if(MainWindow.Toplevel.SelectedPhotos().Length == 0) return null;
			Photo photo = MainWindow.Toplevel.SelectedPhotos()[0];
			using (ImageFile img = ImageFile.Create (photo.DefaultVersionUri)) {
				input = img.Load ();
			}
			//HACK Copy Code from Crop Editor
			Rectangle selection = FSpot.Utils.PixbufUtils.TransformOrientation (
				(int)view.PixbufOrientation <= 4 ? input.Width : input.Height, 
			    (int)view.PixbufOrientation <= 4 ? input.Height : input.Width, 
			    view.Selection, view.PixbufOrientation);
			Pixbuf iconPixbuf = new Pixbuf (input.Colorspace, input.HasAlpha, input.BitsPerSample, selection.Width, selection.Height);
			input.CopyArea (selection.X, selection.Y, selection.Width, selection.Height, iconPixbuf, 0, 0);
			return iconPixbuf;
		}
		public Face CreateFace (Photo photo, uint leftX, uint topY, uint width, Pixbuf iconPixbuf,Tag tag
		                        ,bool tagConfirmed,bool autoDetected,bool autoRecognized){
			
			Log.Debug("CreateFace called");
			long unix_time = 10000;//DbUtils.UnixTimeFromDateTime( photo.Time);
			
			Log.Debug("CreateFace : Db Exec Query" );
			DbCommand dbcom = null;
			
			try{
			//FIXME Check Whether MD5 Sum of Photo has been generated
				dbcom = new DbCommand (
					"INSERT INTO faces (photo_id,photo_version_id, left_x," +
					"top_y, width, tag_id, tag_confirm, auto_detected, auto_recognized,"+
			        "photo_md5, time"+ ",icon"+ ")" +
					"VALUES (:photo_id,:photo_version_id, :left_x," +
					":top_y, :width, :tag_id, :tag_confirm, :auto_detected, :auto_recognized,"+
					":photo_md5, :time, :icon"+ ")",
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
			}catch(System.Exception e){
				Console.WriteLine(e.Message);
			}
			
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
		
		public void SetTag(Face face,Tag tag)
		{
			face.Tag = tag;
			ConfirmTag(face);
			if(face.Tag.Icon == null){
					face.Tag.Icon = face.iconPixbuf;
					MainWindow.Toplevel.Database.Tags.Commit(face.Tag);
			}
		}
		
		public void ConfirmTag(Face face)
		{
			face.tagConfirmed = true;
			Commit(face);
			if(!face.photo.HasTag(face.Tag)){
				face.photo.AddTag(face.Tag);
				MainWindow.Toplevel.Database.Photos.Commit(face.photo);
			}
		}
		public void DeclineTag(Face face)
		{
			Log.Debug("Declining Face#"+face.Id);
			face.tagConfirmed = false;
			if(face.Tag !=null)
				AddRejectedTag(face,face.Tag);
			else 
				Log.Debug("Decline Null Tag!!");
			face.Tag = null;
			Commit(face);
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
					"width = :width , photo_md5 = :photo_md5, icon = :icon  WHERE id= :id",
						"photo_id", face.photo.Id,
						"tag_id",face.Tag !=null ? (Object) face.Tag.Id : null,
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
			EmitChanged(items, new DbItemEventArgs<Face>(items));
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
