using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Dlls
{
    /// <summary>
    /// 模式处理
    /// </summary>
    public static class Mood
    {
        /// <summary>
        /// 灰度化
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <returns>新图像矩阵</returns>
        public static Mat ToGrey(Mat img)
        {
            if (img.Channels() == 1) return img;
            return img.CvtColor(ColorConversionCodes.BGR2GRAY);
        }

        private static int Saturate_cast(double a)
        {
            int b = (int)Math.Round(a);
            if (b > 255) return 255;
            if (b < 0) return 0;
            return b;
        }

        /// <summary>
        /// 改变亮度和对比度
        /// </summary>
        /// <param name="src">原图像矩阵</param>
        /// <param name="alpha">对比度数值（初始为100，范围为[0,300]）</param>        
        /// <param name="beta">亮度数值（初始为0，范围为[-150,-150]）</param>
        /// <returns>新图像矩阵</returns>
        public static Mat BriAndCon(Mat src, int alpha, int beta)
        {
            Mat dst = new Mat(src.Size(), src.Type());
            int cn = src.Channels();
            for (int y = 0; y < src.Rows; y++)
            {
                for (int x = 0; x < src.Cols; x++)
                {
                    if (cn == 3)
                    {
                        Vec3b temp = src.At<Vec3b>(y, x);
                        //Console.WriteLine(temp.Item0 + " " + temp.Item1 + " " + temp.Item2);
                        //Console.ReadKey();
                        byte b = (byte)Saturate_cast(temp.Item0 * alpha  * 0.01 + beta);
                        byte g = (byte)Saturate_cast(temp.Item1 * alpha  * 0.01 + beta);
                        byte r = (byte)Saturate_cast(temp.Item2 * alpha  * 0.01 + beta);
                        Vec3b color = new Vec3b
                        {
                            Item0 = b,
                            Item1 = g,
                            Item2 = r
                        };
                        dst.Set<Vec3b>(y, x, color);
                        //Console.WriteLine(temp.Item0 +" "+ temp.Item1+" " + temp.Item2);
                        //Console.WriteLine(color.Item0 + " " + color.Item1 + " " + color.Item2);
                        //Console.ReadKey();
                    }
                    else if (cn == 1)
                    {
                        byte color = src.At<byte>(y, x);
                        dst.Set<byte>(y, x, (byte)Saturate_cast(color * alpha *0.01 + beta));
                    }
                }
            }
            return dst;
        }

        /// <summary>
        /// RGB颜色调整（包括在颜色调整里面）
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="Rdel">R通道的偏差值[-255,255]</param>
        /// <param name="Gdel">G通道的偏差值[-255,255]</param>
        /// <param name="Bdel">B通道的偏差值[-255,255]</param>
        /// <returns>新图像矩阵</returns>
        public static Mat RGBColorAdjust(Mat img,int Rdel,int Gdel,int Bdel)
        {
            Mat[] channel = Cv2.Split(img);
            int row = img.Rows;
            int col = img.Cols;
            for(int i=0;i<row;i++)
                for(int j=0;j<col;j++)
                {
                    channel[0].Set(i, j, (byte)Saturate_cast(channel[0].At<byte>(i, j) + Bdel * 1.0));
                    channel[1].Set(i, j, (byte)Saturate_cast(channel[1].At<byte>(i, j) + Gdel * 1.0));
                    channel[2].Set(i, j, (byte)Saturate_cast(channel[2].At<byte>(i, j) + Rdel * 1.0));
                }
            Cv2.Merge(channel, img);
            //channel = Cv2.Split(img);
            //Console.WriteLine(channel[0].At<byte>(0, 0) + " " + channel[1].At<byte>(0, 0) + " " + channel[2].At<byte>(0, 0));
            return img;
        }

        /// <summary>
        /// HLS颜色调整（包括在颜色调整里面）
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="Hdel">H通道的偏差值[-255,255]</param>
        /// <param name="Ldel">L通道的偏差值[-255,255]</param>
        /// <param name="Sdel">S通道的偏差值[-255,255]</param>
        /// <returns>新图像矩阵</returns>
        public static Mat HLSColorAdjust(Mat img,int Hdel,int Ldel,int Sdel)
        {
            Mat dst = new Mat();
            dst=img.Clone();
            Cv2.CvtColor(dst, dst, ColorConversionCodes.BGR2HLS);
            // 色相 亮度 饱和度
            //Mat[] cn = Cv2.Split(img);
            //Console.WriteLine(cn[0].At<byte>(0, 0) + " " + cn[1].At<byte>(0, 0) + " " + cn[2].At<byte>(0, 0));
            //Mat[] channel = Cv2.Split(dst);
            //Console.WriteLine(channel[0].At<byte>(0, 0) + " " + channel[1].At<byte>(0, 0) + " " + channel[2].At<byte>(0, 0));
            dst=RGBColorAdjust(dst, Sdel, Ldel, Hdel);
            return dst.CvtColor(ColorConversionCodes.HLS2BGR);
        }

        /// <summary>
        /// HSV颜色调整（包括在颜色调整里面）
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="Hdel">H通道的偏差值[-255,255]</param>
        /// <param name="Sdel">S通道的偏差值[-255,255]</param>
        /// <param name="Vdel">V通道的偏差值[-255,255]</param>
        /// <returns>新图像矩阵</returns>
        public static Mat HSVColorAdjust(Mat img, int Hdel, int Sdel, int Vdel)
        {
            Mat dst = new Mat();
            dst = img.Clone();
            Cv2.CvtColor(dst, dst, ColorConversionCodes.BGR2HSV);
            dst = RGBColorAdjust(dst, Vdel, Sdel, Hdel);
            return dst.CvtColor(ColorConversionCodes.HSV2BGR);
        }

        /// <summary>
        /// 提色
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="type">颜色类型（0：红色；1：绿色；2：蓝色）</param>
        /// <returns>新图像矩阵</returns>
        public static Mat TakeColor(Mat img,int type)
        {
            //Console.WriteLine(img.Type());
            if (img.Channels() == 1) return img;
            Mat dst = new Mat(img.Size(), img.Type());
            Mat[] channel = Cv2.Split(img);
            Mat temp = new Mat(img.Size(), channel[0].Type(), new Scalar(0));
            if (type == 0)
            {
                Mat[] cn = new Mat[3];
                cn[0] = temp;
                cn[1] = temp;
                cn[2] = channel[2];
                Cv2.Merge(cn,dst);
                return dst;
            }
            if (type == 1)
            {
                //Console.WriteLine("!");
                Mat[] cn = new Mat[3];
                cn[0] = temp;
                cn[1] = channel[1];
                cn[2] = temp;
                Cv2.Merge(cn, dst);
                //Console.WriteLine("?");
                //Console.WriteLine(cn[1].At<byte>(0, 0));
                //Console.WriteLine(dst.At<Vec3b>(0, 0)[0] + " "+ dst.At<Vec3b>(0, 0)[1] + " "+ dst.At<Vec3b>(0, 0)[2]);
                return dst;
            }
            if (type == 2)
            {
                Mat[] cn = new Mat[3];
                cn[0] = channel[0];
                cn[1] = temp;
                cn[2] = temp;
                Cv2.Merge(cn, dst);
                return dst;
            }
            return img;
        }

        /// <summary>
        /// 滤色
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="type">颜色类型（0：红色；1：绿色；2：蓝色）</param>
        /// <returns>新图像矩阵</returns>
        public static Mat FilterColor(Mat img,int type)
        {
            //Console.WriteLine(img.Type());
            if (img.Channels() == 1) return img;
            Mat dst = new Mat(img.Size(), img.Type());
            Mat[] channel = Cv2.Split(img);
            Mat temp = new Mat(img.Size(), channel[0].Type(), new Scalar(0));
            if (type == 0)
            {
                Mat[] cn = new Mat[3];
                cn[0] = channel[0];
                cn[1] = channel[1];
                cn[2] = temp;
                Cv2.Merge(cn, dst);
                return dst;
            }
            if (type == 1)
            {
                //Console.WriteLine("!");
                Mat[] cn = new Mat[3];
                cn[0] = channel[0];
                cn[1] = temp;
                cn[2] = channel[1];
                Cv2.Merge(cn, dst);
                //Console.WriteLine("?");
                //Console.WriteLine(cn[1].At<byte>(0, 0));
                //Console.WriteLine(dst.At<Vec3b>(0, 0)[0] + " "+ dst.At<Vec3b>(0, 0)[1] + " "+ dst.At<Vec3b>(0, 0)[2]);
                return dst;
            }
            if (type == 2)
            {
                Mat[] cn = new Mat[3];
                cn[0] = temp;
                cn[1] = channel[1];
                cn[2] = channel[2];
                Cv2.Merge(cn, dst);
                return dst;
            }
            return img;
        }

        private static Mat ToHist(Mat img,out float cnt,out float[] val)
        {
            val = new float[256];
            cnt = 0;
            Mat[] mats = new Mat[] { img };
            int[] channels = new int[] { 0 };
            int[] histsize = new int[] { 256 };
            Rangef[] range = new Rangef[1];
            range[0].Start = 0.0f;
            range[0].End = 256.0f;
            Mat hist = new Mat();
            int scale = 2;
            Cv2.CalcHist(mats, channels, new Mat(), hist, 1, histsize, range);
            Mat dst = new Mat(256, 256 * scale, MatType.CV_8U, new Scalar(255));
            double minval = 0;
            double maxval = 0;
            Point minloc, maxloc;
            Cv2.MinMaxLoc(hist, out minval, out maxval, out minloc, out maxloc);
            int hpt = Saturate_cast(0.9 * 256);
            for (int i = 0; i < 256; i++)
            {
                float binval = hist.Get<float>(i);
                cnt = cnt + binval;
                val[i] = binval;
                int realval = Saturate_cast(binval * hpt / maxval);
                //Console.WriteLine(binval);
                Cv2.Rectangle(dst, new Point(i * scale, 255 - realval), new Point((i + 1) * scale - 1, 255), new Scalar(0));
            }
            //Console.WriteLine(cnt);
            return dst;
        }

        /// <summary>
        /// 直方图
        /// 如果是灰度图，显示明度直方图；如果是RGB图，显示红色、绿色、蓝色直方图。
        /// </summary>
        /// <param name="img">图像矩阵</param>
        /// <param name="cnt">图像总像素</param>
        /// <param name="val">各个通道像素值的像素值</param>
        /// <param name="type">通道类型（如果是灰度图，0：明度直方图；如果是RGB图，0：红色直方图、1：绿色直方图、2：蓝色直方图）。</param>
        public static void Hist(Mat img,out float cnt,out float[] val,int type)
        {
            val = new float[256];
            cnt = 0;
            int cn = img.Channels();
            if (cn == 1)
            {
                Mat dst = ToHist(img, out cnt, out val);
                Cv2.ImShow("明度直方图", dst);
                Cv2.WaitKey(0);
            }
            else if (cn == 3)
            {
                Mat[] mats = new Mat[3];
                mats = Cv2.Split(img);
                if (type == 0)
                {
                    Mat dst = ToHist(mats[2], out cnt, out val);
                    Cv2.ImShow("红色直方图", dst);
                    Cv2.WaitKey(0);
                }
                else if (type == 1)
                {
                    Mat dst = ToHist(mats[1], out cnt, out val);
                    Cv2.ImShow("绿色直方图", dst);
                    Cv2.WaitKey(0);
                }
                else if(type==2)
                {
                    Mat dst = ToHist(mats[0], out cnt, out val);
                    Cv2.ImShow("蓝色直方图", dst);
                    Cv2.WaitKey(0);
                }
            }
        }

        /// <summary>
        /// 裁剪
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="ll">左上角点的x坐标</param>
        /// <param name="lr">左上角点的y坐标</param>
        /// <param name="width">选择框的长度</param>
        /// <param name="height">选择框的宽度</param>
        /// <returns>裁剪后的矩阵</returns>
        public static Mat Cut(Mat img,int ll,int lr,int width,int height)
        {
            Rect ROI = new Rect(ll, lr, width, height);
            Mat ImageROI = new Mat(img, ROI);
            return ImageROI;
        }

        /// <summary>
        /// 反色
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <returns>新图像矩阵</returns>
        public static Mat Reverse(Mat img)
        {
            int row = img.Rows;
            int col = img.Cols;
            int cn = img.Channels();
            if (cn == 1)
            {
                Mat dst = new Mat(new Size(row, col), MatType.CV_8UC1);
                for (int i = 0; i <row; i++)
                {
                    for (int j = 0; j <col; j++)
                    {
                        int b = img.At<byte>(i, j);
                        dst.Set<byte>(i, j, (byte)(255 - b));
                    }
                }
                return dst;
            }
            else if(cn==3)
            {
                Mat dst = new Mat(new Size(row, col), MatType.CV_8UC3);
                for (int i = 0; i < row; i++)
                {
                    for(int j = 0; j < col; j++)
                    {
                        int b = img.At<Vec3b>(i, j)[0];
                        int g = img.At<Vec3b>(i, j)[1];
                        int r = img.At<Vec3b>(i, j)[2];
                        dst.Set<Vec3b>(i, j,new Vec3b((byte)(255-b),(byte)(255-g),(byte)(255-r)));
                    }
                }
                return dst;
            }
            return new Mat();
        }

        /// <summary>
        /// 图像大小
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="width">宽度（只能为整数）</param>
        /// <param name="height">高度（只能为整数）</param>
        /// <param name="type">插值方式（1：线性插值，2：区域插值，3：三次样条插值）</param>
        /// <returns></returns>
        public static Mat ImageSize(Mat img,int width,int height,int type) 
        {
            int cn = img.Channels();
            Mat dst;
            if (cn == 1)
                dst = new Mat(new Size(width, height), MatType.CV_8UC1);
            else
                dst = new Mat(new Size(width, height), MatType.CV_8UC3);
            if (type == 1) Cv2.Resize(img, dst, dst.Size(), 0, 0, InterpolationFlags.Linear);
            else if (type == 2) Cv2.Resize(img, dst, dst.Size(), 0, 0, InterpolationFlags.Area);
            else if (type == 3) Cv2.Resize(img, dst, dst.Size(), 0, 0, InterpolationFlags.Cubic);
            return dst;
        }

    }
}
