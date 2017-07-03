using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace just.soft.files.Controllers
{
    public class FileController : ApiController
    {
        // GET: api/File
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/File/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/File
        public void Post([FromBody]string value)
        {

        }

        // PUT: api/File/5
        public void Put(int id, [FromBody]string value)
        {

        }

        // DELETE: api/File/5
        public void Delete(int id)
        {
        }

        [Route("upload5")]
        public int Post_bigFile1()
        {
            //前端传输是否为切割文件最后一个小文件
            var isLast = HttpContext.Current.Request["isLast"];
            //前端传输当前为第几次切割小文件
            var count = HttpContext.Current.Request["count"];
            //获取前端处理过的传输文件名
            string fileName = HttpContext.Current.Request["name"];
            //存储接受到的切割文件
            if (HttpContext.Current.Request.Files.Count <= 0) return -1;
            HttpPostedFile file = HttpContext.Current.Request.Files[0];

            //处理文件名称(去除.part*，还原真实文件名称)

            string newFileName = fileName.Substring(0, fileName.LastIndexOf('.'));
            //判断指定目录是否存在临时存储文件夹，没有就创建
            if (!System.IO.Directory.Exists(@"E:\uploaded\slice\" + newFileName))
            {
                //不存在就创建目录 
                System.IO.Directory.CreateDirectory(@"E:\uploaded\slice\" + newFileName);
            }
            //存储文件
            file.SaveAs("E:\\uploaded\\slice\\" + newFileName + "\\" + HttpContext.Current.Request["name"]);
            //判断是否为最后一次切割文件传输
            if (isLast == "true")
            {
                //判断组合的文件是否存在
                if (File.Exists(@"E:\\uploaded\\" + newFileName))//如果文件存在
                {
                    File.Delete(@"E:\\uploaded\\" + newFileName);//先删除,否则新文件就不能创建
                }
                //创建空的文件流
                FileStream FileOut = new FileStream(@"E:\\uploaded\\" + newFileName, FileMode.CreateNew, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(FileOut);
                //获取临时存储目录下的所有切割文件
                string[] allFile = Directory.GetFiles("E:\\uploaded\\slice\\" + newFileName);
                //将文件进行排序拼接
                allFile = allFile.OrderBy(s => int.Parse(Regex.Match(s, @"\d+$").Value)).ToArray();
                for (int i = 0; i < allFile.Length; i++)
                {
                    FileStream FileIn = new FileStream(allFile[i], FileMode.Open);
                    BinaryReader br = new BinaryReader(FileIn);
                    byte[] data = new byte[1048576];   //流读取,缓存空间
                    int readLen = 0;                //每次实际读取的字节大小
                    readLen = br.Read(data, 0, data.Length);
                    bw.Write(data, 0, readLen);
                    //关闭输入流
                    FileIn.Close();
                };
                //关闭二进制写入
                bw.Close();
                FileOut.Close();
            }
            return int.Parse(count) + 1;
        }
    }
}
