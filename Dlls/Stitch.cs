using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Collections;

namespace Dlls
{
    public static class Stitch
    {
        private static int Saturate_cast(double a)
        {
            int b = (int)Math.Round(a);
            if (b > 255) return 255;
            if (b < 0) return 0;
            return b;
        }

        public static Mat[] PreStitching(Mat[] mats, int num)
        {
            Size s = mats[0].Size();
            int w = s.Width;
            int h = s.Height;
            mats[0] = Mood.ImageSize(mats[0], 500, 500 * h / w, 3);   //尺寸归一化
            Mat lab = new Mat(mats[0].Size(), mats[0].Type());
            double[,] x1 = new double[3, 3];
            x1[0, 0] = 1 / Math.Sqrt(3); x1[0, 1] = x1[0, 2] = 0;
            x1[1, 1] = 1 / Math.Sqrt(6); x1[1, 0] = x1[1, 2] = 0;
            x1[2, 2] = 1 / Math.Sqrt(2); x1[2, 0] = x1[2, 1] = 0;
            Mat xs1 = new Mat(3, 3, MatType.CV_64FC1, x1);
            x1[0, 0] = x1[0, 1] = x1[0, 2] = 1;
            x1[1, 0] = x1[1, 1] = 1; x1[1, 2] = -2;
            x1[2, 0] = 1; x1[2, 1] = -1; x1[2, 2] = 0;
            Mat xs2 = new Mat(3, 3, MatType.CV_64FC1, x1);
            xs1 *= xs2;
            //Console.WriteLine(xs1.Rows + "(0)" + xs1.Cols);
            double sum0 = 0;
            int row = mats[0].Rows;
            int col = mats[0].Cols;
            Console.WriteLine(row + " " + col);
            var it = mats[0].GetGenericIndexer<Vec3b>();
            for (int j = 0; j < row; j++)
                for (int k = 0; k < col; k++)
                {
                    Vec3b now = it[j, k];
                    double b = now.Item0; double g = now.Item1; double r = now.Item2;
                    //Console.WriteLine(b + " " + g + " " + r);
                    double[,] x2 = new double[3, 1];
                    x2[0, 0] = Math.Log(0.3811 * r + 0.5783 * g + 0.0402 * b);
                    x2[1, 0] = Math.Log(0.1967 * r + 0.7244 * g + 0.0782 * b);
                    x2[2, 0] = Math.Log(0.0241 * r + 0.1288 * g + 0.8444 * b);
                    Mat rgb = new Mat(3, 1, MatType.CV_64FC1, x2);
                    Mat res1 = xs1 * rgb;
                    var it1 = res1.GetGenericIndexer<double>();
                    double l1 = it1[0, 0];
                    double a1 = it1[1, 0];
                    double b1 = it1[2, 0];
                    //Console.WriteLine(l1 + " " + a1 + " " + b1);
                    Vec3b lab1 = new Vec3b();
                    lab1.Item0 = (byte)Saturate_cast(l1);
                    lab1.Item1 = (byte)Saturate_cast(a1);
                    lab1.Item2 = (byte)Saturate_cast(b1);
                    lab.Set(j, k, lab1);
                    sum0 += lab1.Item0;
                }
            double ave0 = sum0 / (row * col);
            double fc0 = 0;
            for (int j = 0; j < row; j++)
                for (int k = 0; k < col; k++)
                {
                    double ll = lab.At<Vec3d>(j, k).Item0;
                    fc0 += (ave0 - ll) * (ave0 - ll);
                }
            fc0 = fc0 / (row * col);
            Console.WriteLine(ave0 + " " + fc0);
            for(int i=1;i<num;i++)
            {
                s = mats[i].Size();
                w = s.Width;
                h = s.Height;
                mats[i] = Mood.ImageSize(mats[i], 500, 500 * h / w, 3);   //尺寸归一化
                lab = new Mat(mats[i].Size(), mats[i].Type());
                x1 = new double[3, 3];
                x1[0, 0] = 1 / Math.Sqrt(3); x1[0, 1] = x1[0, 2] = 0;
                x1[1, 1] = 1 / Math.Sqrt(6); x1[1, 0] = x1[1, 2] = 0;
                x1[2, 2] = 1 / Math.Sqrt(2); x1[2, 0] = x1[2, 1] = 0;
                xs1 = new Mat(3, 3, MatType.CV_64FC1, x1);
                x1[0, 0] = x1[0, 1] = x1[0, 2] = 1;
                x1[1, 0] = x1[1, 1] = 1; x1[1, 2] = -2;
                x1[2, 0] = 1; x1[2, 1] = -1; x1[2, 2] = 0;
                xs2 = new Mat(3, 3, MatType.CV_64FC1, x1);
                xs1 *= xs2;
                double sum1 = 0;
                row = mats[i].Rows;
                col = mats[i].Cols;
                for (int j = 0; j < row; j++)
                    for (int k = 0; k < col; k++)
                    {
                        Vec3b now = mats[i].At<Vec3b>(j, k);
                        double b = now.Item0; double g = now.Item1; double r = now.Item2;
                        double[,] x2 = new double[3, 1];
                        x2[0, 0] = Math.Log(0.3811 * r + 0.5783 * g + 0.0402 * b);
                        x2[1, 0] = Math.Log(0.1967 * r + 0.7244 * g + 0.0782 * b);
                        x2[2, 0] = Math.Log(0.0241 * r + 0.1288 * g + 0.8444 * b);
                        Mat rgb = new Mat(3, 1, MatType.CV_64FC1, x2);
                        Mat res1 = xs1 * rgb;
                        double l1 = res1.At<double>(0, 0);
                        double a1 = res1.At<double>(1, 0);
                        double b1 = res1.At<double>(2, 0);
                        Vec3b lab1 = new Vec3b();
                        lab1.Item0 = (byte)Saturate_cast(l1);
                        lab1.Item1 = (byte)Saturate_cast(a1);
                        lab1.Item2 = (byte)Saturate_cast(b1);
                        lab.Set(j, k, lab1);
                        sum1 += lab1.Item0;
                    }
                double ave = sum1 / (row * col);
                double fc = 0;
                for (int j = 0; j < row; j++)
                    for (int k = 0; k < col; k++)
                    {
                        double ll = lab.At<Vec3b>(j, k).Item0;
                        fc += (ave - ll) * (ave - ll);
                    }
                fc = fc / (row * col);
                Console.WriteLine(i + ":" + ave + " " + fc);
                for (int j = 0; j < row; j++)
                    for (int k = 0; k < col; k++)
                    {
                        Vec3b lab2 = new Vec3b();
                        lab2.Item0 = (byte)Saturate_cast(fc0 / fc * (lab.At<Vec3b>(j, k)[0] - ave) + ave0);
                        lab2.Item1 = lab.At<Vec3b>(j, k)[1];
                        lab2.Item2 = lab.At<Vec3b>(j, k)[2];
                        lab.Set(j, k, lab2);
                    }
                x1[0, 0] = 1; x1[0, 1] = 1; x1[0, 2] = 1;
                x1[1, 0] = 1; x1[1, 1] = 1; x1[1, 2] = -1;
                x1[2, 0] = 1; x1[2, 1] = -2; x1[2, 2] = 0;
                xs1 = new Mat(3, 3, MatType.CV_64FC1, x1);
                x1[0, 0] = 1 / Math.Sqrt(3); x1[0, 1] = x1[0, 2] = 0;
                x1[1, 1] = 1 / Math.Sqrt(6); x1[1, 0] = x1[1, 2] = 0;
                x1[2, 2] = 1 / Math.Sqrt(2); x1[2, 0] = x1[2, 1] = 0;
                xs2 = new Mat(3, 3, MatType.CV_64FC1, x1);
                xs1 *= xs2;
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                        xs1.Set(j, k, Math.Exp(xs1.At<double>(j, k)));
                x1[0, 0] = 4.4679; x1[0, 1] = -3.5873; x1[0, 2] = 0.1193;
                x1[1, 0] = -1.2186; x1[1, 1] = 2.3809; x1[1, 2] = -0.1624;
                x1[2, 0] = 0.0497; x1[2, 1] = -0.2439; x1[2, 2] = 1.2045;
                xs2 = new Mat(3, 3, MatType.CV_64FC1, x1);
                Mat xs3 = new Mat(3, 1, MatType.CV_64FC1);
                for (int j = 0; j < row; j++)
                    for (int k = 0; k < col; k++)
                    {
                        Vec3b lab3 = lab.At<Vec3b>(j, k);
                        double l3 = lab3.Item0;
                        double a3 = lab3.Item1;
                        double b3 = lab3.Item2;
                        xs3.Set(0, 0, l3);
                        xs3.Set(1, 0, a3);
                        xs3.Set(2, 0, b3);
                        xs3 = xs1 * xs2 * xs3;
                        Vec3b r = new Vec3b();
                        r.Item0 = (byte)Saturate_cast(xs3.At<double>(0, 0));
                        r.Item1 = (byte)Saturate_cast(xs3.At<double>(1, 0));
                        r.Item2 = (byte)Saturate_cast(xs3.At<double>(2, 0));
                        mats[i].Set(i, j, r);
                    }
            }
            return mats;
        }

