
using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using FSpot.Utils;
using FaceSpot.Db;
using System.IO;

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
			
		//fix me
		//public static EigenObjectRecognizer processedEigen;
		
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
			Log.Debug("ProcessPCA Started...");		
			
			int numsFaces = faces.Length;
			
			List<Image<Gray, Byte>> train_imagesList = new List<Image<Gray, byte>>();
			List<string> train_labelsList = new List<string>();
				
			// load faces from detected data			
			int i = 0;
			
			for(int k=0;k<faces.Length;k++){				
				if(faces[k].Tag == null){
					//Log.Debug("null Tag :: id = {0}, name = {0}",faces[k].Id,faces[k].Name);
					continue;
				}
				
				train_labelsList.Add(faces[k].Tag.Name);
				train_imagesList.Add(ImageTypeConverter.ConvertPixbufToGrayCVImage(faces[k].iconPixbuf));
			}
			
			//fixme 
			for(int k=0; k<train_imagesList.Count;k++){
				train_imagesList[k] = train_imagesList[k].Resize(100,100);
			}
			
			string[] train_labels = train_labelsList.ToArray();
			Image<Gray, Byte>[] train_images = train_imagesList.ToArray();
			
		    MCvTermCriteria crit = new MCvTermCriteria(0.0001);		 			
			EigenObjectRecognizer eigenRec = new EigenObjectRecognizer(train_images,train_labels,7500,ref crit);

			string path = Path.Combine (FSpot.Global.BaseDirectory, "eigen.dat");
			// Serialize
			SerializeUtil.Serialize(path, eigenRec);			
			
			// save recognized data into file of eigen value and into EigenValueTags class								
			EigenValueTags eigenVtags = RecordEigenValue(eigenRec);															
			
			Log.Debug("ProcessPCA ended...");		
			
			return eigenVtags;
		}
	}
}
