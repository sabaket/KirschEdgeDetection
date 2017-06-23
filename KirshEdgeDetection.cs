using System;
using static System.Console;
using System.IO;
using System.Linq;

namespace Bme121.Pa3
{
    
    static partial class Program
    {
        static void Main( )
        {
            string inputFile  = @"21_training.csv";
            string outputFile = @"21_training_edges.csv";
            int height;  // image height (number of rows)
            int width;  // image width (number of columns)
            
            //FileStream and StreamReader created to read the csv file
            FileStream input = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
            StreamReader inStream = new StreamReader(input);
            
            //The first two lines of the csv file represent the height and width respectively 
            height = int.Parse(inStream.ReadLine().Trim(new Char[] {' ', ','}));
            width = int.Parse(inStream.ReadLine().Trim(new Char[] {' ', ','}));
            
            //initialized the two Color arrays 
            Color[ , ] inImage = new Color[height,width];
            Color[ , ] outImage = new Color[height,width];
            
            //two for loops used to read the data  
            for(int i=0;i<height; i++)
            {
                //every line of the file is split into an int array 
                string line = inStream.ReadLine();
                int[] intensity = line.Split(',').Select(int.Parse).ToArray();
                for (int j=0;j<(4*width)-3; j++) 
                {
                    //The data for every pixel is stored as a Color object using the FromArgb method
                    inImage[i, j/4] = Color.FromArgb(intensity[j], intensity[j+1], intensity[j+2], intensity[j+3]);
                    j=j+3;   
                }
            }
            
            //The pixels of the sides of the image are stored as the same ones 
            for(int j=1;j<(width)-1;j++)
            {
                outImage [0,j] = inImage [0,j];
                outImage [height - 1, j] = inImage [height - 1, j];
            }
            
            for(int i=0; i<height; i++)
            {
                outImage [i,0] = inImage [i,0];
                outImage [i, (width)-1] = inImage [i, (width)-1];
            }
            
            // The output image is generated using Kirsch edge detection
            for(int i=1;i<height-1;i++)
            {
                for(int j=1; j<(width)-1;j++)
                {
                    outImage [i,j] = GetKirschEdgeValue(inImage[i-1,j-1], inImage[i-1,j], inImage[i-1, j+1],
                                                        inImage[i, j-1], inImage[i,j], inImage[i, j+1],
                                                        inImage[i+1, j-1], inImage[i+1, j], inImage[i+1, j+1]);
                }
            }
            
            // FileStream and StreamWriter streams are created to Write the output image to its csv file.
            FileStream output = new FileStream(outputFile, FileMode.Create, FileAccess.Write); 
            StreamWriter outStream = new StreamWriter(output);
            
            //The first two lines of the file show the height and width of the image
            outStream.WriteLine(height);
            outStream.WriteLine(width);
            
            //the int values of each Color are connected by a comma as a string for each line 
            //After the string of each line is completed the line is written into the file
            for(int i=0; i<height; i++)
            {
                string line = "";
                for(int j=0; j<(width); j++)
                {
                    line = line + outImage[i,j].A + "," + outImage[i,j].R + "," + outImage[i,j].G + "," + outImage[i,j].B +  ",";
                }
                outStream.WriteLine(line);
            }
            
            input.Dispose();
            inStream.Dispose();
            
            outStream.Dispose();
            output.Dispose();
        
        }
        
        // This method computes the Kirsch edge-detection value for pixel color
        // at the centre location given the centre-location pixel color and the
        // colors of its eight neighbours.  These are numbered as follows.
        // The resulting color has the same alpha as the centre pixel, 
        // and Kirsch edge-detection intensities which are computed separately
        // for each of the red, green, and blue components using its eight neighbours.
        // c1 c2 c3
        // c4    c5
        // c6 c7 c8
        static Color GetKirschEdgeValue( 
            Color c1, Color c2,     Color c3, 
            Color c4, Color centre, Color c5, 
            Color c6, Color c7,     Color c8 )
        {
            //calls the int GetKirschEdgeValue to calculate the three Red, Green, and Blue values of each pixel 
            int alpha = 255;
            int red = GetKirschEdgeValue(c1.R, c2.R, c3.R, c4.R, c5.R, c6.R, c7.R, c8.R);
            int green = GetKirschEdgeValue(c1.G, c2.G, c3.G, c4.G, c5.G, c6.G, c7.G, c8.G);
            int blue  = GetKirschEdgeValue(c1.B, c2.B, c3.B, c4.B, c5.B, c6.B, c7.B, c8.B);
            
            centre = Color.FromArgb( alpha, red, green, blue);

            //returns the centre Color object  
            return centre;
        }
        
        // This method computes the Kirsch edge-detection value for pixel intensity
        // at the centre location given the pixel intensities of the eight neighbours.
        // These are numbered as follows.
        // i1 i2 i3
        // i4    i5
        // i6 i7 i8
        static int GetKirschEdgeValue( 
            int i1, int i2, int i3, 
            int i4,         int i5, 
            int i6, int i7, int i8 )
        {
            //calculates and returns the max value of the 8 operations
            int result = Math.Max((5 *i1 + 5*i2 + 5*i3 + -3*i4 + -3*i5 + -3*i6 + -3*i7 + -3*i8),(-3 *i1 + 5*i2 + 5*i3 + 5*i4 + -3*i5 + -3*i6 + -3*i7 + -3*i8));
            result = Math.Max(result, (-3 *i1 + -3*i2 + 5*i3 + 5*i4 + 5*i5 + -3*i6 + -3*i7 + -3*i8));
            result = Math.Max(result, (-3 *i1 + -3*i2 + -3*i3 + 5*i4 + 5*i5 + 5*i6 + -3*i7 + -3*i8));
            result = Math.Max(result, (-3 *i1 + -3*i2 + -3*i3 + -3*i4 + 5*i5 + 5*i6 + 5*i7 + -3*i8));
            result = Math.Max(result, (-3 *i1 + -3*i2 + -3*i3 + -3*i4 + -3*i5 + 5*i6 + 5*i7 + 5*i8));
            result = Math.Max(result, (5 *i1 + -3*i2 + -3*i3 + -3*i4 + -3*i5 + -3*i6 + 5*i7 + 5*i8));
            result = Math.Max(result, (5 *i1 + 5*i2 + -3*i3 + -3*i4 + -3*i5 + -3*i6 + -3*i7 + 5*i8));
            
            if(result < 0) result = 0;
            if(result > 255) result = 255;

            return result;
        }
    }
    
    // Implementation of part of System.Drawing.Color.
    // This is needed because .Net Core doesn't seem to include the assembly 
    // containing System.Drawing.Color even though docs.microsoft.com claims 
    // it is part of the .Net Core API.
    struct Color
    {
        int alpha;
        int red;
        int green;
        int blue;
        
        public int A { get { return alpha; } }
        public int R { get { return red;   } }
        public int G { get { return green; } }
        public int B { get { return blue;  } }
        
        public static Color FromArgb( int alpha, int red, int green, int blue )
        {
            Color result = new Color( );
            result.alpha = alpha;
            result.red   = red;
            result.green = green;
            result.blue  = blue;
            return result;
        }
    }
}