        private static Point2d Point2fToPoint2d(Point2f input)
        {
            Point2d p2 = new Point2d(input.X, input.Y);
            return p2;
        }
        
        private static double CalcK(Point2f a,Point2f b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return dy / dx;
        }

        //左上/左下/右上/右下
        private static Point2d[] CalcCorners(Mat H,Mat src)
        {
            Point2d[] corners = new Point2d[4];
            double[] v2 = new double[3] { 0, 0, 1 };    //左上角
            double[] v1 = new double[3];
            Mat V2 = new Mat(3, 1,MatType.CV_64FC1,v2);
            Mat V1 = new Mat(3, 1, MatType.CV_64FC1, v1);
            Console.WriteLine("H:" + H.Size());

            V1 = H * V2;
            Console.WriteLine("V2:" + V2.At<double>(0,0)+" "+ V2.At<double>(1, 0)+" "+ V2.At<double>(2, 0));
            Console.WriteLine("V1:" + V1.At<double>(0, 0) + " "+ V1.At<double>(1, 0)+" "+ V1.At<double>(2, 0));
            Console.WriteLine("v1:" + v1[0] + " " + v1[1] + " " + v1[2]);
            corners[0].X = V1.At<double>(0, 0) / V1.At<double>(2, 0);
            corners[0].Y = V1.At<double>(1, 0) / V1.At<double>(2, 0);

            v2[0] = 0;v2[1] = src.Rows;v2[2] = 1;
            V2 = new Mat(3, 1, MatType.CV_64FC1, v2);
            V1 = new Mat(3, 1, MatType.CV_64FC1, v1);
            V1 = H * V2;
            Console.WriteLine("V2:" + V2.At<double>(0, 0) + " " + V2.At<double>(1, 0) + " " + V2.At<double>(2, 0));
            Console.WriteLine("V1:" + V1.At<double>(0, 0) + " " + V1.At<double>(1, 0) + " " + V1.At<double>(2, 0));
            corners[1].X = V1.At<double>(0, 0) / V1.At<double>(2, 0);
            corners[1].Y = V1.At<double>(1, 0) / V1.At<double>(2, 0);

            v2[0] = src.Cols; v2[1] = 0; v2[2] = 1;
            V2 = new Mat(3, 1, MatType.CV_64FC1, v2);
            V1 = new Mat(3, 1, MatType.CV_64FC1, v1);
            V1 = H * V2;
            Console.WriteLine("V2:" + V2.At<double>(0, 0) + " " + V2.At<double>(1, 0) + " " + V2.At<double>(2, 0));
            Console.WriteLine("V1:" + V1.At<double>(0, 0) + " " + V1.At<double>(1, 0) + " " + V1.At<double>(2, 0));
            corners[2].X = V1.At<double>(0, 0) / V1.At<double>(2, 0);
            corners[2].Y = V1.At<double>(1, 0) / V1.At<double>(2, 0);

            v2[0] = src.Cols; v2[1] = src.Rows; v2[2] = 1;
            V2 = new Mat(3, 1, MatType.CV_64FC1, v2);
            V1 = new Mat(3, 1, MatType.CV_64FC1, v1);
            V1 = H * V2;
            Console.WriteLine("V2:" + V2.At<double>(0, 0) + " " + V2.At<double>(1, 0) + " " + V2.At<double>(2, 0));
            Console.WriteLine("V1:" + V1.At<double>(0, 0) + " " + V1.At<double>(1, 0) + " " + V1.At<double>(2, 0));
            corners[3].X = V1.At<double>(0, 0) / V1.At<double>(2, 0);
            corners[3].Y = V1.At<double>(1, 0) / V1.At<double>(2, 0);
            return corners;
        }

