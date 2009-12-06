
using System;
using FSpot;
namespace FaceSpot
{
	public class Face : DbItem, IDisposable
		//TODO Decide whether should it implement IComparable
	{
		//TODO add left_x, top_x, right_x, down_x
		//TODO design how to add thumbnails
		
		public Face (uint id) : base (id)
		{
		}
		
		public void Dispose()
		{
			//TODO Add required child item dispose
			
			System.GC.SuppressFinalize(this);
		}
	}
}
