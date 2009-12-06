using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Gtk;
using Mono.Unix;
using FSpot;
using FSpot.Utils;
using FSpot.Widgets;

namespace FaceSpot
{
	public class FaceSidebarPage : SidebarPage{
		
		public FaceSidebarPage () : base(new FaceSidebarWidget (),
		                                 Catalog.GetString("Face"),
		                                 "gtk-index") 
		{
			(SidebarWidget as FaceSidebarWidget).Page = this;
		}
		
		protected override void AddedToSidebar()
		{
			FaceSidebarWidget widget = SidebarWidget as FaceSidebarWidget;
			Sidebar.SelectionChanged += widget.HandleSelectionChanged;
		}
	}
	
}
