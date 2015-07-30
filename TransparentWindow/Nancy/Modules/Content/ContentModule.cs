using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using Nancy;
using TransparentWindow.DataSource;

namespace TransparentWindow.Nancy.Modules.Content
{
    public class ContentModule
        : BaseModule
    {
        private readonly Configuration _configuration;

        public ContentModule(Configuration configuration)
            :base("content")
        {
            _configuration = configuration;

            Get["/{search_path*}", runAsync: true] = Async(FindView);
        }

        private dynamic FindView(dynamic request, CancellationToken ct)
        {
            var path = request.search_path;
            if (!path.HasValue)
                return HttpStatusCode.BadRequest;

            ct.ThrowIfCancellationRequested();
            var filePath = FindResource((string)path);

            if (filePath == null)
                return HttpStatusCode.NotFound;

            var fileInfo = new FileInfoWrapper(new FileInfo(filePath));
            if (!fileInfo.Exists)
                return System.Net.HttpStatusCode.NotFound;

            return Response.FromStream(fileInfo.OpenRead(), MimeTypes.GetMimeType(filePath));
        }

        private string FindResource(string path)
        {
            return (from pathPair in _configuration.Paths
                    where path.StartsWith(pathPair.Key, StringComparison.InvariantCultureIgnoreCase)
                    let sub = path.Substring(pathPair.Key.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    select Path.Combine(pathPair.Value, sub)
            ).FirstOrDefault();
        }
    }
}