        private static Mat PreStitch2(Mat mat1,Mat mat2,Mat img1,Mat img2)
        {
            //Shi-Tomasi角点检测
            /*int n = 0;
            Point2f[] cornersPoint = Cv2.GoodFeaturesToTrack(mat1, 1000, 0.01, 10, new Mat(), 3, false, 0.04);
            foreach (var item in cornersPoint)
            {
                Cv2.Circle(mat1, Convert.ToInt16(item.X), Convert.ToInt16(item.Y), 2, Scalar.White, 2);
                Console.WriteLine(item);
                n++;
            }
            Cv2.ImWrite("C:\\Users\\予感\\Desktop\\shi.bmp", mat1);
            Cv2.ImShow("mat1", mat1);
            Console.WriteLine("n="+n);*/
            /*
            //亚像素级角点检测
            int n = 0;
            Size winSize = new Size(21, 21);   // 搜索矩形大小的一半，类似于渲染的小方格
            Size zeroZone = new Size(-1, -1);    //这里是死区的一半尺寸，（-1，-1表示没有死区）
            List<Point2f> inputCorners = new List<Point2f>();  //这里是输入角的初始坐标和提供的细化坐标（类似扫描）
            for (int k = 50; k < mat1.Rows - 60; k += 40)   //给inputCorners 矩阵赋值
            {
                for (int j = 50; j < mat1.Cols - 50; j += 40)
                {
                    inputCorners.Add(new Point(j, k));
                }
            }
            TermCriteria criteria = new TermCriteria(CriteriaType.Eps, 100, 0.01);   //这个是检测数据
            //这里开始检测角点
            Point2f[] y_cornersPoint = Cv2.CornerSubPix(mat1, inputCorners, winSize, zeroZone, criteria);
            //遍历画出角点
            foreach (var item in y_cornersPoint)
            {
                Cv2.Circle(mat1, Convert.ToInt16(item.X), Convert.ToInt16(item.Y), 2, Scalar.White, 2);
                Console.WriteLine(item);
                    n++;
            }
            Cv2.ImWrite("C:\\Users\\予感\\Desktop\\亚像素.bmp", mat1);
            Cv2.ImShow("mat1", mat1);
            Console.WriteLine("n="+n);
            */
            //FAST角点检测
            /*int n = 0;
            KeyPoint[] keyPoint = Cv2.FAST(mat1, 150);
            foreach (var item in keyPoint)
            {
                Cv2.Circle(mat1, Convert.ToInt16(item.Pt.X), Convert.ToInt16(item.Pt.Y), 2, Scalar.White, 2);
                Console.WriteLine(item);
                n++;
            }
            Cv2.ImWrite("C:\\Users\\予感\\Desktop\\FAST.bmp", mat1);
            Cv2.ImShow("mat1", mat1);
            Console.WriteLine("n=" + n);*/
            //surf特征点检测
            OpenCvSharp.XFeatures2D.SURF surf = OpenCvSharp.XFeatures2D.SURF.Create(400, 4, 3, true);
            KeyPoint[] keypoints1,keypoints2;
            Mat temp1 = new Mat();
            Mat temp2 = new Mat();
            surf.DetectAndCompute(mat1, null, out keypoints1, temp1);
            surf.DetectAndCompute(mat2, null, out keypoints2, temp2);
            //Cv2.DrawKeypoints(mat1, keypoints1, mat1);
            //Cv2.DrawKeypoints(mat2, keypoints2, mat2);
            //Cv2.ImShow("a", mat1);
            //Cv2.WaitKey(0);
            //Cv2.ImShow("b", mat2);
            //Cv2.WaitKey(0);
            var flnMatcher = new FlannBasedMatcher();
            var matches = flnMatcher.Match(temp1, temp2);
            //求最小最大距离
            double minDistance = 1000;//反向逼近
            double maxDistance = 0;
            for (int i = 0; i < temp1.Rows; i++)
            {
                double distance = matches[i].Distance;
                if (distance > maxDistance)
                    maxDistance = distance;
                if (distance < minDistance)
                    minDistance = distance;
            }
            Console.WriteLine($"max distance : {maxDistance}");
            Console.WriteLine($"min distance : {minDistance}");

            var pointsSrc = new List<Point2f>();
            var pointsDst = new List<Point2f>();
            //筛选较好的匹配点
            var goodMatches = new List<DMatch>();
            goodMatches.Clear();
            for (int i = 0; i < temp1.Rows; i++)
            {
                double distance = matches[i].Distance;
                if (distance <= 3*minDistance)
                {
                    pointsSrc.Add(keypoints1[matches[i].QueryIdx].Pt);
                    pointsDst.Add(keypoints2[matches[i].TrainIdx].Pt);
                    //距离小于范围的压入新的DMatch
                    goodMatches.Add(matches[i]);
                }
            }

            var outMat = new Mat();

            // 算法RANSAC对匹配的结果做过滤
            var pSrc = pointsSrc.ConvertAll(Point2fToPoint2d);
            var pDst = pointsDst.ConvertAll(Point2fToPoint2d);
            var outMask = new Mat();
            var imagePoints1 = new List<Point2f>();
            var imagePoints2 = new List<Point2f>();
            var calck = new List<double>();
            double sum = 0, ave = 0;
            // 如果原始的匹配结果为空, 则跳过过滤步骤
            if (pSrc.Count > 0 && pDst.Count > 0)
                Cv2.FindHomography(pSrc, pDst, HomographyMethods.Ransac, mask: outMask);
            // 如果通过RANSAC处理后的匹配点大于10个,才应用过滤. 否则使用原始的匹配点结果(匹配点过少的时候通过RANSAC处理后,可能会得到0个匹配点的结果).
            if (outMask.Rows > 10)
            {
                byte[] maskBytes = new byte[outMask.Rows * outMask.Cols];
                outMask.GetArray(0, 0, maskBytes);
                for (int i = 0; i < outMask.Rows * outMask.Cols; i++)
                {

                    if (maskBytes[i] == 1)
                    {
                        imagePoints1.Add(pointsSrc[i]);
                        imagePoints2.Add(pointsDst[i]);
                        double k = CalcK(pointsSrc[i], pointsDst[i]);
                        //Console.WriteLine(i + ":" + imagePoints1[i] + " " + imagePoints2[i] + "k=" + k);
                        sum += k;
                        calck.Add(k);
                    }
                    Console.Write(maskBytes[i]);
                }
                Console.Write("\n");
                Mat ooutmat = new Mat();
                Cv2.DrawMatches(mat1, keypoints1, mat2, keypoints2, goodMatches, ooutmat, null, flags: DrawMatchesFlags.NotDrawSinglePoints);
                //Cv2.ImShow("ooutMat", ooutmat);
                //Cv2.ImWrite("C:\\Users\\予感\\Desktop\\out1.bmp", ooutmat);
                //Cv2.WaitKey(0);
                Cv2.DrawMatches(mat1, keypoints1, mat2, keypoints2, goodMatches, outMat, matchesMask: maskBytes, flags: DrawMatchesFlags.NotDrawSinglePoints);
            }
            else
            {
                Cv2.DrawMatches(mat1, keypoints1, mat2, keypoints2, goodMatches, outMat, flags: DrawMatchesFlags.NotDrawSinglePoints);
                for (int i = 0; i < goodMatches.Count; i++)
                {
                    imagePoints1.Add(keypoints1[goodMatches[i].QueryIdx].Pt);
                    imagePoints2.Add(keypoints2[goodMatches[i].TrainIdx].Pt);
                    double k = CalcK(imagePoints1[i], imagePoints2[i]);
                    Console.WriteLine(i + ":" + imagePoints1[i] + " " + imagePoints2[i] + "k=" + k);
                    sum += k;
                    calck.Add(k);
                }
            }
            //Cv2.ImShow("outMat", outMat);
            //Cv2.ImWrite("C:\\Users\\予感\\Desktop\\out2.bmp", outMat);
            //Cv2.WaitKey(0);
            Console.WriteLine("outmask=" + outMask.Rows);
            Console.WriteLine("img1count=" + imagePoints1.Count+" img2count="+imagePoints2.Count);
            ave = sum / imagePoints1.Count;
            int nn = 0; 
            for(int i=0;i<imagePoints1.Count;i++)
            {
                if (Math.Abs(calck[i] - ave) > 10)
                    nn++;
            }
            Console.WriteLine("ave=" + ave+ " nn="+nn+" total="+goodMatches.Count);
            if (nn>2||imagePoints1.Count<5) return null;
            var imagePoints01 = imagePoints1.ConvertAll(Point2fToPoint2d);
            var imagePoints02 = imagePoints2.ConvertAll(Point2fToPoint2d);
            Mat homo = Cv2.FindHomography(imagePoints02, imagePoints01, HomographyMethods.Ransac);
            Console.WriteLine("变换矩阵为\n" + homo);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    Console.Write(homo.At<double>(i, j) + " ");
                Console.Write("\n");
            }
            //图像对准
            Point2d[] corners=CalcCorners(homo, img2);
            Console.WriteLine("左上角:" + corners[0].X+" "+ corners[0].Y);
            Console.WriteLine("左下角:" + corners[1].X + " " + corners[1].Y);
            Console.WriteLine("右上角:" + corners[2].X + " " + corners[2].Y);
            Console.WriteLine("右下角:" + corners[3].X + " " + corners[3].Y);
            Mat result=new Mat();
            //Mat resultback=new Mat(); //保存的是新帧经过单应矩阵变换以后的图像
            Cv2.WarpPerspective(img2, result, homo, new Size(Math.Max(corners[2].X, Math.Max(corners[3].X,img1.Cols)), Math.Max(img1.Rows, Math.Max(corners[1].Y,corners[3].Y))/*mat1.Cols+mat2.Cols,mat1.Rows+mat2.Rows*/));
            //Cv2.ImShow("warp", result);
            //Cv2.ImWrite("C:\\Users\\予感\\Desktop\\warp.bmp", result);
            //Cv2.WaitKey(0);
            Mat need = result.Clone();
            //result.CopyTo(resultback);
            Mat half=new Mat(result, new Rect(0,0,mat1.Cols,mat1.Rows));
            for (int i = 0; i < img1.Rows; i++)
                for (int j = 0; j < img1.Cols; j++)
                    if (img1.At<Vec3b>(i, j)[0] <50 && img1.At<Vec3b>(i, j)[1] <50 && img1.At<Vec3b>(i, j)[2] <50)
                    {
                        /*if (need.At<Vec3b>(i, j)[0] == 0 && need.At<Vec3b>(i, j)[1] == 0 && need.At<Vec3b>(i, j)[2] == 0)
                        {
                            int a = i - 1;
                            for (int k = i - 1; k >= 0 && a != i - 1; k--)
                                if (need.At<Vec3b>(k, j)[0] != 0 || need.At<Vec3b>(k, j)[1] != 0 || need.At<Vec3b>(k, j)[2] != 0)
                                    a = k;
                            img1.Set<Vec3b>(i, j, need.At<Vec3b>(a, j));
                            Console.WriteLine(">>>"+i+" "+a);
                        }
                        else */
                            img1.Set<Vec3b>(i, j, need.At<Vec3b>(i, j));
                    }
            //Cv2.ImShow("img1", img1);
            img1.CopyTo(half);
            //Cv2.ImShow("result", result);
            //Cv2.ImWrite("C:\\Users\\予感\\Desktop\\res.bmp", result);
            //Cv2.WaitKey(0);
            /*Mat result = new Mat();
            Mat resultback = new Mat();
            var srcTri = new List<Point2f>();
            var dstTri = new List<Point2f>();
            for(int i=0;i<3;i++)
            {
                srcTri.Add(imagePoints2[i]);
                dstTri.Add(imagePoints1[i]);
            }
            Mat wrap_mat = Cv2.GetAffineTransform(imagePoints2, imagePoints1);
            Cv2.WarpAffine(img2, result, wrap_mat, new Size(1000,1000));
            Cv2.ImShow("warp", result);
            Cv2.WaitKey(0);
            result.CopyTo(resultback);
            Mat half = new Mat(result, new Rect(0, 0, img1.Cols, img1.Rows));
            img1.CopyTo(half);
            Cv2.ImShow("result", result);
            Cv2.WaitKey(0);*/
            /*
            //渐入渐出融合
            Mat result_linerblend = result.Clone();
            double dblend = 0.0;
            int ioffset = mat2.Cols - 100;
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < result_linerblend.Rows; j++)
                {
                    byte temp= (byte)Saturate_cast(result.At<byte>(j,ioffset + i) * (1 - dblend) + resultback.At<byte>(j,ioffset + i) * dblend);
                    result_linerblend.Set<byte>(j, ioffset + i, temp);
                }
                dblend = dblend + 0.01;
            }
            Cv2.ImShow("result_linerblend", result_linerblend);
            */

