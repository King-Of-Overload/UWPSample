using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace VisitMyServer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string server = "http://192.168.1.101:8080/UWPServer/MyServer";

        private HttpClient httpClient;//http请求对象
        private CancellationTokenSource cts;//取消对象
        public MainPage()
        {
            this.InitializeComponent();
            httpClient = new HttpClient();
            cts = new CancellationTokenSource();
        }

        //请求处理方法
        private async void HttpRequestAsync(Func<Task<string>> httpRequestFuncAsync)
        {
            string responseBody;
            waiting.Visibility = Visibility.Visible;
            try
            {
                responseBody = await httpRequestFuncAsync();
                cts.Token.ThrowIfCancellationRequested();
            }
            catch (TaskCanceledException)
            {
                responseBody = "请求被取消";
            }
            catch (Exception ex)
            {
                responseBody = "异常信息" + ex.Message;
            }
            finally
            {
                waiting.Visibility = Visibility.Collapsed;
            }
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await new MessageDialog(responseBody).ShowAsync();
            });
        }

        //Get请求获取string 类型数据
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
             HttpRequestAsync(async () => {
                string resourceAddress = server + "?cacheable=1";
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(resourceAddress));
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            });
        }

        //Get请求获取Stream类型数据
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            HttpRequestAsync(async () => {
                string resourceAddress = server + "extraData=2000";
                StringBuilder responseBody = new StringBuilder();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(resourceAddress));
                HttpResponseMessage response = await httpClient.SendRequestAsync(request, HttpCompletionOption.ResponseHeadersRead);
                using (Stream responseStream = (await response.Content.ReadAsInputStreamAsync()).AsStreamForRead())
                {
                    int read = 0;
                    byte[] responseBytes = new byte[1024];
                    do
                    {
                        read = await responseStream.ReadAsync(responseBytes, 0, responseBytes.Length);
                        responseBody.AppendFormat("Bytes read from stream :{0}", read);
                        responseBody.AppendLine();
                        //把byte[] 转化为Ibuffer类型
                        IBuffer responseBuffer = CryptographicBuffer.CreateFromByteArray(responseBytes);
                        responseBuffer.Length = (uint)read;
                        //转化为Hex字符串
                        responseBody.AppendFormat(CryptographicBuffer.EncodeToHexString(responseBuffer));
                        responseBody.AppendLine();
                    } while (read != 0);
                }
                    return responseBody.ToString();
            });
        }
        
        //Post请求发送String类型数据
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            HttpRequestAsync(async () => {
                string resourceAddress = server;
                string responseBody;
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(resourceAddress),
                    new Windows.Web.Http.HttpStringContent("hello UWP")).AsTask(cts.Token);
                responseBody = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
                return responseBody;
            });
        }

        //Post请求发送Stream类型的数据
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            HttpRequestAsync(async ()=> {
                string resourceAddress = server;
                string responseBody;
                const int contentLength = 1000;
                //使用Stream初始化一个HttpStreamContent对象
                Stream stream = GenerateSampleStream(contentLength);
                HttpStreamContent streamContent = new HttpStreamContent(stream.AsInputStream());
                //初始化一个post类型的HttpRequestMessage对象
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(resourceAddress));
                request.Content = streamContent;//添加stream
                //发送post请求
                HttpResponseMessage response = await httpClient.SendRequestAsync(request).AsTask(cts.Token);
                //获取请求结果
                responseBody = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
                return responseBody;
            });
        }

        private static MemoryStream GenerateSampleStream(int size)
        {
            byte[] subData = new byte[size];
            for (int i=0; i<subData.Length; i++)
            {
                //ASCII 40表示  (
                subData[i] = 40;
            }
            return new MemoryStream(subData);
        }

        //监控post请求的进度
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            HttpRequestAsync(async ()=> {
                string resourceAddress = server;
                string responseBody;
                const uint streamLength = 10000000;
                Stream stream = GenerateBigStream(streamLength);
                HttpStreamContent streamContent = new HttpStreamContent(stream.AsInputStream());
                streamContent.Headers.ContentLength = streamLength;
                //创建进度对象
                IProgress<HttpProgress> progress = new Progress<HttpProgress>(ProgressHandler);
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(resourceAddress), streamContent).AsTask(cts.Token);
                responseBody = "完成";
                return responseBody;
            });
        }

        private static MemoryStream GenerateBigStream(uint size)
        {
            byte[] subData = new byte[size];
            for (int i = 0; i < subData.Length; i++)
            {
                //ASCII 40表示  (
                subData[i] = 40;
            }
            return new MemoryStream(subData);
        }

        private void ProgressHandler(HttpProgress progress)
        {
            string infoString = "";
            infoString = progress.Stage.ToString();
            //需要发送的数据
            ulong totalBytesToSend = 0;
            if (progress.TotalBytesToSend.HasValue)
            {
                totalBytesToSend = progress.TotalBytesToSend.Value;
                infoString += "发送数据:" + totalBytesToSend.ToString(CultureInfo.InvariantCulture);
            }
            //已接收的数据
            ulong totalBytesToReceive = 0;
            if (progress.TotalBytesToReceive.HasValue)
            {
                totalBytesToReceive = progress.TotalBytesToReceive.Value;
                infoString += "接收数据:" + totalBytesToReceive.ToString(CultureInfo.InvariantCulture);
            }
            //进度
            double requestProgress = 0;
            //前面50%为发送进度，后50%为接收进度
            if (progress.Stage == HttpProgressStage.SendingContent && totalBytesToSend > 0)
            {
                requestProgress = progress.BytesSent * 50 / totalBytesToSend;
                infoString += "发送进度:";
            }else if (progress.Stage == HttpProgressStage.ReceivingContent)
            {
                requestProgress += 50;
                if (totalBytesToReceive > 0)
                {
                    requestProgress += progress.BytesReceived * 50 / totalBytesToReceive;
                }
                infoString += "接收进度：";
            }else
            {
                return;
            }
            infoString += requestProgress;
            information.Text = infoString;
        }

        //设置网络请求的Cookie
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            HttpRequestAsync(async ()=> {
                string resourceAddress = server;
                string responseBody;
                //创建一个HttpCookie对象， id为cookie名称 localhost主机名 /为服务器虚拟路径
                HttpCookie cookie = new HttpCookie("id", "192.168.1.101", "/");
                cookie.Value = "zjut";
                //将过期设置为null表示只在一个会话里面失效
                cookie.Expires = null;
                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();//端口过滤器
                bool replaced = filter.CookieManager.SetCookie(cookie, false);
                HttpResponseMessage response = await httpClient.PostAsync(new Uri(resourceAddress),
                    new HttpStringContent("hello UWP")).AsTask(cts.Token);
                responseBody = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
                return responseBody;
            });
        }

        //获取网络请求的cookie
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HttpRequestAsync(async () => {
                string resourceAddress = server + "?setCookies = 1";
                string responseBody;
                //发送网络请求
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(resourceAddress)).AsTask(cts.Token);
                responseBody = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
                //获取基础协议筛选器
                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                //获取网络请求下载到的Cookie数据
                HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(new Uri(resourceAddress));
                //遍历
                responseBody = cookieCollection.Count + "cookies:";
                foreach (HttpCookie cookie in cookieCollection)
                {
                    responseBody += "Name: " + cookie.Name + " ";
                    responseBody += "Domain: " + cookie.Domain + " ";
                    responseBody += "Path: " + cookie.Path + " ";
                    responseBody += "Value: " + cookie.Value + "";
                    responseBody += "Expires: " + cookie.Expires + "";
                    responseBody += "Secure: " + cookie.Secure + "";
                    responseBody += "HttpOnly: " + cookie.HttpOnly + "";
                }
                return responseBody;
            });
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
            cts.Dispose();
            cts = new CancellationTokenSource();
        }
    }
}
