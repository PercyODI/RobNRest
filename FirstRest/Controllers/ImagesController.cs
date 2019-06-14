using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FirstRest.Controllers
{
    public class ImagesController : ApiController
    {
        [HttpPost]
        public IHttpActionResult MyFileUpload()
        [HttpPost]
        public IHttpActionResult MyFileUpload()
        {
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];

                    var filePath = "C:\\inetpub\\wwwroot\\RobNRest\\pictures\\" + postedFile.FileName;

                    postedFile.SaveAs(filePath);
                    docfiles.Add(filePath);
                }

                return Ok(httpRequest.Files.AllKeys.Select(k => "http://dkw99robnrest/RobNRest/pictures/" + k));
            }
            else
            {
                return BadRequest();
            }

        }
    }
}
