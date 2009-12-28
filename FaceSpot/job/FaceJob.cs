
using System;
using Banshee.Kernel;
namespace FaceSpot
{


	public abstract class FaceJob : Job
	{ 
		public FaceJob(uint id, string job_options,JobPriority job_priority,
		              DateTime run_at,bool persistent) : 
			base(id, job_options, job_priority, run_at, persistent){}
	}
}
