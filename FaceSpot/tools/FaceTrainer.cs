
using System;
using FSpot.Utils;

using NeuronDotNet.Core;
using NeuronDotNet.Core.Backpropagation;
using FaceSpot.Db;
using System.IO;
using System.Collections.Generic;

namespace FaceSpot	
{	

	public class FaceTrainer
	{				
		public static BackpropagationNetwork bpnet;
		
		public static void Train(Face[] faces){			
			TrainNetwork(EigenRecogizer.ProcessPCA(faces));
			FaceClassifier.Instance.LoadResource();
			//FaceSpotDb.Instance.Faces.ClearAutoRecognized();
		}
		
		/// <summary>
		/// train and save as the spcified path
		/// </summary>
		/// <param name="eigen">
		/// A <see cref="EigenValueTags"/>
		/// </param>
		private static void TrainNetwork(EigenValueTags eigen){
			Log.Debug("================ Train Started ================ ");
			
			string[] dLabels = eigen.FacesLabel;					
			int numInstances = eigen.eigenTaglist.Count;
			int inputNodes = eigen.eigenTaglist[0].val.Length;					
			int outputNodes = dLabels.Length;
			int hiddenNodes = inputNodes+outputNodes;
			
			float[][] trainInput = new float[numInstances][];
			float[][] trainOutput = new float[numInstances][];
			
			//Random r = new Random();
			int numstrain = 0;
			for(int i=0;i<numInstances;i++){				
				
				trainInput[numstrain] = new float[inputNodes];
				trainOutput[numstrain] = new float[outputNodes];
				
				for(int j=0;j<dLabels.Length;j++){
					if(eigen.eigenTaglist[i].tag.Equals(dLabels[j]))
						trainOutput[numstrain][j] = 0.9f;
					else
						trainOutput[numstrain][j] = 0.1f;
				}
				
				for(int j=0;j<inputNodes;j++){					
					trainInput[numstrain][j] = eigen.eigenTaglist[i].val[j];
				}
				numstrain++;
			}						
				

			// convert to double
			Log.Debug("nums train = "+ numstrain);
			double[][] trainInputD = new double[numstrain][];
			double[][] trainOutputD = new double[numstrain][];
			for(int i=0;i<numstrain;i++){				
				trainInputD[i] = new double[inputNodes];
				trainOutputD[i] = new double[outputNodes];
				for(int j=0;j<outputNodes;j++){
					trainOutputD[i][j] = trainOutput[i][j];
				}
				
				for(int j=0;j<inputNodes;j++){	
					trainInputD[i][j] = trainInput[i][j];
				}
			}						 					     													
					
//			TimeSpan tp = System.DateTime.Now.TimeOfDay;	
			
			Log.Debug("NeuronDotNet.Core.Backpropagation.SigmoidLayer inputLayer = new NeuronDotNet.Core.Backpropagation.SigmoidLayer(inputNodes)");
			Log.Debug("#in = {0}, #hid = {1}, #out = {2}",inputNodes,hiddenNodes,outputNodes);
			NeuronDotNet.Core.Backpropagation.SigmoidLayer inputLayer = new NeuronDotNet.Core.Backpropagation.SigmoidLayer(inputNodes);
			NeuronDotNet.Core.Backpropagation.SigmoidLayer hiddenlayer = new NeuronDotNet.Core.Backpropagation.SigmoidLayer(hiddenNodes);
			NeuronDotNet.Core.Backpropagation.SigmoidLayer outputlayer = new NeuronDotNet.Core.Backpropagation.SigmoidLayer(outputNodes);
			Log.Debug("BackpropagationConnector input_hidden =  new BackpropagationConnector(inputLayer, hiddenlayer);");
			BackpropagationConnector input_hidden =  new BackpropagationConnector(inputLayer, hiddenlayer);
			BackpropagationConnector hidden_output =  new BackpropagationConnector(hiddenlayer, outputlayer);
			
			input_hidden.Momentum = 0.3;
			hidden_output.Momentum = 0.3;
			Log.Debug("bpnet = new BackpropagationNetwork(inputLayer,outputlayer);");
			bpnet = new BackpropagationNetwork(inputLayer,outputlayer);
			Log.Debug("TrainingSet tset = new TrainingSet(inputNodes, outputNodes);");
			TrainingSet tset = new TrainingSet(inputNodes, outputNodes);			
			for(int i=0;i<numstrain;i++)
				tset.Add(new TrainingSample(trainInputD[i], trainOutputD[i]));
			
			// prevent getting stuck in local minima
			bpnet.JitterNoiseLimit = 0.0001;
			bpnet.Initialize();
			
			int numEpoch = 200;			
			bpnet.SetLearningRate(0.2);
			bpnet.Learn(tset, numEpoch);
						
			Log.Debug("error = {0}",bpnet.MeanSquaredError);
			
//			string savepath = facedbPath + "object/";
//			if(!Directory.Exists(savepath))
//				Directory.CreateDirectory(savepath);
			
			// Serialize
			string path = Path.Combine (FSpot.Global.BaseDirectory, "ann.dat");
			SerializeUtil.Serialize(path, bpnet);						
			
			// Deserialize
			//BackpropagationNetwork testnet = (BackpropagationNetwork)SerializeUtil.DeSerialize("nn.dat");
//			Log.Debug("error = {0}",bpnet.MeanSquaredError);
			//bpnet = (BackpropagationNetwork)SerializeUtil.DeSerialize("/home/hyperjump/nn.dat");
			//Log.Debug("error = {0}",bpnet.MeanSquaredError);
			
			// test by using training data
//			int correct = 0;			
//			for(int i=0;i<numInstances;i++){
//				
//				double[] v = new double[inputNodes];
//				for(int j=0;j<v.Length;j++){
//					v[j] = (double)eigen.eigenTaglist[i].val[j];
					//Console.Write("{0},",v[j]);
//				}								                                       	
				//Console.WriteLine();
			
//				double[] netOutput = bpnet.Run(v);
				//Console.WriteLine("net out:");
//				for(int j=0;j<netOutput.Length;j++)
//					Console.Write("{0},",netOutput[j]);
				
//				string result = FaceClassifier.Instance.AnalyseNetworkOutput(eigen, netOutput);
//				if(eigen.eigenTaglist[i].tag.Equals(result))
//					correct++;				
//			}
//			Log.Debug("% correct = " + (float)correct/(float)numInstances * 100);
			
			//Save Train Status

			
//			Log.Debug("Saving Train Status...");
			List<Tstate> tstateList = new List<Tstate>();
			int[] num = new int[dLabels.Length];
			Log.Debug("num length = {0}",num.Length);
			
			foreach(VTag vt in eigen.eigenTaglist){				
				for(int k=0;k<num.Length;k++)					
					if(vt.tag.Equals(dLabels[k]))
						num[k]++;							
			}			
			for(int k=0;k<dLabels.Length;k++){
				tstateList.Add(new Tstate(dLabels[k], num[k]));
			}

			FaceSpotDb.Instance.TrainingData.Trainstat = tstateList;						
			
//			Log.Debug("time ="+  System.DateTime.Now.TimeOfDay.Subtract(tp));																			
			Log.Debug("================ Train ended ================ ");
		}				
	}
}
