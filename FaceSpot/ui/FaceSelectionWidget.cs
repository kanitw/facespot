
using System;
using FSpot.Widgets;
using Gtk;
namespace FaceSpot
{


	public class PeopleSelectionWidget : SaneTreeView
	{

		public PeopleSelectionWidget () 
			: base (new TreeStore (typeof(uint), typeof(string)))
		{
		}
	}
}
