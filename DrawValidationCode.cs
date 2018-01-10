using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    /// <summary>
    /// 验证码生成类
    /// </summary>
    public class DrawValidationCode
    {
        private int letterWidth = 16;//单个字体的宽度范围  
        private int letterHeight = 22;//单个字体的高度范围  
        int randAngle = 45;// 随机转动角度
        private char[] chars = "0123456789".ToCharArray();
        private string[] fonts = { "Arial", "Georgia" };
        /// <summary>  
        /// 产生波形滤镜效果  
        /// </summary>  
        private const double PI = 3.1415926535897932384626433832795;
        private const double PI2 = 6.283185307179586476925286766559;

        /// <summary>
        /// string str_ValidateCode = GetRandomNumberString(letterCount);
        ///CreateImage(str_ValidateCode);
        /// </summary>
        /// <param name="checkCode"></param>
        public Bitmap CreateImage(string checkCode)
        {
            int int_ImageWidth = checkCode.Length * letterWidth;
            Random newRandom = new Random();
            Bitmap image = new Bitmap(int_ImageWidth, letterHeight);
            Graphics g = Graphics.FromImage(image);
            //生成随机生成器  
            Random random = new Random();
            //白色背景  
            g.Clear(Color.White);
            //画图片的背景噪音线  
            for (int i = 0; i < 10; i++)
            {
                int x1 = random.Next(image.Width);
                int x2 = random.Next(image.Width);
                int y1 = random.Next(image.Height);
                int y2 = random.Next(image.Height);

                g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
            }

            //画图片的前景噪音点  
            for (int i = 0; i < 10; i++)
            {
                int x = random.Next(image.Width);
                int y = random.Next(image.Height);

                image.SetPixel(x, y, Color.FromArgb(random.Next()));
            }
            //随机字体和颜色的验证码字符  

            int findex;
            for (int int_index = 0; int_index < checkCode.Length; int_index++)
            {
                findex = newRandom.Next(fonts.Length - 1);
                string str_char = checkCode.Substring(int_index, 1);
                //Brush newBrush = new SolidBrush(GetRandomColor());
                Brush newBrush = new LinearGradientBrush(new Point(0, 0), new Point(letterWidth, letterHeight), GetRandomColor(), GetRandomColor());
                int pointX = 0;
                if (int_index == 0)
                {
                    pointX = newRandom.Next(4);
                }
                else
                {
                    pointX = int_index * letterWidth - 2 + newRandom.Next(4);
                }
                Point thePos = new Point(pointX, 1 + newRandom.Next(3));//5+1+a+s+p+x  
                Point dot = new Point(23, 15);
                //// 转动的度数
                //float angle = random.Next(-randAngle, randAngle);

                //// 移动光标到指定位置
                //g.TranslateTransform(dot.X, dot.Y);
                //g.RotateTransform(angle);
                g.DrawString(str_char, new Font(fonts[findex], random.Next(12,14), FontStyle.Bold), newBrush, thePos);
                //graph.DrawString(chars[i].ToString(),fontstyle,new SolidBrush(Color.Blue),1,1,format);
                // 转回去
                //g.RotateTransform(-angle);
                //// 移动光标到指定位置
                //g.TranslateTransform(3, -dot.Y);


                //g.DrawString(str_char, new Font(fonts[findex], 12, FontStyle.Bold), newBrush, thePos);
            }
            //灰色边框  
            g.DrawRectangle(new Pen(Color.LightGray, 1), 0, 0, int_ImageWidth - 1, (letterHeight - 1));
            //图片扭曲  
            image = TwistImage(image, true, random.Next(3,4), random.Next(0, (int)(2 * PI)));  
            //将生成的图片发回客户端  
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            return image;
        }
        /// <summary>  
        /// 正弦曲线Wave扭曲图片  
        /// </summary>  
        /// <param name="srcBmp">图片路径</param>  
        /// <param name="bXDir">如果扭曲则选择为True</param>  
        /// <param name="nMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>  
        /// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>  
        /// <returns></returns>  
        public System.Drawing.Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            System.Drawing.Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            // 将位图背景填充为白色  
            System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();
            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;

            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI2 * (double)j) / dBaseAxisLen : (PI2 * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);
                    // 取得当前点的颜色  
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);
                    System.Drawing.Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }
            return destBmp;
        }

        public Color GetRandomColor()
        {
            Random RandomNum_First = new Random((int)DateTime.Now.Ticks);
            System.Threading.Thread.Sleep(RandomNum_First.Next(50));
            Random RandomNum_Sencond = new Random((int)DateTime.Now.Ticks);
            int int_Red = RandomNum_First.Next(210);
            int int_Green = RandomNum_Sencond.Next(180);
            int int_Blue = (int_Red + int_Green > 300) ? 0 : 400 - int_Red - int_Green;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;
            return Color.FromArgb(int_Red, int_Green, int_Blue);// 5+1+a+s+p+x  
        }

        //  生成随机数字字符串  
        public string GetRandomNumberString(int int_NumberLength)
        {
            Random random = new Random();
            string validateCode = string.Empty;
            for (int i = 0; i < int_NumberLength; i++)
                validateCode += chars[random.Next(0, chars.Length)].ToString();
            return validateCode;
        }
    }
}