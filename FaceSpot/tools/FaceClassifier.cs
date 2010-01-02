
using System;
using FaceSpot.Db;
using NeuronDotNet.Core.Backpropagation;
using Emgu.CV;
using Emgu.CV.Structure;
using FSpot;
using FSpot.Utils;

namespace FaceSpot
{

	public class FaceClassifier
	{
		private double[] v;
		private BackpropagationNetwork bpnet;
		private EigenObjectRecognizer eigenRec;
		private static FaceClassifier instance;		
		
		public static FaceClassifier Instance
		{
			get { 
				if(instance == null)
					instance = new FaceClassifier();
				return instance;
			}
		}
		
		private FaceClassifier ()
		{
			LoadEigenRecognizer();
			LoadTrainedNetwork();
		}
		
		public void Classify(Face face){										                
			float[] eigenValue = eigenRec.GetEigenDistances(ImageTypeConverter.ConvertPixbufToGrayCVImage(face.iconPixbuf));
			
			int inputNodes = bpnet.InputLayer.NeuronCount;
			v = new double[inputNodes];
					
			//fixme - this is slow
			EigenValueTags eigenVTags = EigenRecogizer.RecordEigenValue(eigenRec);
						
			for(int j=0;j<inputNodes;j++)
				v[j] = (double)eigenValue[j];				                                       		
			
			string suggestedName = FaceClassifier.AnalyseNetworkOutput(eigenVTags, bpnet.Run(v));							
			if(suggestedName != null && suggestedName.Length != 0){
				Tag tag = MainWindow.Toplevel.Database.Tags.GetTagByName(suggestedName);
				if(tag ==null ) Log.Debug("Error: Doesn't Found Tag Name"+suggestedName);
				else  Log.Debug("Found Tag"+tag.Name);
				face.Tag = tag;
			}
			face.autoRecognized = true;
			FaceSpotDb.Instance.Faces.Commit(face);
			Log.Debug("Classify Face#"+face.Id+" Finished : ="+suggestedName+"?");
			
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
		public static string AnalyseNetworkOutput(EigenValueTags eigenVTags, double[] f){		
			//fixme ^^ change >> static
			double max = f[0];
			int maxIndex = 0;
						
			for(int i=0;i<f.Length;i++){			
				if(f[i] > max){
					maxIndex = i;
					max = f[i];
				}
			}									
			string[] labels = eigenVTags.FacesLabel;
			
			return labels[maxIndex];
		}
		
		private void LoadTrainedNetwork(){
			//fixme 
			//change loading method
			bpnet = (BackpropagationNetwork)SerializeUtil.DeSerialize("nn.dat");			
		}
		
		private void LoadEigenRecognizer(){
			//fixme			
			//change loading method
			eigenRec = (EigenObjectRecognizer)SerializeUtil.DeSerialize("/home/hyperjump/eigenRec");						
		}
		
	}	
}