            //最大值法融合
            /*Mat result_maxvalue = result.Clone();
            int ioffset = mat2.Cols - 100;
            for (int i = 0; i < mat2.Rows; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    int iresult = result.At<Vec3b>(i, ioffset + j).Item0 + result.At<Vec3b>(i, ioffset + j).Item1 + result.At<Vec3b>(i, ioffset + j).Item2;
                    int iresultback = resultback.At<Vec3b>(i, ioffset + j).Item0 + resultback.At<Vec3b>(i, ioffset + j).Item1 + resultback.At<Vec3b>(i, ioffset + j).Item2;
                    if (iresultback > iresult)
                    {
                        result_maxvalue.Set<Vec3b>(i, ioffset + j, resultback.At<Vec3b>(i, ioffset + j));
                    }
                }
            }
            //Cv2.ImShow("result_maxvalue", result_maxvalue);*/

            int rs = (int)Math.Max(0, Math.Max(corners[0].Y,corners[2].Y));
            int start = (int)Math.Min(Math.Max(corners[0].X,0), Math.Max(0,corners[1].X));
            int col = (int)Math.Min(img1.Cols,Math.Min(corners[2].X,corners[3].X));
            double overlapwidth = col - start;
            int row = Math.Min(img1.Rows,Math.Min(need.Rows,result.Rows));
            double alpha = 1;
            Console.WriteLine("result:" + result.Size());
            Console.WriteLine("need:" + need.Size());
            Console.WriteLine("img1:" + img1.Size());
            Console.WriteLine("row:" +row+" start:"+start+" col:"+col);
            /*Mat mask = new Mat(result.Size(), mat1.Type(), Scalar.Black);
            Cv2.Rectangle(mask, new Rect(start, 0, col - start, row),Scalar.White,-1);
            Cv2.ImShow("mask", mask);
            Cv2.WaitKey(0);
            Point center = new Point(row / 2, (start + col) / 2);
            Console.WriteLine(center.X + " " + center.Y);
            Cv2.SeamlessClone(need, img1, mask, center, result, SeamlessCloneMethods.MixedClone);*/
            Mat test = result.Clone();
            Cv2.Rectangle(test, new Rect(start, rs, col - start, row-rs), Scalar.Black, 3);
            //Cv2.ImWrite("C:\\Users\\予感\\Desktop\\test.bmp", test);
            //Cv2.ImShow("test", test);
            //Cv2.WaitKey(0);
            Console.WriteLine("(0," + row + ")" + "(" + start + "," + col + ")");
            for (int i = rs; i < row; i++)
            {
                //Console.WriteLine("(" + i +")");
                for (int j = start; j < col; j++)
                {
                    Vec3b rtemp = result.At<Vec3b>(i, j);
                    Vec3b ntemp = need.At<Vec3b>(i, j);
                    Vec3b itemp = img1.At<Vec3b>(i, j);
                    if (ntemp[0] < 150 && ntemp[1] < 150 && ntemp[2] < 150)
                        alpha = 1;
                    else if (itemp[0] < 150 && itemp[1] < 150 && itemp[2] < 150)
                        alpha = 0;
                    else
                        alpha = (overlapwidth - (j - start)) / overlapwidth;
                    Vec3b a;
                    a.Item0 = (byte)(itemp[0] * alpha + ntemp[0] * (1 - alpha));
                    a.Item1 = (byte)(itemp[1] * alpha + ntemp[1] * (1 - alpha));
                    a.Item2 = (byte)(itemp[2] * alpha + ntemp[2] * (1 - alpha));
                    result.Set<Vec3b>(i, j, a);
                }
            }
            //Cv2.ImWrite("C:\\Users\\予感\\Desktop\\res1.bmp", result);
            /*
            //带阈值的加权平滑处理
            Mat result_advance = result.Clone();
            for (int i = 0; i < mat2.Rows; i++)
            {
                for (int j = 0; j < 33; j++)
                {
                    int iimg1 = result.At<Vec3b>(i, ioffset + j).Item0 + result.At<Vec3b>(i, ioffset + j).Item1 + result.At<Vec3b>(i, ioffset + j).Item2;
                    //int iimg2= resultback.at<Vec3b>(i,ioffset+j)[0]+ resultback.at<Vec3b>(i,ioffset+j)[1]+ resultback.at<Vec3b>(i,ioffset+j)[2];
                    int ilinerblend = result_linerblend.At<Vec3b>(i, ioffset + j).Item0 + result_linerblend.At<Vec3b>(i, ioffset + j).Item1 + result_linerblend.At<Vec3b>(i, ioffset + j).Item2;
                    if (Math.Abs(iimg1 - ilinerblend) < 3)
                    {
                        result_advance.Set<Vec3b>(i, ioffset + j,result_linerblend.At<Vec3b>(i, ioffset + j));
                    }
                }
            }
            for (int i = 0; i < mat2.Rows; i++)
            {
                for (int j = 33; j < 66; j++)
                {
                    int iimg1 = result.At<Vec3b>(i, ioffset + j).Item0 + result.At<Vec3b>(i, ioffset + j).Item1 + result.At<Vec3b>(i, ioffset + j).Item2;
                    int iimg2 = resultback.At<Vec3b>(i, ioffset + j)[0] + resultback.At<Vec3b>(i, ioffset + j)[1] + resultback.At<Vec3b>(i, ioffset + j)[2];
                    int ilinerblend = result_linerblend.At<Vec3b>(i, ioffset + j)[0] + result_linerblend.At<Vec3b>(i, ioffset + j)[1] + result_linerblend.At<Vec3b>(i, ioffset + j)[2];
                    if (Math.Abs(Math.Max(iimg1, iimg2) - ilinerblend) < 3)
                    {
                        result_advance.Set<Vec3b>(i, ioffset + j,result_linerblend.At<Vec3b>(i, ioffset + j));
                    }
                    else if (iimg2 > iimg1)
                    {
                        result_advance.Set<Vec3b>(i, ioffset + j,resultback.At<Vec3b>(i, ioffset + j));
                    }
                }
            }
            for (int i = 0; i < mat2.Rows; i++)
            {
                for (int j = 66; j < 100; j++)
                {
                    //int iimg1= result.at<Vec3b>(i,ioffset+j)[0]+ result.at<Vec3b>(i,ioffset+j)[1]+ result.at<Vec3b>(i,ioffset+j)[2];
                    int iimg2 = resultback.At<Vec3b>(i, ioffset + j)[0] + resultback.At<Vec3b>(i, ioffset + j)[1] + resultback.At<Vec3b>(i, ioffset + j)[2];
                    int ilinerblend = result_linerblend.At<Vec3b>(i, ioffset + j)[0] + result_linerblend.At<Vec3b>(i, ioffset + j)[1] + result_linerblend.At<Vec3b>(i, ioffset + j)[2];
                    if (Math.Abs(iimg2 - ilinerblend) < 3)
                    {
                        result_advance.Set<Vec3b>(i, ioffset + j,result_linerblend.At<Vec3b>(i, ioffset + j));
                    }
                    else
                    {
                        result_advance.Set<Vec3b>(i, ioffset + j,resultback.At<Vec3b>(i, ioffset + j));
                    }
                }
            }
            Cv2.ImShow("result_advance", result_advance);
            */
            //Cv2.ImShow(">>>", result);
            return result;
        }

