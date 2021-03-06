using System;
using FSpot;
using Gdk;
using FSpot.Widgets;
using FSpot.Utils;
using System.Collections;
using System.Collections.Generic;
	
namespace FaceSpot.Db
{
	public class Face : DbItem, IDisposable//, IBrowsableItem
		//TODO Decide whether should it implement IComparable
	{
		uint leftX,topY,width;	
		#region Position/Width Encapsulation
		public uint LeftX {
			get {
				return leftX;
			}
			set {
				leftX = value;
			}
		}

		public uint TopY {
			get {
				return topY;
			}
			set {
				topY = value;
			}
		}

		public uint Width {
			get {
				return width;
			}
			set {
				width = value;
			}
		}
		
		public 
		
		#endregion
		
		const int faceDefaultWidth = 100;
		
		public bool autoDetected, autoRecognized;
		
		//TODO Use Photo_md5 
		string photo_md5;
		
		public Photo photo;
		
		public Pixbuf iconPixbuf;
		public bool iconWasCleared;
		/// <summary>
		/// 
		/// </summary>
		private Tag tag;
		//private Tag[] rejectedTag;
		
		private List<Tag> rejectedTag;
		

		private bool tagConfirmed;
		public bool TagConfirmed{
			get{
//				FaceTStatTracker.Instance.TrackFaceDbStatus();
				return tagConfirmed;				
			}
			set{
				tagConfirmed = value;
				//FIXME Jump please fix this please
			//	FaceTStatTracker.Instance.TrackFaceDbStatus();
			}
		}
		private long unix_time;
		
		public Face (uint id,uint leftX,uint topY,uint width,Photo photo,
		             Tag tag,bool tagConfirmed, bool autoDetected, bool autoRecognized, Pixbuf icon,long unix_time) 
			: base (id)
		{
			this.leftX = leftX;
			this.topY = topY;
			this.width = width;
			this.photo = photo;
			this.tag = tag;
			this.tagConfirmed = tagConfirmed;
			this.autoDetected = autoDetected;
			this.autoRecognized = autoRecognized;
			this.iconPixbuf = icon;
			this.unix_time = unix_time;
			//FIXME Possible Error HERE
			photo_md5 = photo.MD5Sum;
		}
		
		public void Dispose()
		{
			System.GC.SuppressFinalize(this);
		}

		#region Implementing IBrowsableitem
//		public Tag[] Tags { get { return new Tag[]{tag};}}
//		
//		public DateTime Time {
//			get { return DbUtils.DateTimeFromUnixTime(unix_time); }
//		}
//		
//		public string Description{
//			get { return  "Photo of "+ tag.Name + " in "+ photo.Name +"("+ photo.Description +")"; }
//		}
//		
//		public uint Rating {
//			get { return photo.Rating; }	
//		}
//		public System.Uri DefaultVersionUri {
//			get { return photo.DefaultVersionUri; }
//		}
		#endregion
		
		public Rectangle Selection{
			get{return new Rectangle((int)LeftX,(int)TopY,(int)Width,(int)Width); }
		}
		
		public string Name {
			get { return tag==null ? null : tag.Name + (tagConfirmed ? "" : "?") ; }	
		}

		public Tag Tag {
			get {
				return tag;
			}
			set { 
				tag = value;
				FaceSpotDb.Instance.Faces.RemoveRejectedTag(this,tag);
			}
		}
		
		internal List<Tag> RejectedTagList {
			get {
				if(rejectedTag == null){
					rejectedTag = FaceSpotDb.Instance.Faces.GetRejectedTag(this);	
				}
				return rejectedTag;
			}
		}
		
		public Tag[] RejectedTag{
			get {
				List<Tag> list = RejectedTagList;
				
				if(list!=null) return list.ToArray();
				return new Tag[]{};
			}
		}
		
		public bool HasRejected(Tag tag){
			return RejectedTagList.Contains(tag);
		}
	}
}
