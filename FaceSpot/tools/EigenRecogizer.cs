
using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using FSpot.Utils;
using FaceSpot.Db;

namespace FaceSpot
{

	public class EigenRecogizer
	{
		/// <summary>
		/// load eigen value in EigenValueTags class
		/// </summary>
		/// <param name="eigenRec">
		/// A <see cref="EigenObjectRecognizer"/>
		/// </param>
		/// <returns>
		/// A <see cref="EigenValueTags"/>
		/// </returns>
		public static EigenValueTags RecordEigenValue(EigenObjectRecognizer eigenRec){
			
			EigenValueTags eigenValueTags = new EigenValueTags();
			const int MAX_EIGEN_LENGTH = 50;
			int nums_train = eigenRec.Labels.Length;
			
			float[][] eigenMatrix = new float[nums_train][];						    
			
			int max_eigenvalueLength = Math.Min(MAX_EIGEN_LENGTH, nums_train/5);
	         
			for(int i=0;i<nums_train;i++){
				
				Emgu.CV.Matrix<float> eigenValue = eigenRec.EigenValues[i];
								
				float[] temp = new float[max_eigenvalueLength];
				                     
				for(int k=0; k<max_eigenvalueLength; k++){					
					temp[k] = eigenValue.Data[k,0];
				}				
				eigenValueTags.Add(new VTag(temp, eigenRec.Labels[i]));							
				
			}		 				  		
			Log.Debug("eigenVTags Length = "+ eigenValueTags.eigenTaglist.Count);
			return eigenValueTags;
		}				
			
		/// <summary>
		/// Process PCA and save a serialized recognizer in specified savepath
		/// </summary>
		/// <param name="faces">
		/// A <see cref="List<FaceTag>"/>
		/// </param>
		/// <param name="savepath">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="filename">
		/// A <see cref="System.String"/>
		/// </param>
		//private void ProcessPCA(List<FaceTag> faces){		
		public static EigenValueTags ProcessPCA(Face[] faces){
			Log.Debug("Recognize Started...");		
			
			int numsFaces = faces.Length;
			
			List<Image<Gray, Byte>> train_imagesList = new List<Image<Gray, byte>>();
			List<string> train_labelsList = new List<string>();
			 					
			// input in this stage is faces (List of FaceTag)
			Log.Debug("total faces to be detected = "+ numsFaces);
				
			// load faces from detected data			
			int i = 0;
			
			for(int k=0;k<faces.Length;k++){
				
				if(faces[k].tag == null){
					//Log.Debug("null Tag :: id = {0}, name = {0}",faces[k].Id,faces[k].Name);
					continue;
				}
				else{
					Log.Debug("tag = {0}, id = {0}, name = {0}",faces[k].tag.Name,faces[k].Id,faces[k].Name);
				}
//				if(faces[k].iconPixbuf == null) {
//					Log.Debug("id = {0}, name = {0}",faces[k].Id,faces[k].Name);
//					continue;
//				}
				train_labelsList.Add(faces[k].tag.Name);
				train_imagesList.Add(ImageTypeConverter.ConvertPixbufToGrayCVImage(faces[k].iconPixbuf));
			}
			
			string[] train_labels = train_labelsList.ToArray();
			Image<Gray, Byte>[] train_images = train_imagesList.ToArray();
			
			
		    MCvTermCriteria crit = new MCvTermCriteria(0.0001);		 			
			EigenObjectRecognizer eigenRec = new EigenObjectRecognizer(train_images,train_labels,7500,ref crit);

			// Serialize
			SerializeUtil.Serialize("/home/hyperjump/eigenRec", eigenRec);			
			
			// save recognized data into file of eigen value and into EigenValueTags class								
			EigenValueTags eigenVtags = RecordEigenValue(eigenRec);															
			
			Log.Debug("Recognize Ended...");
			
			return eigenVtags;
		}
	}
}
