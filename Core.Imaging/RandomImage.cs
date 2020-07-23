using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Core.Imaging
{
    public class RandomImage : IDisposable
    {
        //Private variable
        private readonly Random _random = new Random();

        //property
        public string Text { get; }

        public Bitmap Image { get; private set; }

        public string Base64Image
        {
            get
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                Image.Save(stream, ImageFormat.Bmp);
                var imageBytes = stream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }
        public int Width { get; private set; }

        public int Height { get; private set; }


        //Methods declaration
        public RandomImage(string s, int width, int height)
        {
            this.Text = s;
            this.setDimensions(width, height);
            this.generateImage();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                this.Image.Dispose();
        }
        private void setDimensions(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), width,
                    "Argument out of range, must be greater than zero.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), height,
                    "Argument out of range, must be greater than zero.");
            this.Width = width;
            this.Height = height;
        }
        private void generateImage()
        {
            var bitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, this.Width, this.Height);
            var hatchBrush = new HatchBrush(HatchStyle.SmallConfetti,
                Color.LightGray, Color.White);
            g.FillRectangle(hatchBrush, rect);
            SizeF size;
            float fontSize = rect.Height + 1;
            Font font;

            do
            {
                fontSize--;
                font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold);
                size = g.MeasureString(this.Text, font);
            } while (size.Width > rect.Width);

            var format = new StringFormat
            {
                Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center
            };
            var path = new GraphicsPath();
            //path.AddString(this.text, font.FontFamily, (int) font.Style, 
            //    font.Size, rect, format);
            path.AddString(this.Text, font.FontFamily, (int)font.Style, fontSize, rect, format);
            float v = 4F;
            PointF[] points =
              {
                new PointF(this._random.Next(rect.Width) / v, this._random.Next(
                   rect.Height) / v),
                new PointF(rect.Width - this._random.Next(rect.Width) / v,
                    this._random.Next(rect.Height) / v),
                new PointF(this._random.Next(rect.Width) / v,
                    rect.Height - this._random.Next(rect.Height) / v),
                new PointF(rect.Width - this._random.Next(rect.Width) / v,
                    rect.Height - this._random.Next(rect.Height) / v)
          };
            Matrix matrix = new Matrix();
            matrix.Translate(0F, 0F);
            path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);
            hatchBrush = new HatchBrush(HatchStyle.Percent10, Color.Black, Color.SkyBlue);
            g.FillPath(hatchBrush, path);
            int m = Math.Max(rect.Width, rect.Height);
            for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
            {
                int x = this._random.Next(rect.Width);
                int y = this._random.Next(rect.Height);
                int w = this._random.Next(m / 50);
                int h = this._random.Next(m / 50);
                g.FillEllipse(hatchBrush, x, y, w, h);
            }
            font.Dispose();
            hatchBrush.Dispose();
            g.Dispose();
            this.Image = bitmap;
        }
    }

}
