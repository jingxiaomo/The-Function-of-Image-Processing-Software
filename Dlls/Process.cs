using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Dlls
{
    /// <summary>
    /// 处理
    /// </summary>
    public static class Process
    {
        /// <summary>
        /// 均值滤波
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="size">内核大小（3×3/5×5/7×7）</param>
        /// <param name="times">迭代次数[1,10]</param>
        /// <returns>新图像矩阵</returns>
        public static Mat Blur(Mat img,int size,int times)
        {
            Size s = new Size(size, size);
            Mat dst=new Mat(img.Size(),img.Type());
            Cv2.Blur(img, dst, s);
            for (int i = 1; i < times; i++)
                Cv2.Blur(dst, dst, s);
            return dst;
        }

        /// <summary>
        /// 高斯滤波
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="size">内核大小（3：3×3/5：5×5/7：7×7）</param>
        /// <param name="times">迭代次数[1,10]</param>
        /// <param name="st">强度[1,10]</param>
        /// <returns>新图像矩阵</returns>
        public static Mat Gaus(Mat img, int size, int times,int st)
        {
            Size s = new Size(size, size);
            Mat dst = new Mat(img.Size(), img.Type());
            Cv2.GaussianBlur(img, dst, s,st,st);
            for (int i = 1; i < times; i++)
                Cv2.GaussianBlur(dst, dst, s,st,st);
            return dst;
        }

        /// <summary>
        /// 中值滤波
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="size">内核大小</param>
        /// <param name="times">迭代次数</param>
        /// <returns></returns>
        public static Mat Mid(Mat img, int size, int times)
        {
            Mat dst = new Mat(img.Size(), img.Type());
            Cv2.MedianBlur(img, dst, size);
            for (int i = 1; i < times; i++)
                Cv2.MedianBlur(dst, dst, size);
            return dst;
        }

        /// <summary>
        /// Sobel
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <returns>新图像矩阵</returns>
        public static Mat Sobel(Mat img)
        {
            Cv2.GaussianBlur(img, img, new Size(3, 3), 0, 0, BorderTypes.Default);
            Mat grad_x = new Mat();
            Cv2.Sobel(img, grad_x, MatType.CV_16S, 1, 0, 3);
            Cv2.ConvertScaleAbs(grad_x, grad_x);
            Mat grad_y = new Mat();
            Cv2.Sobel(img, grad_y, MatType.CV_16S, 0, 1, 3);
            Cv2.ConvertScaleAbs(grad_y, grad_y);
            Mat dst = new Mat(grad_x.Size(),grad_x.Type());
            int width = grad_x.Cols;
            int height = grad_x.Rows;
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    int xg = grad_x.At<byte>(x, y);
                    int yg = grad_y.At<byte>(x, y);
                    double v1 = Math.Pow(xg, 2);
                    double v2 = Math.Pow(yg, 2);
                    int val = (int)Math.Sqrt(v1 + v2);
                    if (val > 255) //确保像素值在 0 -- 255 之间
                        val = 255;
                    if (val < 0)
                        val = 0;
                    byte xy = (byte)val;
                    dst.Set<byte>(x, y, xy);
                    //Console.WriteLine(x + " " + y + " " + xy);
                }
            }
            return dst;
        }

        /// <summary>
        /// Roberts
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <returns>新图像矩阵</returns>
        public static Mat Roberts(Mat img)
        {
            Mat dst = img.Clone();
            int nRows = dst.Rows;
            int nCols = dst.Cols;
            for (int i = 0; i < nRows - 1; i++)
            {
                for (int j = 0; j < nCols - 1; j++)
                {
                    int t1 = (img.At<byte>(i, j) -
                        img.At<byte>(i + 1, j + 1)) *
                        (img.At<byte>(i, j) -
                        img.At<byte>(i + 1, j + 1));
                    int t2 = (img.At<byte>(i + 1, j) -
                        img.At<byte>(i, j + 1)) *
                        (img.At<byte>(i + 1, j) -
                        img.At<byte>(i, j + 1));
                    dst.Set(i, j, (byte)Math.Sqrt(t1 + t2));
                }
            }
            return dst;
        }
        
        /// <summary>
        /// Canny
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="size">内核大小</param>
        /// <param name="times">迭代次数</param>
        /// <returns></returns>
        public static Mat Canny(Mat img,int size,int times)
        {
            Mat dst = new Mat(img.Size(), img.Type());
            Cv2.Canny(img, dst, 3, 9, size);
            for (int i = 1; i < times; i++)
                Cv2.Canny(dst, dst, 3, 9, size);
            return dst;
        }

        /// <summary>
        /// Laplacian
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="size">内核大小</param>
        /// <param name="times">迭代次数</param>
        /// <returns></returns>
        public static Mat Laplacian(Mat img,int size,int times)
        {
            Mat dst = new Mat();
            Cv2.Laplacian(img, dst, -1, size);
            for(int i=1;i<times;i++)
                Cv2.Laplacian(dst, dst, -1, size);
            Cv2.Threshold(dst, dst, 0, 255, ThresholdTypes.Otsu | ThresholdTypes.Binary);
            Cv2.ConvertScaleAbs(dst, dst);
            return dst;
        }

        /// <summary>
        /// 膨胀
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="type">内核类型（0：2×2正方；1：3×1行；2：1×3列；3：3×3叉；4：5×5圆；5：7×7圆；6：11×11圆）
        /// </param>
        /// <param name="times">迭代次数</param>
        /// <returns>新图像矩阵</returns>
        public static Mat Dilate(Mat img,int type,int times)
        {
            Mat dst = new Mat(img.Size(), img.Type());
            Mat element=new Mat();
            if (type == 0)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(2,2));
            else if(type==1)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 1));
            else if(type==2)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(1, 3));
            else if(type==3)
                element = Cv2.GetStructuringElement(MorphShapes.Cross, new Size(3, 3));
            else if (type == 4)
                element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(5, 5));
            else if (type == 5)
                element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(7, 7));
            else if(type==6)
                element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(11, 11));
            Cv2.Dilate(img, dst, element,null,times);
            return dst;
        }

        /// <summary>
        /// 腐蚀
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="type">内核类型</param>
        /// <param name="times">迭代次数</param>
        /// <returns>新图像矩阵</returns>
        public static Mat Erode(Mat img, int type, int times)
        {
            Mat dst = new Mat(img.Size(), img.Type());
            Mat element = new Mat();
            if (type == 0)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(2, 2));
            else if (type == 1)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 1));
            else if (type == 2)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(1, 3));
            else if (type == 3)
                element = Cv2.GetStructuringElement(MorphShapes.Cross, new Size(3, 3));
            else if (type == 4)
                element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(5, 5));
            else if (type == 5)
                element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(7, 7));
            else if (type == 6)
                element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(11, 11));
            Cv2.Erode(img, dst, element,null,times);
            return dst;
        }

        /// <summary>
        /// 开运算
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="type">内核类型</param>
        /// <param name="times">迭代次数</param>
        /// <returns>新图像矩阵</returns>
        public static Mat Open(Mat img, int type, int times)
        {
            Mat dst = new Mat(img.Size(), img.Type());
            Mat element = new Mat();
            if (type == 0)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(2, 2));
            else if (type == 1)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 1));
            else if (type == 2)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(1, 3));
            else if (type == 3)
                element = Cv2.GetStructuringElement(MorphShapes.Cross, new Size(3, 3));
            else if (type == 4)
                element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(5, 5));
            else if (type == 5)
                element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(7, 7));
            else if (type == 6)
                element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(11, 11));
            Cv2.MorphologyEx(img, dst, MorphTypes.Open,element, null, times);
            return dst;
        }

        /// <summary>
        /// 闭运算
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="type">内核类型</param>
        /// <param name="times">迭代次数</param>
        /// <returns>新图像矩阵</returns>
        public static Mat Close(Mat img, int type, int times)
        {
            Mat dst = new Mat(img.Size(), img.Type());
            Mat element = new Mat();
            if (type == 0)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(2, 2));
            else if (type == 1)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 1));
            else if (type == 2)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(1, 3));
            else if (type == 3)
                element = Cv2.GetStructuringElement(MorphShapes.Cross, new Size(3, 3));
            else if (type == 4)
                element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(5, 5));
            else if (type == 5)
                element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(7, 7));
            else if (type == 6)
                element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(11, 11));
            Cv2.MorphologyEx(img, dst, MorphTypes.Close, element, null, times);
            return dst;
        }

        /// <summary>
        /// （形态学）梯度
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="type">内核类型（0：3×3；1：5×5；2：7×7）</param>
        /// <param name="times">迭代次数</param>
        /// <returns>新图像矩阵</returns>
        public static Mat Grad(Mat img, int type, int times)
        {
            Mat dst = new Mat(img.Size(), img.Type());
            Mat element = new Mat();
            if (type == 0)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));
            else if (type == 1)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(5, 5));
            else if (type == 2)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(7, 7));
            Cv2.MorphologyEx(img, dst, MorphTypes.Gradient, element, null, times);
            return dst;
        }
        
        ///<summary>
        /// 顶帽
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="type">内核类型（0：3×3；1：5×5；2：7×7）</param>
        /// <param name="times">迭代次数</param>
        /// <returns>新图像矩阵</returns>
        public static Mat TopHat(Mat img, int type, int times)
        {
            Mat dst = new Mat(img.Size(), img.Type());
            Mat element = new Mat();
            if (type == 0)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));
            else if (type == 1)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(5, 5));
            else if (type == 2)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(7, 7));
            Cv2.MorphologyEx(img, dst, MorphTypes.TopHat, element, null, times);
            return dst;
        }

        ///<summary>
        /// 黑帽
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="type">内核类型（0：3×3；1：5×5；2：7×7）</param>
        /// <param name="times">迭代次数</param>
        /// <returns>新图像矩阵</returns>
        public static Mat BlackHat(Mat img, int type, int times)
        {
            Mat dst = new Mat(img.Size(), img.Type());
            Mat element = new Mat();
            if (type == 0)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));
            else if (type == 1)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(5, 5));
            else if (type == 2)
                element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(7, 7));
            Cv2.MorphologyEx(img, dst, MorphTypes.BlackHat, element, null, times);
            return dst;
        }

        /// <summary>
        /// 二值化
        /// </summary>
        /// <param name="img">原图像矩阵</param>
        /// <param name="thre">阈值</param>
        /// <returns>新图像矩阵</returns>
        public static Mat ToTwo(Mat img,int thre)
        {
            Mat dst = img.CvtColor(ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(dst, dst, thre, 255, ThresholdTypes.Binary);
            return dst;
        }
    }
}
