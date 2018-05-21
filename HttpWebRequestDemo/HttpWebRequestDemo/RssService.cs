using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;
using Windows.Web.Http;
using System.Net;

namespace HttpWebRequestDemo
{
   public class RssService
    {
        public static void GetRssItems(string rssFeed, Action<IEnumerable<RssItem>> onGetRssItemsCompleted = null,
            Action<string> onError = null, Action onFinally = null)
        {
            var request = HttpWebRequest.Create(rssFeed);
            request.Method = "GET";
            request.BeginGetResponse((result) => {//request 请求之后的回调方法
                try
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)result.AsyncState;
                    WebResponse response = httpWebRequest.EndGetResponse(result);
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string content = reader.ReadToEnd();//读取stream
                            //将网络上获取的信息转化为RSS pojo类
                            List<RssItem> ressItems = new List<RssItem>();
                            SyndicationFeed feed = new SyndicationFeed();//用于格式化rss
                            feed.Load(content);
                            foreach (SyndicationItem item in feed.Items)
                            {
                                RssItem rssItem = new RssItem(item.Title.Text, item.Summary.Text, item.PublishedDate.ToString(),
                                    item.Links[0].Uri.AbsoluteUri);
                                ressItems.Add(rssItem);
                            }
                            //通知完成返回事件执行
                            if (onGetRssItemsCompleted != null)//回调函数不为空
                            {
                                onGetRssItemsCompleted(ressItems);
                            }
                        }
                    }

                }
                catch (WebException webException)
                {
                    string exceptionInfo = "";
                    switch (webException.Status)
                    {
                        case WebExceptionStatus.ConnectFailure://访问失败
                            {
                                exceptionInfo = "ConnectionFailure:远程服务器连接失败.";
                                break;
                            }
                        case WebExceptionStatus.MessageLengthLimitExceeded:
                            {
                                exceptionInfo = "MessageLengthLimitExceeded: 网络请求消息长度受到限制";
                                break;
                            }
                        case WebExceptionStatus.Pending:
                            {
                                exceptionInfo = "Pending: 内部异步请求挂起.";
                                break;
                            }
                        case WebExceptionStatus.RequestCanceled:
                            {
                                exceptionInfo = "RequestCanceled: 该请求被取消.";
                                break;
                            }
                        case WebExceptionStatus.SendFailure:
                            {
                                exceptionInfo = "SendFailure: 发送失败.";
                                break;
                            }
                        case WebExceptionStatus.UnknownError:
                            {
                                exceptionInfo = "UnknownError: 未知错误.";
                                break;
                            }
                        case WebExceptionStatus.Success:
                            {
                                exceptionInfo = "Success:请求成功.";
                                break;
                            }
                        default:
                            {
                                exceptionInfo = "未知网络异常";
                                break;
                            }
                    }
                    if (onError != null)//错误回调方法
                    {
                        onError(exceptionInfo);
                    }
                }
                finally
                {
                    if (onFinally != null)
                    {
                        onFinally();
                    }
                }
            },request);
        }
    }
}
