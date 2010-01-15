
using System;
using FaceSpot.Db;
using NeuronDotNet.Core.Backpropagation;
using Emgu.CV;
using Emgu.CV.Structure;
using FSpot;
using FSpot.Utils;
using System.IO;

namespace FaceSpot
{

	public class FaceClassifier
	{
		
		private BackpropagationNetwork bpnet;
		private EigenObjectRecognizer eigenRec;
		private bool isReady = false;
		private static FaceClassifier instance;				
		public static FaceClassifier Instance
		{
			get { 
				if(instance == null){
					instance = new FaceClassifier();
					instance.LoadResource();
				}
				return instance;
			}
		}
		
		internal void LoadResource(){				
			isReady = LoadEigenRecognizer();
			isReady &= LoadTrainedNetwork();
		}		
		
		internal void Classify(Face face){			
			if(!isReady){				
				//Log.Debug(">>> Classify() not ready");
				return;
			}
			
			Log.Debug(">>> Classify() called - {0}", face.Id);
			
			System.Threading.Thread.Sleep(10);
			
			Emgu.CV.Image<Gray, byte> emFace = ImageTypeConverter.ConvertPixbufToGrayCVImage(face.iconPixbuf);			
			//emFace.Save("/home/hyperjump/out/"+face.Id + "a.png");
			
			//emFace.Save(face.Tag.Name+".jpg");
			float[] eigenValue = EigenObjectRecognizer.EigenDecomposite(emFace,eigenRec.EigenImages,eigenRec.AverageImage);
			//float[] eigenValue = eigenRec.GetEigenDistances(emFace);
			
			Log.Debug("eigenValue.Length = {0}", eigenValue.Length);
			if(bpnet == null){
				Log.Debug("bpnet == null");
				LoadTrainedNetwork();
			}
			int inputNodes = bpnet.InputLayer.NeuronCount;
			Log.Debug("bpnet.InputLayer.NeuronCount = {0}", bpnet.InputLayer.NeuronCount);
			double[] v = new double[inputNodes];
					
			//fixme - this is slow
			EigenValueTags eigenVTags = EigenRecognizer.Instance.RecordEigenValue(eigenRec);
			
			for(int j=0;j<inputNodes;j++){
				v[j] = (double)eigenValue[j];	
				
//				v[j] /= r.Next(1,3);
//				if(r.Next(1,6) > 3)
//					v[j] *= -1;
//				Console.Out.Write("{0},",v[j]);
			}
//			Console.WriteLine();
			
			//Console.WriteLine("network output:");
			//Log.Debug("mean sqr error = {0}",bpnet.MeanSquaredError);
			double[] output = bpnet.Run(v);
			
			for(int j=0;j<output.Length;j++)
				Console.Write("{0},",output[j]);
			Console.WriteLine();
			
			// suggestedname from neural network
			string suggestedName = AnalyseNetworkOutput(eigenVTags, output);					
			// suggestedname from nearest neighbor
			string sss = eigenRec.Recognize(ImageTypeConverter.ConvertPixbufToGrayCVImage(face.iconPixbuf));
			
			//Log.Debug("EigenObj = {0••••••••••••••••••}, ANN = {0}",sss,suggestedName);
			if( sss == null || sss.Length == 0){								
				suggestedName = null;
			}
			else{				
			//	suggestedName = sss;				
			}
				
			Log.Debug("no suggestion - id = {0}, name = {0}",face.Id, face.Name);
			
			if(suggestedName != null && suggestedName.Length != 0){
				Tag tag = MainWindow.Toplevel.Database.Tags.GetTagByName(suggestedName);
				if(tag ==null ) Log.Debug("Error: Doesn't Found Tag Name"+suggestedName);
				else  Log.Debug("Found Tag"+tag.Name);
				
				if(!face.HasRejected(tag)){
					Log.Debug("Classify Face#"+face.Id+" Finished : ="+suggestedName+"?");
					face.Tag = tag;
				}
				else 
					Log.Debug("Unfortunately Face#"+face.Id+" has already rejected ="+suggestedName+"!");
								
			}else 
				Log.Debug("Classify Face#"+face.Id+" Finished - No suggestions");
			
			FaceSpotDb.Instance.Faces.Commit(face);	
			face.autoRecognized = true;			
			
			Log.Debug(">>> Classify() ended - {0}", face.Id);
		}
		
		/// <summary>
		/// Interpret an output from neural network in a form of label using tag in EigenValueTags class
		/// </summary>
		/// <param name="eigenVTags">
		/// A <see cref="EigenValueTags"/>
		/// </param>
		/// <param name="f">
		/// A <see cref="System.Double[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public string AnalyseNetworkOutput(EigenValueTags eigenVTags, double[] f){		
			double max = f[0];
			int maxIndex = 0;
						
			for(int i=0;i<f.Length;i++){			
				if(f[i] > max){
					maxIndex = i;
					max = f[i];
				}
			}	
			Log.Debug("AnalyseNetwork... max = "+max);
			
			if(max < 0.8)
				return null;
			
			string[] labels = eigenVTags.FacesLabel;
			
			return labels[maxIndex];
		}
		
		private bool LoadTrainedNetwork(){
			Log.Debug("LoadTrainedNetwork called...");	
			try{	
				string path = Path.Combine (FSpot.Global.BaseDirectory, "ann.dat");
				bpnet = (BackpropagationNetwork)SerializeUtil.DeSerialize(path);			
			}catch(Exception e){
				Log.Exception(e);				
				return false;
			}
			Log.Debug("LoadTrainedNetwork ended...");
			return true;
		}
		
		private bool LoadEigenRecognizer(){
			Log.Debug("LoadEigenRecognizer called...");
			try{
				string path = Path.Combine (FSpot.Global.BaseDirectory, "eigen.dat");
				eigenRec = (EigenObjectRecognizer)SerializeUtil.DeSerialize(path);					
			}catch(Exception e){
				Log.Exception(e);
				return false;
			}
			Log.Debug("LoadEigenRecognizer ended...");
			return true;
		}
		
		/// <summary>
		/// Given savepath and filename, create a csv file containing set of eigen values.
		/// The csv is formatted according to WEKA classifer.
		/// </summary>
		/// <param name="eigenRec">
		/// A <see cref="EigenObjectRecognizer"/>
		/// </param>
		/// <param name="savepath">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="filename">
		/// A <see cref="System.String"/>
		/// </param>
		private void WriteEigenValueFile(EigenObjectRecognizer eigenRec, string savepath, string filename){
			
			// don't store eigen value more than this number
			const int MAX_EIGEN_LENGTH = 30;
			
			int nums_train = eigenRec.Labels.Length;
			
			float[] eigenvalue_float = new float[nums_train];
			float[][] eigenMatrix = new float[nums_train][];
				
		    TextWriter tw = new StreamWriter(savepath+filename+".csv");
			
			int max_eigenvalueLength = Math.Min(MAX_EIGEN_LENGTH, 4 + nums_train/5);
			if(nums_train < 5)
				max_eigenvalueLength = nums_train;
			
			// write header
			for(int i=0;i<max_eigenvalueLength;i++){
				tw.Write("a"+i+",");				
			}			
			tw.WriteLine("class");
	         
			for(int i=0;i<nums_train;i++){				
				Emgu.CV.Matrix<float> eigenValue = eigenRec.EigenValues[i];
					
				for(int k=0; k<max_eigenvalueLength; k++)														
					tw.Write(eigenValue.Data[k,0]+",");									
																								
				tw.WriteLine(eigenRec.Labels[i]);
			}		 				  		
	        tw.Close();	
		}
		
	}	
}
