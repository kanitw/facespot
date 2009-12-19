
using System;
using Gtk;
using Gdk;
using FSpot;

namespace FaceSpot
{


	public class PeopleTreeView : TreeView
	{
		TreeStore peopleTreeStore;
		TagStore TagStore{
			get {
				return MainWindow.Toplevel.Database.Tags;	
			}
		}
		// FIXME this is a hack.
		private static Pixbuf empty_pixbuf = new Pixbuf (Colorspace.Rgb, true, 8, 1, 1);
		CellRendererPixbuf pix_render;
		TreeViewColumn complete_column;
		CellRendererText text_render;
		private const int IdColumn = 0;
		private const int NameColumn = 1;
		public PeopleTreeView ()
		{
			Selection.Mode = SelectionMode.Single;
			HeadersVisible = false;
			
			peopleTreeStore = new TreeStore(typeof(uint),typeof(string));
			this.Model = peopleTreeStore;
			
			TagStore.ItemsAdded += TagStoreItemsAdded;
			TagStore.ItemsChanged += TagStoreItemsChanged;
			TagStore.ItemsRemoved += TagStoreItemsRemoved;
			
			complete_column = new TreeViewColumn();
			
			pix_render = new CellRendererPixbuf();
			complete_column.PackStart(pix_render,false);
			complete_column.SetCellDataFunc(pix_render, new TreeCellDataFunc(IconDataFunc));
			
			text_render = new CellRendererText();
			complete_column.PackStart( text_render, true);
			complete_column.SetCellDataFunc( text_render, new TreeCellDataFunc( NameDataFunc));
			
			AppendColumn(complete_column);
			
			RefreshPeopleTreeStore ();
			//TODO If have time - add people search.
		}

		private void RefreshPeopleTreeStore ()
		{
			peopleTreeStore.Clear();
			//treeStore.AppendValues(tag.Id,tag.NameT
			//new Category(
			PopulatePeopleCategories (peopleTreeStore, People.Tag, TreeIter.Zero);
			this.ExpandAll();
		}

		void PopulatePeopleCategories (TreeStore treeStore ,Tag parent,TreeIter parent_iter)
		{
			foreach (Tag tag in (parent as Category).Children) {
				if (tag is Category) {
					TreeIter iter = parent_iter.Equals(TreeIter.Zero) ?
						treeStore.AppendValues(tag.Id,tag.Name) : 
						treeStore.AppendValues(parent_iter, tag.Id,tag.Name);
					PopulatePeopleCategories (treeStore,tag,iter);
				}
			}
		}
		private void NameDataFunc (TreeViewColumn column,
					   CellRenderer renderer,
					   TreeModel model,
					   TreeIter iter)
		{
			// FIXME not sure why it happens...
			if (model == null)
				return;
	
			GLib.Value value = new GLib.Value ();
			Model.GetValue (iter, IdColumn, ref value);
			uint tag_id = (uint) value;
	
			Tag tag = TagStore.Get (tag_id) as Tag;
			(renderer as CellRendererText).Text = tag.Name;
		}		
		
		private void IconDataFunc (TreeViewColumn column, 
					   CellRenderer renderer,
					   TreeModel model,
					   TreeIter iter)
		{
			GLib.Value value = new GLib.Value ();
			Model.GetValue (iter, IdColumn, ref value);
			uint tag_id = (uint) value;
			Tag tag = TagStore.Get (tag_id) as Tag;
			
			if (tag.SizedIcon != null) {
				Cms.Profile screen_profile;
				if (FSpot.ColorManagement.Profiles.TryGetValue (Preferences.Get<string> (Preferences.COLOR_MANAGEMENT_DISPLAY_PROFILE), out screen_profile)) {
					//FIXME, we're leaking a pixbuf here
					Gdk.Pixbuf temp = tag.SizedIcon.Copy();
					FSpot.ColorManagement.ApplyProfile (temp, screen_profile);
					(renderer as CellRendererPixbuf).Pixbuf = temp;
				} else
					(renderer as CellRendererPixbuf).Pixbuf = tag.SizedIcon;
			} else
				(renderer as CellRendererPixbuf).Pixbuf = empty_pixbuf;
		}
		void TagStoreItemsRemoved (object sender, DbItemEventArgs<Tag> e)
		{
			RefreshPeopleTreeStore();
		}

		void TagStoreItemsChanged (object sender, DbItemEventArgs<Tag> e)
		{
			RefreshPeopleTreeStore();
		}

		void TagStoreItemsAdded (object sender, DbItemEventArgs<Tag> e)
		{
			RefreshPeopleTreeStore();
		}
		
		public TreePath PathAtPoint (double x, double y)
		{
			TreePath path_at_pointer = null;
			GetPathAtPos ((int) x, (int) y, out path_at_pointer);
			return path_at_pointer;
		}
	}
}
