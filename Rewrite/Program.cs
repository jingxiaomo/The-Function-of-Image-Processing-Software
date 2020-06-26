using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using Dlls;

namespace Rewrite
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Console.WriteLine("hello");
            Mat a=Open.OpenImage("C:\\Users\\予感\\Desktop\\a.jpg");
            if (a.Empty()) Console.WriteLine("!!!");
            Cv2.ImShow("a", a);
            Cv2.WaitKey(0);
            //a.SaveImage("C:\\Users\\予感\\Desktop\\a.jpg", b);
            //a.OpenVideo("C:\\Users\\予感\\Desktop\\1.flv");
            Console.WriteLine("hhh");*/
            /*Rotate c = new Rotate();
            Mat d=c.Rot90(b);
            Cv2.ImShow("rot90",d);
            Cv2.WaitKey(0);
            d = c.Rot180(b);
            Cv2.ImShow("rot180", d);
            Cv2.WaitKey(0);
            d = c.Rot270(b);
            Cv2.ImShow("rot270", d);
            Cv2.WaitKey(0);
            d = c.HoriRot(b);
            Cv2.ImShow("hori", d);
            Cv2.WaitKey(0);
            d = c.VertRot(b);
            Cv2.ImShow("vert", d);
            Cv2.WaitKey(0);
            Mat d = c.ImgRotate(b, 30, false);
            Cv2.ImShow("d", d);
            Cv2.WaitKey(0);*/
            //Console.WriteLine(d.Channels());
            //Cv2.WaitKey(0);
            /*Mat cc= c.BriAndCon(b, 220, 50);
            Cv2.ImShow("cc", cc);
            Cv2.WaitKey(0);*/
            /*Mat b = Rotate.ImgRotate(a, 135, true);
            Cv2.ImShow("b", b);
            Cv2.WaitKey(0);
            Mat c = Rotate.ImgRotate(b, 45, true);
            Cv2.ImShow("c", c);
            Cv2.WaitKey(0);*/
            /*Mat b = Mood.ToGrey(a);
            Cv2.ImShow("b", b);
            Cv2.WaitKey(0);*/
            /*Mat b = Mood.Reverse(a);
            Cv2.ImShow("b", b);
            Cv2.WaitKey(0);*/
            /*Mat c=Mood.ImageSize(a, 1000, 1000,1);
            Mat d = Mood.ImageSize(b, 1000, 1000, 1);
            Cv2.ImShow("c", c);
            Cv2.WaitKey(0);
            Cv2.ImShow("d", d);
            Cv2.WaitKey(0);*/
            /*Mat c = Process.Gaus(a, 3 , 1,10);
            Cv2.ImShow("3", c);
            Cv2.WaitKey(0);
            c = Process.Gaus(a, 3, 1,1);
            Cv2.ImShow("5", c);
            Cv2.WaitKey(0);
            c = Process.Gaus(a, 7, 1,1);
            Cv2.ImShow("7", c);
            Cv2.WaitKey(0);*/
            /*Mat c = Process.Mid(a, 7, 1);
            Cv2.ImShow("c", c);
            Cv2.WaitKey(0);*/
            /*Mat d = Process.Gaus(b, 3, 1, 1);
            Mat e= Process.BlackHat(b,1,1);
            Cv2.ImShow("e", e);
            Cv2.WaitKey(0);
            Mat c = Process.BlackHat(d,1,1);
            Cv2.ImShow("c", c);
            Cv2.WaitKey(0);*/
            /*Mat c = Mood.HSVColorAdjust(a, 0, 50, 0);
            Cv2.ImShow("c", c);
            Cv2.WaitKey(0);*/
            /*Mat c = Mood.TakeColor(a, 2);
            Cv2.ImShow("c", c);
            Cv2.WaitKey(0);*/
            /*Mat c = Mood.FilterColor(a, 2);
            Cv2.ImShow("c", c);
            Cv2.WaitKey(0);*/
            Mat[] mats = new Mat[20];
            /*mats[0] = Open.OpenImage("C:\\Users\\予感\\Desktop\\切片采集\\精巢\\001.bmp");
            mats[1] = Open.OpenImage("C:\\Users\\予感\\Desktop\\切片采集\\精巢\\002.bmp");
            mats[2] = Open.OpenImage("C:\\Users\\予感\\Desktop\\切片采集\\精巢\\003.bmp");
            mats[3] = Open.OpenImage("C:\\Users\\予感\\Desktop\\切片采集\\精巢\\004.bmp");*/
            //mats[4] = Open.OpenImage("C:\\Users\\予感\\Desktop\\切片采集\\精巢\\005.bmp");
            //mats[0] = Open.OpenImage("C:\\Users\\予感\\Desktop\\切片采集\\精巢\\001.bmp");
            //mats[1] = Open.OpenImage("C:\\Users\\予感\\Desktop\\切片采集\\精巢\\002.bmp");            
            //mats[2] = Open.OpenImage("C:\\Users\\予感\\Desktop\\切片采集\\苍蝇口器\\008.bmp");
            for (int i = 1; i <= 9; i++)
            {
                Console.WriteLine(i);
                mats[i - 1] = Open.OpenImage("C:\\Users\\予感\\Desktop\\切片采集\\木本双子叶植物茎\\00" + i + ".bmp");
                Size ss = mats[i-1].Size();
                int w = ss.Width;
                int h = ss.Height;
                mats[i-1] = Mood.ImageSize(mats[i-1], 500, 500 * h / w, 3);
            }
            for (int i = 10; i <= 17; i++)
            {
                Console.WriteLine(i);
                mats[i - 1] = Open.OpenImage("C:\\Users\\予感\\Desktop\\切片采集\\木本双子叶植物茎\\0" + i + ".bmp");
                Size ss = mats[i-1].Size();
                int w = ss.Width;
                int h = ss.Height;
                mats[i-1] = Mood.ImageSize(mats[i-1], 500, 500 * h / w, 3);
            }
            //mats[0] = Open.OpenImage("C:\\Users\\予感\\Desktop\\切片采集\\木本双子叶植物茎\\001.bmp");
            //mats[1] = Open.OpenImage("C:\\Users\\予感\\Desktop\\切片采集\\木本双子叶植物茎\\015.bmp");
            //Cv2.ImShow("a", mats[0]);
            //Cv2.WaitKey(0);
            //mats =Stitch.PreStitching(mats, 17);
            /*for (int i = 0; i < 9; i++)
                Cv2.ImWrite("C:\\Users\\予感\\Desktop\\00" + (i+1)+".bmp", mats[i]);
            for(int i=9;i<17;i++)
                Cv2.ImWrite("C:\\Users\\予感\\Desktop\\0" + (i + 1) + ".bmp", mats[i]);*/
            Mat mat = Stitch.Stitching(mats, 17);
            /*List<Mat> m = new List<Mat>();
            for (int i = 0; i < 17; i++)
            {
                Size ss = mats[i].Size();
                int w = ss.Width;
                int h = ss.Height;
                mats[i] = Mood.ImageSize(mats[i], 500, 500 * h / w, 3);
                m.Add(mats[i]);
            }
            Console.WriteLine(m.Count);
            Mat mat = new Mat();
            var s = Stitcher.Create();
            s.Stitch(m, mat);*/
            //Cv2.ImWrite("C:\\Users\\予感\\Desktop\\切片采集\\木本双子叶植物茎\\result1.bmp", mat);
            //Console.WriteLine("Done!!!");
            Cv2.ImShow("hhh", mat);
            Cv2.WaitKey(0);
        }
    }
}
