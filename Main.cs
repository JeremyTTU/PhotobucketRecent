using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PhotobucketRecent
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (!Directory.Exists ("images"))
				Directory.CreateDirectory ("images");

			var regex = new Regex (@"http://i[0-9]{1,}.photobucket.com/albums/.+?/(.*?(jpg|jpeg|png|bmp))");

			var pages = 0;

			while (true) {
				var result = new WebClient ().DownloadString ("http://photobucket.com/images/recent/");
				var matcheses = regex.Matches (result);
				var matches = new Match[matcheses.Count];
				matcheses.CopyTo (matches, 0);

				Parallel.ForEach (matches, (match) => {
					try {
						if (match == null) return;
						var path = match.Groups [1].Value.Replace ("/th_", "/");
						path = path.Substring (0, path.LastIndexOf ("/"));

						var f = match.Groups [1].Value;
						f = f.Substring (f.LastIndexOf ("/"), f.Length - f.LastIndexOf ("/"));

						if (!File.Exists ("images/" + match.Groups [1].Value.Replace ("/th_", "/"))) {
							new WebClient ().DownloadFile (match.Value.Replace ("/th_", "/"), "images/" + f.Replace ("/th_", "/"));
						}
					} catch (Exception ex) {
						Console.WriteLine ("Error: " + ex.Message);
					}
				});

				pages++;
				Console.WriteLine ("Completed page #{0}...", pages);
				Thread.Sleep (1000);
			}
		}
	}
}