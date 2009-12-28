
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

using Emgu.CV;
using Emgu.CV.Structure;

using FSpot;
	
using Gdk;

namespace FaceSpot
{


	public class FaceDetector
	{

		public FaceDetector ()
		{			
		}
		
		public static Pixbuf ConvertCVImageToPixbuf(Emgu.CV.Image<Bgr, byte> img){
			System.Drawing.Bitmap bmp = img.Bitmap;
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);			
			return new Pixbuf(ms.GetBuffer());
		}
		
		public static FacePixbufPos[] DetectToPixbuf(Photo photo){
			Console.WriteLine("DetectToPixbuf called...");
			FaceImagePos[] faces = Detect(photo);
			FacePixbufPos[] facesbuf = new FacePixbufPos[faces.Length];			
			
			for(int i=0;i<faces.Length;i++)
				facesbuf[i] = faces[i].toFacePixbufPos();
			
			return facesbuf;
		}
		
		public static FaceImagePos[] Detect(Photo photo){			
			Uri uri = photo.DefaultVersionUri;							
			return Detect(new Emgu.CV.Image<Bgr, Byte>(uri.AbsolutePath));									
		}
		
		/// <summary>
		/// Detect faces in the given image and return an array of face images
		/// </summary>
		/// <param name="image">
		/// A <see cref="Image"/>
		/// </param>
		/// <returns>
		/// A <see cref="Image"/>
		/// </returns>
		public static FaceImagePos[] Detect(Image<Bgr, Byte> image){
			Console.WriteLine("Detect start...");
			const bool drawRec = false;
			const int smallest_width = 16;
			const int cropped_width = 100;
			
			//Note that lowest confidence is 1 which means every face will be accepted
			const int faceDetectConfd = 2;						
			
			Image<Bgr, Byte> faceImage = null;
			List<Image<Bgr, Byte>> faceList = new List<Image<Bgr, byte>>();
			List<System.Drawing.Rectangle> recList = new List<System.Drawing.Rectangle>();
			List<FaceImagePos> faceImagePosList = new List<FaceImagePos>();
			
			//Convert it to Grayscale
			Image<Gray, Byte> gray = image.Convert<Gray, Byte>();
						
	        //Normalizes brightness and increases contrast of the image
	        gray._EqualizeHist();
			
			//fixme : make this xml access standardized
			Assembly _assembly = Assembly.GetExecutingAssembly();						
			UriBuilder uri = new UriBuilder(_assembly.CodeBase);
            string path = Uri.UnescapeDataString(uri.Path);
			String xmlDirpath = Path.GetDirectoryName(path) + "/tools/haarcascade/";			
			
			
	        //Load the HaarCascade objects			
	        HaarCascade face = new HaarCascade(xmlDirpath+ "haarcascade_frontalface_alt_tree.xml");	
	        HaarCascade lefteye = new HaarCascade(xmlDirpath+ "haarcascade_mcs_lefteye.xml");
			HaarCascade righteye = new HaarCascade(xmlDirpath+ "haarcascade_mcs_righteye.xml");
			HaarCascade mouth = new HaarCascade(xmlDirpath+ "haarcascade_mcs_mouth.xml");
			HaarCascade nose = new HaarCascade(xmlDirpath+ "haarcascade_mcs_nose.xml");					 					 				 
				
			// smallest dist between each face image, main criterian for images with more than one face
			float smallestSqrDist = (float)(2 * Math.Pow(smallest_width ,2));
				
	        //Detect the faces  from the gray scale image and store the locations as rectangle
	        //The first dimensional is the channel
	        //The second dimension is the index of the rectangle in the specific channel
	        MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(face, 1.1, 1, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new System.Drawing.Size(smallest_width, smallest_width));
			 
	        foreach (MCvAvgComp f in facesDetected[0])
	        {
				// eliminate those with low confidence
				if(f.neighbors < faceDetectConfd) continue;	
					
	            //Set the region of interest on the faces
	            gray.ROI = f.rect;
				
	            MCvAvgComp[][] lefteyesDetected = gray.DetectHaarCascade(lefteye, 1.1, 1, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new System.Drawing.Size(smallest_width, smallest_width));				
			    MCvAvgComp[][] righteyesDetected = gray.DetectHaarCascade(righteye, 1.1, 1, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new System.Drawing.Size(smallest_width, smallest_width));				
	            MCvAvgComp[][] mouthDetected = gray.DetectHaarCascade(mouth, 1.1, 1, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new System.Drawing.Size(smallest_width, smallest_width));								
			    MCvAvgComp[][] noseDetected = gray.DetectHaarCascade(nose, 1.1, 1, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new System.Drawing.Size(smallest_width, smallest_width));
				
				gray.ROI = System.Drawing.Rectangle.Empty;						
							
	            //if there is no eye in the specific region, the region shouldn't contains a face
	            //note that we might not be able to recoginize a person who ware glass in this case 
	            if ((lefteyesDetected[0].Length == 0 &&  righteyesDetected[0].Length == 0 ) ||
					(mouthDetected[0].Length == 0 && noseDetected[0].Length == 0)) 
						continue;            					
				
//				LogWriteLine("[] = " + eyesDetected[0].Length);						
										
				int higesteyeY = 0;
					
	            foreach (MCvAvgComp e in lefteyesDetected[0])
	            {		
//					LogWriteLine("len = "+e.neighbors);
					
	                if (e.neighbors >= 1)
	                {							
	                  System.Drawing.Rectangle eyeRect = e.rect;						
	                  eyeRect.Offset(f.rect.X, f.rect.Y);				  
							
					  if(drawRec)									
	                  	image.Draw(eyeRect, new Bgr(System.Drawing.Color.Red), 1);
							
					  if( f.rect.Y > higesteyeY ) higesteyeY = f.rect.Y;
	                }
	            }	
								
				foreach (MCvAvgComp e in righteyesDetected[0])
	            {		
					//LogWriteLine("len = "+e.neighbors);
					
	                if (e.neighbors >= 1)
	                {							
	                  System.Drawing.Rectangle eyeRect = e.rect;						
	                  eyeRect.Offset(f.rect.X, f.rect.Y);				  
					  if(drawRec)
	                  	image.Draw(eyeRect, new Bgr(System.Drawing.Color.Red), 1);
							
					  if( f.rect.Y > higesteyeY ) higesteyeY = f.rect.Y;
	                }
	            }	
				
				//LogWriteLine("highesteye = " + higesteyeY);
					
				int lowestMouth = int.MaxValue;
				foreach (MCvAvgComp e in mouthDetected[0])
	            {		
//					LogWriteLine("m len = " + e.neighbors);
					
	                if (e.neighbors >= 1)
	                {							
	                  System.Drawing.Rectangle mRect = e.rect;						
	                  mRect.Offset(f.rect.X, f.rect.Y);	
						
					  if(drawRec)
	                  	image.Draw(mRect, new Bgr(System.Drawing.Color.Green), 1);										  
							
					  if(e.rect.Y < lowestMouth) lowestMouth = e.rect.Y;
	                }
	            }								
//				LogWriteLine("lowestMouth = " + lowestMouth);
					
				foreach (MCvAvgComp e in noseDetected[0])
	            {		
//					LogWriteLine("m len = " + e.neighbors);
					
	                if (e.neighbors >= 1)
	                {							
	                  System.Drawing.Rectangle mRect = e.rect;						
	                  mRect.Offset(f.rect.X, f.rect.Y);	
					  if(drawRec)
	                  	image.Draw(mRect, new Bgr(System.Drawing.Color.Green), 1);										  
	                }
	            }		
				
					
				System.Drawing.Rectangle rect = f.rect;
				
				// check if this ROI is an subset in previous ROI
				for(int j=0;j<recList.Count;j++){
					System.Drawing.Rectangle r = recList[j];
						
					// if found then delete the bigger ROI from the list
					if(r.Contains(f.rect)){					
						recList.Remove(r);
						faceList.Remove(faceList[j]);
						break;
					}
				}
					
				float minDistSqr = float.MaxValue;
				for(int j=0;j<recList.Count;j++){
					System.Drawing.Rectangle r = recList[j];
					float distSqr = (float)(Math.Pow(r.X - f.rect.X,2) + Math.Pow(r.Y - f.rect.Y,2));
					
					// the bigger window found then set distSqr to Zero when there is face inside ROI 
					if(f.rect.Contains(new System.Drawing.Point(r.X,r.Y)) || lowestMouth > higesteyeY) distSqr = 0;					
					if(distSqr < minDistSqr) minDistSqr = distSqr;
				}
					
				//LogWriteLine("mindistSqr = " + minDistSqr);
				
				if(recList.Count==0 || minDistSqr > smallestSqrDist)
				{
					faceImage = image.Copy(rect);
					faceImage = faceImage.Resize(cropped_width, cropped_width);
					
					faceImagePosList.Add(new FaceImagePos(faceImage, (uint)f.rect.Left, (uint)f.rect.Top));
					//faceList.Add(faceImage);
					//recList.Add(f.rect);
				}				
			}
			
			//LogWriteLine("face# in pic = "+faceList.Count);
			return faceImagePosList.ToArray();
		}
	}
	
	public class FacePixbufPos{
		public Photo photo;
		public Pixbuf pixbuf;
		public uint leftX;
		public uint topY;
				
		public FacePixbufPos(Photo photo, Pixbuf pixbuf, uint leftX, uint topY){
			this.photo = photo;
			this.pixbuf = pixbuf;
			this.leftX = leftX;
			this.topY = topY;
		}
	}
	
	public class FaceImagePos{
		public Photo photo;
		public Emgu.CV.Image<Bgr, byte> image;
		public uint leftX;
		public uint topY;
		
		public FaceImagePos(Emgu.CV.Image<Bgr, byte> image, uint leftX, uint topY){
			this.photo = photo;
			this.image = image;
			this.leftX = leftX;
			this.topY = topY;
		}
		
		public FacePixbufPos toFacePixbufPos(){
			return new FacePixbufPos(photo, FaceDetector.ConvertCVImageToPixbuf(image), leftX, topY);
		}
	}
}
