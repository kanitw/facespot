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
		//bool selected = false;
		bool firsttime = true;
		public FaceSidebarPage () : base(new FaceSidebarWidget(),
		                                 Catalog.GetString("Face"),
		                                 "gtk-index") 
		{
			(SidebarWidget as FaceSidebarWidget).Page = this;
		}
		
		protected override void AddedToSidebar ()
		{
			Log.Debug("FaceSidebar AddedToSidebar");
			FaceSidebarWidget widget = SidebarWidget as FaceSidebarWidget;
			Sidebar.SelectionChanged += widget.HandleSelectionChanged;
			Sidebar sidebar= MainWindow.Toplevel.Sidebar;
			sidebar.Notebook.SwitchPage += SidebarNotebookSwitchPage;
		}

		void SidebarNotebookSwitchPage (object o, SwitchPageArgs args)
		{
			//Log.Debug("FaceSidebar Notice that page switched");
			Sidebar sidebar= MainWindow.Toplevel.Sidebar;
			if ( sidebar.Notebook.CurrentPageWidget ==  this.SidebarWidget )
			{
				(SidebarWidget as FaceSidebarWidget).selected = true;
				
				//FIXME First time that you open this It'll not force user to use 1:1 ratio
				
				if(firsttime){ 
					firsttime = false;
				}
				else {
					Log.Debug("FaceSidebar Selected : Set Ratio 1:1");	
					MainWindow.Toplevel.PhotoView.View.SelectionXyRatio = 1.0;
				}
				
			}else if( (SidebarWidget as FaceSidebarWidget).selected){
				(SidebarWidget as FaceSidebarWidget).selected = false;	
				
				if(firsttime ) firsttime = false;
				else {
					Log.Debug("FaceSidebar UnSelected");
					try {
						MainWindow.Toplevel.PhotoView.View.SelectionXyRatio = 0.0;
					} catch(NullReferenceException ne){
						Log.Exception(ne);	
					}
				}
			}
		}
	}
	
}
