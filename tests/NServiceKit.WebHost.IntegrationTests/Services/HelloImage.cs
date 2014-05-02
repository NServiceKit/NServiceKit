using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NServiceKit.Common.Web;
using NServiceKit.Service;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A hello image.</summary>
    [Route("/HelloImage")]
    public class HelloImage {}

    /// <summary>A hello image service.</summary>
    public class HelloImageService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(HelloImage request)
        {
            using (var image = new Bitmap(10, 10))
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.Clear(Color.Red);
                }
                var ms = new MemoryStream();
                image.Save(ms, ImageFormat.Png);
                return new HttpResult(ms, "image/png"); //writes stream directly to response
            }
        }
    }

    /// <summary>A hello image 2.</summary>
    [Route("/HelloImage2")]
    public class HelloImage2 {}

    /// <summary>A hello image 2 service.</summary>
    public class HelloImage2Service : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(HelloImage2 request)
        {
            using (Bitmap image = new Bitmap(10, 10))
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.Clear(Color.Red);
                }
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, ImageFormat.Png);
                    var imageData = m.ToArray(); //buffers
                    return new HttpResult(imageData, "image/png");
                }
            }
        }
    }

    /// <summary>A hello image 3.</summary>
    [Route("/HelloImage3")]
    public class HelloImage3 {}

    //Your own Custom Result, writes directly to response stream
    public class ImageResult : IDisposable, IStreamWriter, IHasOptions
    {
        private readonly Image image;
        private readonly ImageFormat imgFormat;

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.ImageResult class.</summary>
        ///
        /// <param name="image">    The image.</param>
        /// <param name="imgFormat">The image format.</param>
        public ImageResult(Image image, ImageFormat imgFormat=null)
        {
            this.image = image;
            this.imgFormat = imgFormat ?? ImageFormat.Png;
            this.Options = new Dictionary<string, string> {
                { HttpHeaders.ContentType, "image/" + this.imgFormat.ToString().ToLower() }
            };
        }

        /// <summary>Writes to.</summary>
        ///
        /// <param name="responseStream">The response stream.</param>
        public void WriteTo(Stream responseStream)
        {
            image.Save(responseStream, imgFormat);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            this.image.Dispose();
        }

        /// <summary>Gets or sets options for controlling the operation.</summary>
        ///
        /// <value>The options.</value>
        public IDictionary<string, string> Options { get; set; }
    }

    /// <summary>A hello image 3 service.</summary>
    public class HelloImage3Service : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(HelloImage3 request)
        {
            var image = new Bitmap(10, 10);
            using (var g = Graphics.FromImage(image))
                g.Clear(Color.Red);

            return new ImageResult(image); //terse + explicit is good :)
        }
    }


}