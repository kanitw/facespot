
using System;
using FSpot;

namespace FaceSpot
{
	public class People
	{
		private static Tag tag;
		public static Category Category {
			get { return (Category)Tag; }
		}
		public static Tag Tag {
			get {
				if (tag == null) {
					tag = MainWindow.Toplevel.Database.Tags.Get (3);
					if (!tag.Name.Equals ("People")) {
						tag = MainWindow.Toplevel.Database.Tags.GetTagByName ("People");
					}
				}
				return tag;
			}
		}
		
	}
}
