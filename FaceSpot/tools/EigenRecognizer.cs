
using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using FSpot.Utils;
using FaceSpot.Db;
using System.IO;

namespace FaceSpot
{

	public class EigenRecognizer
	{		
		private static EigenRecognizer instance;		
		public static EigenRecognizer Instance{
			get {
				if(instance == null)
					instance = new EigenRecognizer();
				return instance;
			}
		}				
		
		/// <summary>
		/// load eigen value in EigenValueTags class
		/// </summary>
		/// <param name="eigenRec">
		/// A <see cref="EigenObjectRecognizer"/>
		/// </param>
		/// <returns>
		/// A <see cref="EigenValueTags"/>
		/// </returns>
		public EigenValueTags RecordEigenValue(EigenObjectRecognizer eigenRec){
			
			EigenValueTags eigenValueTags = new EigenValueTags();
			const int MAX_EIGEN_LENGTH = 30;
			int nums_train = eigenRec.Labels.Length;
			
			float[][] eigenMatrix = new float[nums_train][];						    
			
			int max_eigenvalueLength = Math.Min(MAX_EIGEN_LENGTH, 4 + nums_train/5);
			if(nums_train < 5)
				max_eigenvalueLength = nums_train;
	         
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
		public EigenValueTags ProcessPCA(Face[] faces){			
			Log.Debug("ProcessPCA Started...");		
			int MINFACES = 3;
			int numsFaces = faces.Length;
			
			List<Image<Gray, Byte>> train_imagesList = new List<Image<Gray, byte>>();
			List<string> train_labelsList = new List<string>();
				
			// load faces from detected data						
			List<int> banList = new List<int>();
			
			// filter too small number of faces
			for(int i=0;i<faces.Length;i++){
				uint cnt = 0;
				for(int j=0;j<faces.Length;j++){
					if(i==j || faces[i].Tag.Name.Equals(faces[j].Tag.Name)){
						cnt++;
					}
				}
				if(cnt < MINFACES)
					banList.Add(i);
			}
			
			for(int k=0;k<faces.Length;k++){				
				if(faces[k].Tag == null || banList.Contains(k)){
					//Log.Debug("null Tag :: id = {0}, name = {0}",faces[k].Id,faces[k].Name);
					continue;
				}
				
				train_labelsList.Add(faces[k].Tag.Name);
				train_imagesList.Add(ImageTypeConverter.ConvertPixbufToGrayCVImage(faces[k].iconPixbuf));
			}
			
			//FIXME
			for(int k=0; k<train_imagesList.Count;k++){
				train_imagesList[k] = train_imagesList[k].Resize(100,100);
			}
			
			string[] train_labels = train_labelsList.ToArray();
			Image<Gray, Byte>[] train_images = train_imagesList.ToArray();
			
		    MCvTermCriteria crit = new MCvTermCriteria(0.0001);		 			
			EigenObjectRecognizer eigenRec = new EigenObjectRecognizer(train_images,train_labels,4000,ref crit);

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
