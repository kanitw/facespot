
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
			Log.Debug("Classify called - {0}",face.Id);
			float[] eigenValue = eigenRec.GetEigenDistances(ImageTypeConverter.ConvertPixbufToGrayCVImage(face.iconPixbuf));
			
			int inputNodes = bpnet.InputLayer.NeuronCount;
			double[] v = new double[inputNodes];
					
			//fixme - this is slow
			EigenValueTags eigenVTags = EigenRecogizer.RecordEigenValue(eigenRec);
						
			for(int j=0;j<inputNodes;j++){
				v[j] = (double)eigenValue[j];				                                       		
				Console.Out.Write("{0},",v[j]);
			}
			Console.WriteLine();
			
			Console.WriteLine("network output:");
			Log.Debug("mean sqr error = {0}",bpnet.MeanSquaredError);
			double[] output = bpnet.Run(v);
			
			for(int j=0;j<output.Length;j++)
				Console.Write("{0},",output[j]);
			Console.WriteLine();
			string suggestedName = FaceClassifier.AnalyseNetworkOutput(eigenVTags, output);			
			
			Log.Debug("no suggestion - id = {0}, name = {0}",face.Id, face.Name);
			
			if(suggestedName != null && suggestedName.Length != 0){
				Tag tag = MainWindow.Toplevel.Database.Tags.GetTagByName(suggestedName);
				if(tag ==null ) Log.Debug("Error: Doesn't Found Tag Name"+suggestedName);
				else  Log.Debug("Found Tag"+tag.Name);
				face.Tag = tag;
				Log.Debug("Classify Face#"+face.Id+" Finished : ="+suggestedName+"?");
			}else 
				Log.Debug("Classify Face#"+face.Id+" Finished - No suggestions");
			face.autoRecognized = true;
			FaceSpotDb.Instance.Faces.Commit(face);
			
			
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
			Log.Debug("AnalyseNetwork... max = "+max);
			if(max < 0.75)
				return null;
			
			string[] labels = eigenVTags.FacesLabel;
			
			return labels[maxIndex];
		}
		
		private void LoadTrainedNetwork(){
			Log.Debug("LoadTrainedNetwork called...");
			//fixme 
			//change loading method					
			//bpnet = (BackpropagationNetwork)SerializeUtil.DeSerialize("/home/hyperjump/nn.dat");
			bpnet = FaceTrainer.bpnet;
		}
		
		private void LoadEigenRecognizer(){
			Log.Debug("LoadEigenRecognizer called...");
			//fixme			
			//change loading method
			eigenRec = (EigenObjectRecognizer)SerializeUtil.DeSerialize("/home/hyperjump/eigenRec.dat");						
		}
		
	}	
}
