using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Dlls
{
    /// <summary>
    /// 旋转
    /// </summary>
    public static class Rotate
    {
        /// <summary>
        /// 顺时针90度
        /// </summary>
        /// <param name="matSrc">原图像矩阵</param>
        /// <returns>新图像矩阵</returns>
        public static Mat Rot90(Mat matSrc)
        {
            Mat matRet = new Mat();
            Cv2.Transpose(matSrc, matRet);
            Cv2.Flip(matRet, matRet, FlipMode.Y);
            return matRet;
        }

        /// <summary>
        /// 顺时针180度
        /// </summary>
        /// <param name="matSrc">原图像矩阵</param>
        /// <returns>新图像矩阵</returns>
        public static Mat Rot180(Mat matSrc)
        {
            Mat matRet=new Mat();
            Cv2.Flip(matSrc, matRet, FlipMode.XY);
            return matRet;
        }

        /// <summary>
        /// 顺时针270度
        /// </summary>
        /// <param name="matSrc">原图像矩阵</param>
        /// <returns>新图像矩阵</returns>
        public static Mat Rot270(Mat matSrc)
        {
            Mat matRet=new Mat();
            Cv2.Transpose(matSrc, matRet);
            Cv2.Flip(matRet, matRet, FlipMode.X);
            return matRet;
        }

        /// <summary>
        /// 任意角度旋转
        /// </summary>
        /// <param name="matSrc">原图像矩阵</param>
        /// <param name="angle">角度</param>
        /// <param name="direction">顺时针或逆时针(true表示逆时针)</param>
        /// <returns>新图像矩阵</returns>
        public static Mat ImgRotate(Mat matSrc, double angle, bool direction)
        {
            if (!direction) angle = -angle;
            int width = matSrc.Cols;
            int height = matSrc.Rows;
            Point2f center;
            center.X = width / (float)2.0;
            center.Y = height / (float)2.0;
            double scale = 1.0;
            Mat trans_mat = Cv2.GetRotationMatrix2D(center, -angle, scale);
            double angle1 = angle * Math.PI / 180.0;
            double a = Math.Sin(angle1) * scale;
            double b = Math.Cos(angle1) * scale;
            double out_width = height * Math.Abs(a) + width * Math.Abs(b);
            double out_height = width * Math.Abs(a) + height * Math.Abs(b);
            double temp1 = trans_mat.At<double>(0, 2);
            temp1+= Math.Round((out_width - width) / 2);
            double temp2 = trans_mat.At<double>(1, 2);
            temp2 += Math.Round((out_height - height) / 2);
            trans_mat.Set<double>(0, 2, temp1);
            trans_mat.Set<double>(1, 2, temp2);
            Mat dst = new Mat();
            Cv2.WarpAffine(matSrc, dst, trans_mat, new Size(out_width, out_height), InterpolationFlags.Linear, BorderTypes.Constant, new Scalar(255, 255, 255));
            return dst;
        }

        /// <summary>
        /// 水平翻转
        /// </summary>
        /// <param name="matSrc">原图像矩阵</param>
        /// <returns>新图像矩阵</returns>
        public static Mat HoriRot(Mat matSrc)
        {
            Mat matRet=new Mat();
            Cv2.Flip(matSrc, matRet, FlipMode.Y);
            return matRet;
        }

        /// <summary>
        /// 垂直翻转
        /// </summary>
        /// <param name="matSrc">原图像矩阵</param>
        /// <returns>新图像矩阵</returns>
        public static Mat VertRot(Mat matSrc)
        {
            Mat matRet=new Mat();
            Cv2.Flip(matSrc, matRet, FlipMode.X);
            return matRet;
        }
    }
}
