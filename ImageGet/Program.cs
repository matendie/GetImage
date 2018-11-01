using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ImageGet
{
	class Program
	{
		static void Main(string[] args)
		{
			string path = @"C:\Dev\TestSites\GetImage\GetImage\ImageGet\Images\";
			for (int i = 0; i < 15; i++)
			{
				try
				{
					Image img = null;
					Browser br = new Browser();
					string html = br.GetHtmlCode();
					List<string> urls = br.GetUrls(html);
					var rnd = new Random();

					int randomUrl = rnd.Next(0, urls.Count - 1);

					string luckyUrl = urls[randomUrl];

					byte[] image = br.GetImage(luckyUrl);
					using (var ms = new MemoryStream(image))
					{
						img = Image.FromStream(ms);
					}

					using (var newBitmap = new Bitmap(img))
					{
						newBitmap.Save(path + rnd.Next(10000000, 90000000) + ".Jpeg", ImageFormat.Jpeg);
					}
				}
				catch (Exception)
				{
					//throw;
				}
			}
		}
	}

	public class Browser
	{
		private readonly List<string> _topics = new List<string> {"places", "animals", "kitchen objects", "under the sea" };

		public string GetHtmlCode()
		{
			var rnd = new Random();

			int topic = rnd.Next(0, _topics.Count - 1);

			string url = "https://www.google.com/search?q=" + _topics[topic] + "&tbm=isch";
			string data = "";

			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Accept = "text/html, application/xhtml+xml, */*";
			request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

			var response = (HttpWebResponse)request.GetResponse();

			using (Stream dataStream = response.GetResponseStream())
			{
				if (dataStream == null)
					return "";
				using (var sr = new StreamReader(dataStream))
				{
					data = sr.ReadToEnd();
				}
			}
			return data;
		}

		public List<string> GetUrls(string html)
		{
			var urls = new List<string>();

			int ndx = html.IndexOf("\"ou\"", StringComparison.Ordinal);

			while (ndx >= 0)
			{
				ndx = html.IndexOf("\"", ndx + 4, StringComparison.Ordinal);
				ndx++;
				int ndx2 = html.IndexOf("\"", ndx, StringComparison.Ordinal);
				string url = html.Substring(ndx, ndx2 - ndx);
				urls.Add(url);
				ndx = html.IndexOf("\"ou\"", ndx2, StringComparison.Ordinal);
			}
			return urls;
		}

		public List<string> GetUrls2(string html)
		{
			var urls = new List<string>();

			string search = @",""ou"":""(.*?)"",";
			MatchCollection matches = Regex.Matches(html, search);

			foreach (Match match in matches)
			{
				urls.Add(match.Groups[1].Value);
			}

			return urls;
		}
		public byte[] GetImage(string url)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			var response = (HttpWebResponse)request.GetResponse();

			using (Stream dataStream = response.GetResponseStream())
			{
				if (dataStream == null)
					return null;
				using (var sr = new BinaryReader(dataStream))
				{
					byte[] bytes = sr.ReadBytes(100000000);

					return bytes;
				}
			}

		}


	}

}
