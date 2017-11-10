using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clahe_w
{
    // Again, bad practice, this class will only be the holder of a 
    // few methods (that could be static for all I care)

    class Clahe
    {
        int numberBinX = 8;
        int numberBinY = 8;
        int grayLevels = 128;
        float binXsize, binYsize;
        float clipLimit = 0.5F; // contrast limiting parameter
        float clipTolerance = 2.5F;
        int[,][] iHists, LUTs;
        Bitmap im, res;

        public Clahe(int nrBinX, int nrBinY, int gLevels, float cLimit, Bitmap image)
        {
            numberBinX = nrBinX;
            numberBinY = nrBinY;
            grayLevels = gLevels;
            im = image;
            clipLimit = cLimit;
            // now would be a good time to make sure that the X and Y size of the image are multiples of the numbers of bins
            // ...
            binXsize = im.Width / numberBinX;
            binYsize = im.Height / numberBinY;
        }

        void computeHistogram()
        {
            iHists = new int[numberBinX, numberBinY][];
            int x0, x1, y0, y1;

            for (int i = 0; i < numberBinX; i++)
                for (int j = 0; j < numberBinY; j++)
                {
                    x0 = (int)Math.Round(i * binXsize);
                    y0 = (int)Math.Round(j * binYsize);
                    x1 = (int)Math.Round((i + 1) * binXsize);
                    y1 = (int)Math.Round((j + 1) * binYsize);
                    iHists[i, j] = computeHistogram(x0, x1, y0, y1);
                }

        }

        int[] computeHistogram(int x0, int x1, int y0, int y1)
        {
            // directly histogram in INT array of 256 values (WARNING --> works for 8 bits)
            int[] iHisto = new int[256];
            Color c;
            int gScale;
            // standard histogram
            for (int x = x0; x < x1; x++)
            {
                for (int y = y0; y < y1; y++)
                {
                    // Get pixel color 
                    c = im.GetPixel(x, y);
                    // convert c to grayscale
                    gScale = (int)((c.R + c.G + c.B) / 3);
                    // put value in iHisto
                    iHisto[gScale] += 1;
                }
            }
            

            return iHisto;
        }

        void computeCumulativeHistogram()
        {
            for(int x=0;x<numberBinX;x++)
                for(int y=0;y<numberBinY;y++)
                {
                    for (int i = 1; i < 256; i++)
                        iHists[x,y][i] += iHists[x, y][i - 1];
                }
        }

        void computeEqualizationLUT()
        {
            // for each bin, compute histogram equalization and return a LUT
            LUTs = new int[numberBinX, numberBinY][];
            for (int i = 0; i < numberBinX; i++)
                for (int j = 0; j < numberBinY; j++)
                {
                    LUTs[i, j] = computeEqualizationLUT(iHists[i, j]);
                }
        }

        void equalizeHistogram()
        {
            int count = 0;
            int x0, x1, y0, y1;
            Color c, b;
            int gScale;
            // for each bin, apply LUT
            for (int i = 0; i < numberBinX; i++)
                for (int j = 0; j < numberBinY; j++)
                {
                    x0 = (int)Math.Round(i * binXsize);
                    y0 = (int)Math.Round(j * binYsize);
                    x1 = (int)Math.Round((i + 1) * binXsize);
                    y1 = (int)Math.Round((j + 1) * binYsize);
                    for (int x = x0; x < x1; x++)
                        for (int y = y0; y < y1; y++)
                        {
                            // get grayscale color 
                            c = im.GetPixel(x, y);
                            gScale = (int)((c.R + c.G + c.B) / 3);
                            gScale = transformPixelIntensity(x, y, i, j, gScale);
                            b = Color.FromArgb(gScale, gScale, gScale);
                            count++;
                            // set result
                            res.SetPixel(x, y, b);
                        }
                }
        }

        void clipContrast()
        {
            for (int i = 0; i < numberBinX; i++)
                for (int j = 0; j < numberBinY; j++)
                {
                    iHists[i, j] = clipContrast(iHists[i, j]);
                }
        }

        int[] clipContrast(int[] histo)
        {
            // in this function, we clip the histogram to a ceiling value
            int middle, top, bottom;
            bottom = 0;
            top = (int)Math.Round(clipLimit * 256);
            int diff;
            do
            {
                diff = 0;
                middle = (int)Math.Round((top + bottom) / 2F);
                for (int i = 0; i < 256; i++)
                {
                    if (histo[i] > middle)
                    {
                        diff += histo[i] - middle;
                    }
                }
                if (diff + middle == top)
                    break;
                else if (diff + middle > top)
                    top = middle;
                else 
                    bottom = middle;
            } while (top - bottom > clipTolerance);
            // now, the clip value = middle
            // compute diff
            diff = 0;
            for (int i = 0; i < 256; i++)
            {
                if (histo[i] > middle)
                {
                    diff += histo[i] - middle;
                    histo[i] = middle; // actually clip it here
                }
            }
            // redistribute excess
            diff = (int)Math.Round(diff/256F); // value to be redistributed to each bin
            for (int i = 0; i < 256; i++)
            {
                histo[i] += diff;
            }
                return histo;
        }

        int transformPixelIntensity(int x, int y, int binX, int binY, int pixVal)
        {
            float val = pixVal;

            // compute position of the corner points ... TODO : should be done only once
            float x0 = binXsize / 2;
            float x1 = im.Width - x0;
            float y0 = binYsize / 2;
            float y1 = im.Height - y0;
            float xi = (int)((binX + 0.5) * binXsize);
            float yi = (int)((binY + 0.5) * binYsize);
            int dx, dy;
            // first, corners
            if ((x <= x0 && (y <= y0 || y >= y1)) ||
                (x >= x1 && (y <= y0 || y >= y1)))
            {
                // go look in LUT, no interpolation
                val = LUTs[binX, binY][pixVal];
            }
            // then, bands
            else if (x <= x0 || x >= x1)
            {
                // linear interpolation
                if (y < yi)
                {
                    val = (yi - y) * LUTs[binX, binY-1][pixVal];
                    val += (binYsize - yi + y) * LUTs[binX, binY][pixVal];
                    val /= binYsize;
                }
                else if (y == yi)
                {
                    val = LUTs[binX, binY][pixVal];
                }
                else // y > yi
                {
                    val = (y - yi) * LUTs[binX, binY+1][pixVal];
                    val += (binYsize - y + yi) * LUTs[binX, binY][pixVal];
                    val /= binYsize;
                }
            }
            else if (y <= y0 || y >= y1)
            {
                // linear interpolation
                if (x < xi)
                {
                    val = (xi - x) * LUTs[binX-1, binY][pixVal];
                    val += (binXsize - xi + x) * LUTs[binX, binY][pixVal];
                    val /= binXsize;
                }
                else if (x == xi)
                {
                    val = LUTs[binX, binY][pixVal];
                }
                else // x > xi
                {
                    val = (x - xi) * LUTs[binX+1, binY][pixVal];
                    val += (binXsize - x + xi) * LUTs[binX, binY][pixVal];
                    val /= binXsize;
                }
            }
            // finally, all the rest --> bilinear interp
            else
            {
                dx = (int)Math.Round(x - xi);
                dy = (int)Math.Round(y - yi);
                if (x >= xi)
                {
                    if (y >= yi)
                        val = bilinear_interpolation(dx, dy, (int)binXsize, (int)binYsize,
                                LUTs[binX, binY][pixVal], LUTs[binX + 1, binY][pixVal], LUTs[binX, binY + 1][pixVal], LUTs[binX + 1, binY + 1][pixVal]);
                    else
                        val = bilinear_interpolation(dx, (int)binYsize+dy, (int)binXsize, (int)binYsize,
                                LUTs[binX, binY-1][pixVal], LUTs[binX + 1, binY-1][pixVal], LUTs[binX, binY][pixVal], LUTs[binX + 1, binY][pixVal]);
                }
                else
                {
                    if (y >= yi)
                        val = bilinear_interpolation((int)binXsize + dx, dy, (int)binXsize, (int)binYsize,
                            LUTs[binX-1, binY][pixVal], LUTs[binX, binY][pixVal], LUTs[binX-1, binY + 1][pixVal], LUTs[binX, binY + 1][pixVal]);
                    else
                        val = bilinear_interpolation((int)binXsize + dx, (int)binYsize + dy, (int)binXsize, (int)binYsize,
                            LUTs[binX-1, binY-1][pixVal], LUTs[binX, binY-1][pixVal], LUTs[binX-1, binY][pixVal], LUTs[binX , binY][pixVal]);
                }
            }


            if (val < 0 || val > 255) // debug - this should never happen
                ;

            return (int)Math.Round(val);
        }

        float bilinear_interpolation(int x, int y, int width, int height, int v1, int v2, int v3, int v4)
        {
            float val1, val2; //intermediate vals.
            float res;

            if (x < 0 || y < 0)
                ; // debug, this should never happen

            val1 = (x * v2 + (width - x) * v1) / (float)width;
            val2 = (x * v4 + (width - x) * v3) / (float)width;

            res = (val2 * y + ((float)height - y) * val1) / (float)height;

            if (res > 255)
                ;
            return res;
        }

        int[] computeEqualizationLUT(int[] hist)
        {
            int[] equalHist = new int[256];
            int count = (int)Math.Round(binXsize * binYsize)-1;
            for(int i=0;i<256;i++)
                equalHist[i] = (int)Math.Round((double)(hist[i] - hist[0])/count*(grayLevels-1));

            return equalHist;
        }

        public Bitmap Process()
        {
            res = new Bitmap(im);

            // 1. compute histograms
            computeHistogram();

            // 2. clip contrast
            clipContrast();

            // 3. compute cumulative histograms
            computeCumulativeHistogram();

            // 3. for each histogram, compute the equalization LUT
            computeEqualizationLUT();

            // 4. apply transformation based on LUTs
            equalizeHistogram();
            
            return res;
        }
    }
}
