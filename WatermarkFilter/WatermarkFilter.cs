using PluginContracts;
using System.Drawing;
using System;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace WatermarkFilter
{
    [PluginInfo("Водяной знак с метаданными", "Даниил", "1.0")]
    public class WatermarkFilter : IImageFilter
    {
        public string Name => "Watermark Filter";
        public string Author => "Даниил";
        public Version Version => new Version(1, 0);

        public Bitmap Apply(Bitmap image)
        {
            Bitmap result = new Bitmap(image);
            using (Graphics g = Graphics.FromImage(result))
            {
                string dateText = GetImageDate(image) ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string locationText = GetImageLocation(image) ?? GetLocationFromComputer() ?? "Геолокация недоступна";
                string finalText = $"{dateText}\n{locationText}";

                Font font = new Font("Arial", 14, FontStyle.Bold);
                Brush brush = new SolidBrush(Color.FromArgb(200, Color.Black));
                SizeF size = g.MeasureString(finalText, font);
                PointF position = new PointF(result.Width - size.Width - 10, result.Height - size.Height - 10);

                g.DrawString(finalText, font, brush, position);
            }
            return result;
        }

        private string GetImageDate(Bitmap image)
        {
            try
            {
                var propItem = image.PropertyItems.FirstOrDefault(p => p.Id == 36867);
                if (propItem != null)
                {
                    string date = System.Text.Encoding.ASCII.GetString(propItem.Value);
                    date = date.Trim('\0').Replace(':', '-');
                    return date;
                }
            }
            catch { }
            return null;
        }

        private string GetImageLocation(Bitmap image)
        {
            try
            {
                return ParseGpsInfo(image);
            }
            catch { }
            return null;
        }

        private string ParseGpsInfo(Bitmap image)
        {
            try
            {
                string latRef = GetPropString(image, 1); // 0x0001
                double[] latVals = GetPropRational(image, 2); // 0x0002
                string lonRef = GetPropString(image, 3); // 0x0003
                double[] lonVals = GetPropRational(image, 4); // 0x0004

                if (latVals == null || lonVals == null || latRef == null || lonRef == null)
                    return null;

                double latitude = latVals[0] + latVals[1] / 60.0 + latVals[2] / 3600.0;
                if (latRef == "S") latitude = -latitude;

                double longitude = lonVals[0] + lonVals[1] / 60.0 + lonVals[2] / 3600.0;
                if (lonRef == "W") longitude = -longitude;

                return $"Lat: {latitude:0.0000}, Lon: {longitude:0.0000}";
            }
            catch { }
            return null;
        }

        private string GetPropString(Bitmap img, int id)
        {
            try
            {
                var p = img.PropertyItems.FirstOrDefault(x => x.Id == id + 0x0000);
                if (p == null) return null;
                return System.Text.Encoding.ASCII.GetString(p.Value).Trim('\0');
            }
            catch { return null; }
        }
        private double[] GetPropRational(Bitmap img, int id)
        {
            try
            {
                var p = img.PropertyItems.FirstOrDefault(x => x.Id == id + 0x0000);
                if (p == null) return null;
                double[] res = new double[3];
                for (int i = 0; i < 3; i++)
                {
                    int num = BitConverter.ToInt32(p.Value, 8 * i);
                    int den = BitConverter.ToInt32(p.Value, 8 * i + 4);
                    res[i] = den != 0 ? (double)num / den : 0.0;
                }
                return res;
            }
            catch { return null; }
        }

        // Получение координат компьютера по IP через бесплатный сервис ip-api.com
        private string GetLocationFromComputer()
        {
            try
            {
                // Можно использовать любой бесплатный сервис, например ip-api.com
                var url = "http://ip-api.com/json/";
                var request = WebRequest.Create(url);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    var serializer = new JavaScriptSerializer();
                    var data = serializer.Deserialize<GeoResult>(json);
                    if (data.status == "success")
                        return $"Lat: {data.lat:0.0000}, Lon: {data.lon:0.0000}";
                }
            }
            catch { }
            return null;
        }

        private class GeoResult
        {
            public string status { get; set; }
            public double lat { get; set; }
            public double lon { get; set; }
        }
    }
}