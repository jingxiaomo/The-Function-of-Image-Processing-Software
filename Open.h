#include<string>
#include <opencv2/opencv.hpp>
using namespace cv;
using namespace std;

Mat OpenImage(string s) {
	Mat src = imread(s);
	imshow(s, src);
	waitKey(0);
	return src;
}

void OpenVideo(string s) {
	VideoCapture cap;
	cap.open(s);
	if (!cap.isOpened())return;//如果视频不能正常打开则返回
	Mat frame;
	while (1)
	{
		cap >> frame;//等价于cap.read(frame);
		//if (frame.empty())//如果某帧为空则退出循环
			//break;
		imshow(s, frame);
		waitKey(20);//每帧延时20毫秒
	}
	cap.release();//释放资源
}

void SaveImage(string s,Mat image) {
	imwrite(s, image);
}