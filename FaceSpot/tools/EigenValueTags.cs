
using System;
using System.Collections;
using System.Collections.Generic;

namespace FaceSpot
{
	public class EigenValueTags
	{
		public List<VTag> eigenTaglist;
		
		private string[] facesLabel;
		public string[] FacesLabel{
			get{
				if(facesLabel==null) facesLabel = FindDistinctClass();		
				return facesLabel;
			}
		}
				                                                    
		public EigenValueTags ()
		{
			eigenTaglist = new List<VTag>();
		}
		
		public void Add(VTag vtag){
			eigenTaglist.Add(vtag);
		}		
		
		private string[] FindDistinctClass(){
			List<string> result = new List<string>();			
			foreach(VTag v in eigenTaglist){
				if(!result.Contains(v.tag))
					result.Add(v.tag);				
			}
			return result.ToArray();
		}
	}
	
	public class VTag{
		public string tag;
		public float[] val;
		
		public VTag(float[] val, string tag){
			this.val = val;
			this.tag = tag;
		}
	}
}
