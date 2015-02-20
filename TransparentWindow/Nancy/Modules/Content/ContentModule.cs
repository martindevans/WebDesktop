using System;
using System.IO;
using System.IO.Abstractions;
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
            foreach (var pathPair in _configuration.Paths)
            {
                if (path.StartsWith(pathPair.Key, StringComparison.InvariantCultureIgnoreCase))
                {
                    var sub = path.Substring(pathPair.Key.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    return Path.Combine(pathPair.Value, sub);
                }
            }

            return null;
        }
    }
}
