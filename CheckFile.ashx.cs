using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using Common;


namespace UploaderDemo
{
    /// <summary>
    /// CheckFile 的摘要说明
    /// </summary>
    public class CheckFile : IHttpHandler
    {


        public void ProcessRequest(HttpContext context)
        {
            string rootFilePath = context.Server.MapPath("/FileUpload/");
            var md5 = context.Request.Form["md5"];
            var filename = context.Request.Form["fileName"];
            long len = long.Parse(context.Request.Form["len"]);

            //eg:fileupload/filename.zip
            string path = string.Format("{0}/{1}", rootFilePath, filename); //context.Server.MapPath(string.Format("/{0}", filename));

            context.Response.ContentType = "application/json";

            if (File.Exists(path))
            {
                byte[] buf = File.ReadAllBytes(path);

                var existLen = buf.Length;
                if (existLen == len && MD5Helper.GetBytesMD5(buf) == md5)
                {
                    //删除临时目录
                    string newpath = string.Format("{0}/{1}", rootFilePath, md5);

                    if (Directory.Exists(newpath))
                    {
                        Directory.Delete(newpath);
                    }
                    context.Response.Write(JsonConvert.SerializeObject(new { index = existLen }));
                }
                else if (existLen < len)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { index = existLen }));
                }
                else
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { index = -1 }));
                }

                //fs.Length
            }
            else
            {
                path = string.Format("{0}/{1}/{2}", rootFilePath, md5, filename);
                //context.Server.MapPath(string.Format("{0}/{1}", md5, filename));

                if (File.Exists(path))
                {
                    byte[] buf = File.ReadAllBytes(path);

                    var existLen = buf.Length;
                    if (existLen == len && MD5Helper.GetBytesMD5(buf) == md5)
                    {
                        //移动文件删除临时目录
                        string newpath = string.Format("{0}/{1}", rootFilePath, filename);
                        //context.Server.MapPath(string.Format("/{0}", filename));

                        FileInfo fi = new FileInfo(path);
                        fi.MoveTo(newpath);//从临时目录中移出

                        Directory.Delete(string.Format("{0}/{1}", rootFilePath, md5));//删除临时目录

                        context.Response.Write(JsonConvert.SerializeObject(new { index = existLen }));

                    }
                    else if (existLen < len)
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new { index = existLen }));
                    }
                    else
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new { index = -1 }));
                    }

                }
                else
                {

                    context.Response.Write(JsonConvert.SerializeObject(new { index = 0 }));
                }
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}