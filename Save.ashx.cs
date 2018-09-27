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

            #region 旧版本
            //byte[] fileData = null;
            //using (var binaryReader = new BinaryReader(context.Request.InputStream))
            //{
            //    fileData = binaryReader.ReadBytes(Convert.ToInt32(context.Request.InputStream.Length));
            //}

            //string path = context.Server.MapPath("");
            //FileStream fs = File.Create(path);
            //fs.Write(fileData, 0, fileData.Length);
            //fs.Dispose();
            //fs.Close(); 
            #endregion

            #region 旧版本1.0
            //PersonTest per = PersonTest.ParseFrom(fileData);//反序列化为对象

            //string filename = per.Name;
            //Google.ProtocolBuffers.ByteString str = per.Buf;
            //string path = context.Server.MapPath(filename);
            //if (per.Isimg)
            //{
            //    ImageHelper.SaveFromBufferOpenOrCreate(str.ToByteArray(), path);//保存图片操作
            //}
            //else
            //{
            //    FileHelper.SaveFile(str.ToByteArray(), path);//保存图片操作
            //}
            //var builder = per.ToBuilder();

            //builder.SetName(per.Name);
            //builder.SetEmail(per.Email);
            //builder.SetId(1001);
            //builder.SetIsimg(false);

            //ByteString bs = ByteString.CopyFrom("ok", System.Text.Encoding.UTF8);
            //builder.SetBuf(bs);


            //per = builder.Build();

            //context.Response.ContentType = "application/protobuf";
            //context.Response.BinaryWrite(per.ToByteArray());

            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World"); 
            #endregion

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