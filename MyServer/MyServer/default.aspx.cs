using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyServer
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            //返回请求的内容
            Response.Write("客户端请求的数据内容：");
            //获取post请求传递过来的内容
            System.IO.Stream inputStream = Request.InputStream;
            using (StreamReader reader = new StreamReader(inputStream))
            {
                string body = reader.ReadToEnd();
                Response.Write(body);//将内容输出
            }
            //Get请求获得移动端传来的数据设置请求缓存时间
            if (Request.QueryString["cacheable"] != null)
            {
                //设置缓存时间
                Response.Expires = Int32.Parse(Request.QueryString["cacheable"]);
                Response.Write("Get请求，当前的服务器时间是：");
                Response.Write(DateTime.Now);
                Response.Write("请求缓存"+ Response.Expires +"分钟");
            }
            //获取移动端请求的cookies
            if (Request.Cookies.Count > 0)
            {
                Response.Write("Cookies: ");
                foreach (var cookieKey in Request.Cookies.AllKeys)
                {
                    Response.Write(cookieKey + ":" +Request.Cookies[cookieKey].Value);
                }
            }
            //接收移动端传来的参数设置cookie值
            if (Request.QueryString["setCookies"] != null)
            {
                //创建一个持续3分钟的cookie
                HttpCookie myCookie1 = new HttpCookie("LastVisit");
                DateTime now = DateTime.Now;
                myCookie1.Value = now.ToString();
                myCookie1.Expires = now.AddMinutes(3);
                Response.Cookies.Add(myCookie1);
                //创建一个http会话的cookie
                HttpCookie myCookie2 = new HttpCookie("SID");
                myCookie2.Value = "31d4d96e407aad42";
                myCookie2.HttpOnly = true;
                Response.Cookies.Add(myCookie2);
            }
            //返回流数据
            if (Request.QueryString["extraData"] != null)
            {
                int streamLength = Int32.Parse(Request.QueryString["extraData"]);
                for (int i=0; i<streamLength; i++)
                {
                    Response.Write("@");
                }
            }
        }
    }
}