        /// <summary>
        /// 图像拼接
        /// </summary>
        /// <param name="mats">图像数组</param>
        /// <param name="num">图像数量</param>
        /// <returns>拼接后的图像</returns>
        public static Mat Stitching(Mat[] mats,int num)
        {
            Mat[] m = new Mat[num];
            Mat dst = mats[0].Clone();
            Mat d = dst.Clone();
            Console.WriteLine("!!!");
            for (int i = 1; i < num; i++)
            {
                Mat fu = mats[i].Clone();
                Mat fudst = dst.Clone();
                m[i] = mats[i].Clone();
                Cv2.CvtColor(dst, d, ColorConversionCodes.BGR2GRAY);  //灰度归一化
                Cv2.CvtColor(mats[i], m[i], ColorConversionCodes.BGR2GRAY);  //灰度归一化
                /*
                                                                             *for (int j=0;j<mats[i].Rows;j++)
                                                                              for(int k=0;k<mats[i].Cols;k++)
                                                                              {
                                                                                  Vec3b temp = mats[i].At<Vec3b>(j, k);
                                                                                  byte r =temp.Item0;
                                                                                  byte g =temp.Item1;
                                                                                  byte b =temp.Item2;
                                                                                  byte MAX = Math.Max(Math.Max(r, g), b);
                                                                                  mats[i].Set<byte>(i,j,MAX);
                                                                              }
                                                                              */
                                                                             //Cv2.ImShow("灰度化", m[i]);
                                                                             //Cv2.WaitKey(0);
                                                                             //Cv2.MedianBlur(mats[i], mats[i], 3); //中值滤波
                                                                             //Cv2.Blur(mats[i], mats[i], new Size(3,3));  //均值滤波                
                                                                             //Cv2.ImShow("d", d);
                                                                             //Cv2.ImShow("m[i]", m[i]);
                //Cv2.ImShow("dd", dst);
                //Cv2.ImShow("mats[i]", mats[i]);
                //Cv2.WaitKey(0);
                Cv2.GaussianBlur(d, d, new Size(3, 3), Math.Sqrt(2), Math.Sqrt(2)); //高斯滤波
                Cv2.GaussianBlur(m[i], m[i], new Size(3, 3), Math.Sqrt(2), Math.Sqrt(2)); //高斯滤波
                //Cv2.Laplacian(mats[i], mats[i], mats[i].Type(), 3); //Laplace边缘检测
                //Cv2.Canny(d, d, 1, 2, 3);
                //Cv2.Canny(m[i], m[i], 1, 2, 3);   //Canny，边缘太多
                d = Process.Sobel(d); //Sobel
                m[i] = Process.Sobel(m[i]); //Sobel                
                //d = Process.Roberts(d);
                //m[i] = Process.Roberts(m[i]);   //Roberts
                //Cv2.ImWrite("C:\\Users\\予感\\Desktop\\0Canny.bmp",d);
                //Cv2.ImWrite("C:\\Users\\予感\\Desktop\\1Canny.bmp", m[i]);
                //d = Process.Laplacian(d,3,1);
                //m[i] = Process.Laplacian(m[i],3,1);   //拉普拉斯
                dst = PreStitch2(d, m[i], dst, mats[i]);
                //dst = PreStitch2(m[i], d, mats[i], dst);
                if (dst == null)
                {
                    for (int j = i + 1; j < num; j++)
                        mats[j - 1] = mats[j];
                    mats[num - 1] = fu;
                    //Cv2.ImShow("fu", fu);
                    //Cv2.ImShow("matsi", mats[i]);
                    //Cv2.ImShow("matsi+1", mats[i+1]);
                    //Cv2.WaitKey(0);
                    dst = fudst;
                    i--;
                    continue;
                }
                Console.WriteLine(dst.Cols + " " + dst.Rows);
                //Cv2.ImShow("dst" + i, dst);
                //Cv2.WaitKey(0);
                int xl=0, xr=0, yl=0, yr=0;
                bool flag = false;
                int sum = 0,sumxl=0,sumxr=0,sumyl=0,sumyr=0;
                flag = false;
                for(int k=0;k<dst.Rows&&!flag;k++) //遍历行
                {
                    sum = 0;
                    for(int j=0;j<dst.Cols&&!flag;j++)
                    {
                        if (dst.At<Vec3b>(k, j)[0] != 0 || dst.At<Vec3b>(k, j)[1] != 0 || dst.At<Vec3b>(k, j)[2] != 0)
                            sum++;  //非黑色
                        if(sum>0)
                        {
                            flag = true;
                            yl = k;
                            sumyl = sum;
                        }
                    }
                }
                flag = false;
                for (int k = dst.Rows-1; k >=0 && !flag; k--) //遍历行
                {
                    sum = 0;
                    for (int j = 0; j < dst.Cols && !flag; j++)
                    {
                        if (dst.At<Vec3b>(k, j)[0] != 0 || dst.At<Vec3b>(k, j)[1] != 0 || dst.At<Vec3b>(k, j)[2] != 0)
                            sum++;  //非黑色
                        if (sum > 0)
                        {
                            flag = true;
                            yr = k;
                            //sumyr = sum;
                        }
                    }
                }
                sum = 0;
                for(int j=0;j<dst.Cols;j++)
                    if (dst.At<Vec3b>(yr-50, j)[0] != 0 || dst.At<Vec3b>(yr-50, j)[1] != 0 || dst.At<Vec3b>(yr-50, j)[2] != 0)
                        sum++;  //非黑色
                sumyr = sum;
                flag = false;
                for (int k = 0; k < dst.Cols && !flag; k++) //遍历列
                {
                    sum = 0;
                    for (int j = 0; j < dst.Rows && !flag; j++)
                    {
                        if (dst.At<Vec3b>(j, k)[0] != 0 || dst.At<Vec3b>(j, k)[1] != 0 || dst.At<Vec3b>(j, k)[2] != 0)
                            sum++;
                        if(sum>0)
                        {
                            flag = true;
                            xl = k;
                            sumxl = sum;
                        }
                    }
                }
                flag = false;
                for (int k = dst.Cols-1; k >=0 && !flag; k--) //遍历列
                {
                    sum = 0;
                    for (int j = 0; j < dst.Rows && !flag; j++)
                    {
                        if (dst.At<Vec3b>(j, k)[0] != 0 || dst.At<Vec3b>(j, k)[1] != 0 || dst.At<Vec3b>(j, k)[2] != 0)
                            sum++;
                        if (sum > 0)
                        {
                            flag = true;
                            xr = k;
                            //sumxr = sum;
                        }
                    }
                }
                sum = 0;
                for (int j = 0; j < dst.Rows; j++)
                    if (dst.At<Vec3b>(j, xr-50)[0] != 0 || dst.At<Vec3b>(j, xr - 50)[1] != 0 || dst.At<Vec3b>(j, xr - 50)[2] != 0)
                        sum++;  //非黑色
                sumxr = sum;
                Console.WriteLine(xl + " " + xr + " " + yl + " " + yr);
                flag = false;
                for (int k = yr ; k >= 0 && !flag; k--) //遍历行
                {
                    sum = 0;
                    for (int j = 0; j < dst.Cols && !flag; j++)
                    {
                        if (dst.At<Vec3b>(k, j)[0] != 0 || dst.At<Vec3b>(k, j)[1] != 0 || dst.At<Vec3b>(k, j)[2] != 0)
                            sum++;  //非黑色
                        if (Math.Abs(sum - sumyr) < 20)
                        {
                            flag = true;
                            yr = k;
                        }
                    }
                }
                flag = false;
                for (int k = xr ; k >= 0 && !flag; k--) //遍历列
                {
                    sum = 0;
                    for (int j = 0; j < dst.Rows && !flag; j++)
                    {
                        if (dst.At<Vec3b>(j, k)[0] != 0 || dst.At<Vec3b>(j, k)[1] != 0 || dst.At<Vec3b>(j, k)[2] != 0)
                            sum++;
                        if (Math.Abs(sum-sumxr)<20)
                        {
                            flag = true;
                            xr = k;
                        }
                    }
                }
                dst = new Mat(dst, new Range(0,yr),new Range(0,xr));
                //Cv2.ImShow("dst" + i, dst);
                //Cv2.ImWrite("C:\\Users\\予感\\Desktop\\dst.bmp", dst);
                //Cv2.WaitKey(0);
            }
            return dst;
        }

    }
}
 
 