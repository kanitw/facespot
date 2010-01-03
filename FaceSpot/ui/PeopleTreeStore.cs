
using System;
using Gtk;
using FSpot;
using FSpot.Utils;

namespace FaceSpot
{
	//TODO Consider whether to Add On Tag Change / Removed Handler
	/// <summary>
	/// Create A Tree Store That has all people in itself
	/// </summary>
	public class PeopleTreeStore: TreeStore
	{
		public PeopleTreeStore () : base(typeof(String),typeof(Tag))
		{
			PopulatePeopleCategories(this,People.Tag,TreeIter.Zero,0);
		}
		
		void PopulatePeopleCategories (TreeStore treeStore ,Tag parent,TreeIter parentIter,int level)
		{
			foreach (Tag tag in (parent as Category).Children) {
				if (tag is Category) {
					//Log.Debug("Append  : "+tag.Name + " to "+parent.Name);
					TreeIter iter = 
						(parentIter.Equals(TreeIter.Zero) ?
						treeStore.AppendValues(tag.Name,/*parent,*/tag):
							treeStore.AppendValues(parentIter,tag.Name,/*parent,*/tag)) ;
					PopulatePeopleCategories (treeStore,tag,iter,level+1);
				}
			}
		}
	}
}
