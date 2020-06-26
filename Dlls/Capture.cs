using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Dlls
{
    /// <summary>
    /// 捕获图像 定时捕获
    /// </summary>
    public static class Capture
    {
        /// <summary>
        /// 捕获图像
        /// </summary>
        /// <param name="VideoName">视频路径</param>
        /// <param name="f">捕获第f帧</param>
        /// <returns>第f帧的图像矩阵</returns>
        public static Mat Catch(string VideoName, int f)
        {
            VideoCapture m=new VideoCapture();
            m.Open(VideoName);
            Mat goal = new Mat();
            if (!m.IsOpened()) return new Mat();
            for (int i = 0; i < m.FrameCount; i++)
            {
                Mat img = new Mat();
                m.Read(img);
                if (i == f)
                {
                    goal = img.Clone();
                    Cv2.ImShow(i.ToString(), img);
                    Cv2.WaitKey(0);
                }
            }
            m.Release();
            return goal;
        }

        /// <summary>
        /// 定时捕获
        /// </summary>
        /// <param name="now">当前视频路径</param>
        /// <param name="save">保存视频路径 示例：C:\\Users\\a\\Desktop\\</param>
        /// <param name="f">当前帧数</param>
        /// <param name="t">捕获间隔（秒）</param>
        /// <param name="cnt">捕获张数</param>
        public static void TimeCatch(string now, string save, int f, int t, int cnt)
        {
            VideoCapture m=new VideoCapture();
            m.Open(now);
            if (!m.IsOpened()) return;
            int z = 24 * t;//秒->帧
            int num = 0;
            for (int i = f + 1; i < m.FrameCount; i++)
            {
                Mat img = new Mat();
                m.Read(img);
                if ((i - f) % z == 0)
                {
                    Cv2.ImWrite(save + i.ToString() + ".jpg", img);
                    num++;
                    if (num == cnt) return;
                }
            }
            m.Release();
        }
    }
}
