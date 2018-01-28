using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Helpers
{
    public class ImageHelper
    {
        /// <summary>
        /// 为图片生成缩略图
        /// </summary>
        /// <param name="image">原图片</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>缩略图</returns>
        public static System.Drawing.Image GetThumbnail(System.Drawing.Image image, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            //从Bitmap创建一个System.Drawing.Graphics
            Graphics gr = Graphics.FromImage(bmp);
            //设置 
            gr.SmoothingMode = SmoothingMode.AntiAlias;
            //下面这个也设成高质量
            gr.CompositingQuality = CompositingQuality.HighQuality;
            //下面这个设成High
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //把原始图像绘制成上面所设置宽高的缩小图
            System.Drawing.Rectangle rectDestination = new Rectangle(0, 0, width, height);

            gr.DrawImage(image, rectDestination, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            return bmp;
        }
        public static byte[] convertByte(Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                //image.Save("D:\\test.jpg");
                //image.Save(ms, format);
                //if (format.Guid.ToString() == ImageFormat.Jpeg.Guid.ToString())
                //{
                //    image.Save(ms, ImageFormat.Jpeg);
                //}
                //else if (format.Guid.ToString() == ImageFormat.Png.Guid.ToString())
                //{
                //    image.Save(ms, ImageFormat.Png);
                //}
                //else if (format.Guid.ToString() == ImageFormat.Bmp.Guid.ToString())
                //{
                //    image.Save(ms, ImageFormat.Bmp);
                //}
                //else if (format.Guid.ToString() == ImageFormat.Gif.Guid.ToString())
                //{
                //    image.Save(ms, ImageFormat.Gif);
                //}
                //else if (format.Guid.ToString() == ImageFormat.Icon.Guid.ToString())
                //{
                //    image.Save(ms, ImageFormat.Icon);
                //}
                //else if (format.Guid.ToString() == ImageFormat.MemoryBmp.Guid.ToString())
                //{
                //    image.Save(ms, ImageFormat.MemoryBmp);
                //}
                byte[] buffer = new byte[ms.Length];
                //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
        public static Image convertImg(byte[] datas)
        {
            MemoryStream ms = new MemoryStream(datas);
            Image img = Image.FromStream(ms, true);//在这里出错  
            //流用完要及时关闭  
            ms.Close();
            return img;
        }
        /// <summary>  
        /// 无损压缩图片  
        /// </summary>  
        /// <param name="sFile">原图片</param>  
        /// <param name="dFile">压缩后保存位置</param>  
        /// <param name="dHeight">高度</param>  
        /// <param name="dWidth">宽度</param>  
        /// <param name="flag">压缩质量 1-100</param>  
        /// <returns></returns>  
        public static bool CompressImage(string sFile, string dFile, int dHeight, int dWidth, int flag)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            ImageFormat tFormat = iSource.RawFormat;
            int sW = 0, sH = 0;
            int IntWidth; //新的图片宽  
            int IntHeight; //新的图片高  
            System.Drawing.Imaging.ImageFormat format = iSource.RawFormat;
            

            //计算缩放图片的大小  

            if (iSource.Width > dWidth && iSource.Height <= dHeight)//宽度比目的图片宽度大，长度比目的图片长度小  
            {
                IntWidth = dWidth;
                IntHeight = (IntWidth * iSource.Height) / iSource.Width;
            }
            else if (iSource.Width <= dWidth && iSource.Height > dHeight)//宽度比目的图片宽度小，长度比目的图片长度大  
            {
                IntHeight = dHeight;
                IntWidth = (IntHeight * iSource.Width) / iSource.Height;
            }
            else if (iSource.Width <= dWidth && iSource.Height <= dHeight) //长宽比目的图片长宽都小  
            {
                IntWidth = iSource.Width;
                IntHeight = iSource.Height;
            }
            else//长宽比目的图片的长宽都大  
            {
                IntWidth = dWidth;
                IntHeight = (IntWidth * iSource.Height) / iSource.Width;
                if (IntHeight > dHeight)//重新计算  
                {
                    IntHeight = dHeight;
                    IntWidth = (IntHeight * iSource.Width) / iSource.Height;
                }
            }
            sW = IntWidth;
            sH = IntHeight;

            System.Drawing.Bitmap SaveImage = new System.Drawing.Bitmap(sW, sH);
            Graphics g1 = Graphics.FromImage(SaveImage);
            g1.Clear(Color.White);
            Bitmap ob = new Bitmap(sW, sH);
            Graphics g = Graphics.FromImage(ob);
            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, new Rectangle(0, 0, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
            g.Dispose();
            //以下代码为保存图片时，设置压缩质量  
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100  
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径  
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }

        }
        /// <summary>
        /// 获取图片指定部分
        /// </summary>
        /// <param name="pPath">图片路径</param>
        /// <param name="pPartStartPointX">目标图片开始绘制处的坐标X值(通常为0)</param>
        /// <param name="pPartStartPointY">目标图片开始绘制处的坐标Y值(通常为0)</param>
        /// <param name="pPartWidth">目标图片的宽度</param>
        /// <param name="pPartHeight">目标图片的高度</param>
        /// <param name="pOrigStartPointX">原始图片开始截取处的坐标X值</param>
        /// <param name="pOrigStartPointY">原始图片开始截取处的坐标Y值</param>
        public static Bitmap GetPart(string pPath, int pPartStartPointX, int pPartStartPointY, int pPartWidth, int pPartHeight, int pOrigStartPointX, int pOrigStartPointY)
        {
            Image originalImg = Image.FromFile(pPath);

            Bitmap partImg = new Bitmap(pPartWidth, pPartHeight);
            Graphics graphics = Graphics.FromImage(partImg);
            Rectangle destRect = new Rectangle(new Point(pPartStartPointX, pPartStartPointY), new System.Drawing.Size(pPartWidth, pPartHeight));//目标位置
            Rectangle origRect = new Rectangle(new Point(pOrigStartPointX, pOrigStartPointY), new System.Drawing.Size(pPartWidth, pPartHeight));//原图位置（默认从原图中截取的图片大小等于目标图片的大小）

            graphics.DrawImage(originalImg, destRect, origRect, GraphicsUnit.Pixel);

            return partImg;
        }


    }
}
