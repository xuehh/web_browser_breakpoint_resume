using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using Common;

namespace UploaderDemo
{
    /// <summary>
    /// Save 的摘要说明
    /// </summary>
    public class Save : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string rootFilePath = context.Server.MapPath("/FileUpload/");

                var md5 = context.Request.Form["md5"];
                var filename = context.Request.Form["fileName"];
                var len = long.Parse(context.Request.Form["len"]);
                long appendedLong = 0;

                string path = string.Format("{0}/{1}", rootFilePath, md5);
                //context.Server.MapPath(string.Format("/{0}", md5));

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = string.Format("{0}/{1}", path, filename);

                //保存文件到根目录 App_Data + 获取文件名称和格式
                //var filePath =context.Server.MapPath("~/") + context.Request.Form["fileName"];
                //创建一个追加（FileMode.Append）方式的文件流
                using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        //读取文件流
                        BinaryReader br = new BinaryReader(context.Request.Files[0].InputStream);
                        //将文件留转成字节数组
                        byte[] bytes = br.ReadBytes((int)context.Request.Files[0].InputStream.Length);
                        //将字节数组追加到文件
                        bw.Write(bytes);

                        appendedLong = fs.Length;
                    }
                }
                if (appendedLong == len)
                {
                    //
                    string newpath = string.Format("{0}/{1}", rootFilePath, filename);
                    // context.Server.MapPath(string.Format("/{0}", filename));

                    FileInfo fi = new FileInfo(path);
                    fi.MoveTo(newpath);
                    Directory.Delete(string.Format("{0}/{1}", rootFilePath, md5));
                }
                context.Response.ContentType = "application/json";
                context.Response.Write(JsonConvert.SerializeObject(new { index = appendedLong }));
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.Write(JsonConvert.SerializeObject(new { index = 0 }));

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
