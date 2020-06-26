using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Dlls
{
    /// <summary>
    /// 打开图片 打开视频 保存图片
    /// </summary>
    public static class Open
    {
        /// <summary>
        /// 打开图片
        /// </summary>
        /// <param name="s">图片路径</param>
        /// <returns>图像矩阵</returns>
        public static Mat OpenImage(string s)
        {
            Mat src = new Mat(s, ImreadModes.Color);
            //Cv2.ImShow(s, src);
            //Cv2.WaitKey(0);
            return src;
        }

        /// <summary>
        /// 打开视频
        /// </summary>
        /// <param name="s">视频路径</param>
        public static void OpenVideo(string s)
        {
            VideoCapture cap = new VideoCapture();
            cap.Open(s);
            if (!cap.IsOpened()) { Console.WriteLine("Error!"); Console.ReadKey(); return; }
            Mat frame = new Mat();
            while (true)
            {
                cap.Read(frame);
                if (frame.Empty()) break;
                Cv2.ImShow(s, frame);
                Cv2.WaitKey(20);//每帧延时20毫秒
            }
            cap.Release();
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="s">图片保存路径</param>
        /// <param name="image">图像矩阵</param>
        public static void SaveImage(string s, Mat image)
        {
            Cv2.ImWrite(s, image);
        }
    }
}
