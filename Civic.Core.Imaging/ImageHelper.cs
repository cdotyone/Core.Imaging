using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Civic.Core.Imaging
{
	public static class ImageHelper
	{
		/// <summary>
		/// returns mimetype from file extension
		/// </summary>
		/// <param name="name">file extension</param>
		/// <returns>the mimetype</returns>
		public static string GetMimeType(string name)
		{
			string mimetype = string.Empty;
            if (!name.Contains(".")) name = "1." + name;
			var ext = Path.GetExtension(name);
			if (ext != null)
			{
				switch (ext.ToLower())
				{
					case ".jpeg":
					case ".jpg":
						mimetype = "image/jpeg";
						break;
					case ".gif":
						mimetype = "image/gif";
						break;
					case ".png":
						mimetype = "image/png";
						break;
				}
			}

			return mimetype;
		}


		/// <summary>
		/// Create a thumbnail
		/// </summary>
		/// <param name="fullImage">Source image</param>
		/// <param name="contentType">mimetype of the stream</param>
		/// <param name="maxWidth">maximum width</param>
		/// <param name="maxHeight">maximum height</param>
		/// <param name="cropToSize">can we crop to make thumbnail fit, helps create uniform thumbnail tiles</param>
		/// <param name="thumbnailStream">The stream to write the thumbnail to</param>
		public static void CreateThumbnail(Stream fullImage, string contentType, int maxWidth, int maxHeight, bool cropToSize, Stream thumbnailStream, string backgroundRGBAData = "rgba(255,255,255,1)")
		{
			var image = Image.FromStream(fullImage);

			int newWidth, newHeight;
		    if (maxWidth == 0) maxWidth = image.Width;
		    if (maxHeight == 0) maxHeight = image.Height;

            ResizeWithAspect(cropToSize, image.Width, image.Height, maxWidth, maxHeight, out newWidth, out newHeight);

			var thumbnail = new Bitmap(newWidth, newHeight);
			var graphic = Graphics.FromImage(thumbnail);
            
            var regex = new Regex(@"([0-9]+\.[0-9]+)");
            var matches = regex.Matches(backgroundRGBAData);
            int r = GetColorValue(matches[0].Value);
            int g = GetColorValue(matches[1].Value);
            int b = GetColorValue(matches[2].Value);
            int a = GetColorValue(matches[3].Value);

            var color = Color.FromArgb(a, r, g, b);
            graphic.Clear(color);

            

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphic.SmoothingMode = SmoothingMode.HighQuality;
			graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
			graphic.CompositingQuality = CompositingQuality.HighQuality;

			graphic.DrawImage(image, 0, 0, newWidth, newHeight);


            if (cropToSize)
			{
				var thumbnailCrop = new Bitmap(maxWidth, maxHeight);
				var graphicCrop = Graphics.FromImage(thumbnailCrop);
                graphicCrop.DrawImage(thumbnail, new Rectangle(0, 0, maxWidth, maxHeight), (newWidth - maxWidth)/2, (newHeight-maxHeight)/3, maxWidth, maxHeight, GraphicsUnit.Pixel);
				thumbnail = thumbnailCrop;
			}

			if (contentType == "image/gif")
			{
				using (thumbnail)
				{
					var quantizer = new OctreeQuantizer(255, 8);
					using (Bitmap quantized = quantizer.Quantize(thumbnail))
					{
						quantized.Save(thumbnailStream, ImageFormat.Gif);
					}
				}
			}

			if (contentType == "image/jpeg")
			{
				var info = ImageCodecInfo.GetImageEncoders();
				var encoderParameters = new EncoderParameters(1);
				encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
				thumbnail.Save(thumbnailStream, info[1], encoderParameters);
			}

			if (contentType == "image/png")
			{
				using (thumbnail)
				{
					var quantizer = new OctreeQuantizer(255, 8);
					using (Bitmap quantized = quantizer.Quantize(thumbnail))
					{
                        quantized.Save(thumbnailStream, ImageFormat.Png);
					}
				}
			}

            graphic.Flush();
        }

        /// <summary>
        /// Determines best size to resize to given the maxes and if we can crop
        /// </summary>
        private static void ResizeWithAspect(bool cropToSize, int originalWidth, int originalHeight, int maxWidth, int maxHeight, out int sizedWidth, out int sizedHeight)
		{
			sizedHeight = maxHeight;
			sizedWidth = maxWidth;

			float aspect;

			float percentW = (float) maxWidth / (float) originalWidth;
			float percentH = (float) maxHeight / (float) originalHeight;
			if (percentH < percentW)
			{
				aspect = (float)originalHeight / (float)originalWidth;
				sizedWidth = (int)(originalWidth * percentH);
			}
			else
			{
				aspect = (float)originalWidth / (float)originalHeight;
				sizedHeight = (int)(originalHeight * percentW);
			}

			if (!cropToSize) return;
			if (sizedHeight < maxHeight)
			{
				sizedHeight = maxHeight;
				sizedWidth = (int) (sizedHeight*aspect);
				return;
			}
			if (sizedWidth < maxWidth)
			{
				sizedWidth = maxWidth;
				sizedHeight = (int)(sizedWidth * aspect);
			}
		}


        private static int GetColorValue(string match)
        {
            return (int)Math.Round(double.Parse(match, CultureInfo.InvariantCulture) * 255);
        }

		public static string ByteSize(long size)
		{
			var sizeSuffixes = new []{ "B", "KB","MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
			const string FORMAT_TEMPLATE = "{0}{1:0.#} {2}";

			if (size == 0)
			{
				return string.Format(FORMAT_TEMPLATE, null, 0, sizeSuffixes[0]);
			}

			var absSize = Math.Abs((double)size);
			var fpPower = Math.Log(absSize, 1000);
			var intPower = (int)fpPower;
			var iUnit = intPower >= sizeSuffixes.Length
							? sizeSuffixes.Length - 1
							: intPower;
			var normSize = absSize / Math.Pow(1000, iUnit);

			return string.Format(
				FORMAT_TEMPLATE,
				size < 0 ? "-" : null, normSize, sizeSuffixes[iUnit]);
		}
	}
}