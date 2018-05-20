using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace HttpWebRequestDemo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private WebRequest request;
        public MainPage()
        {
            this.InitializeComponent();
            request = HttpWebRequest.Create("http://www.baidu.com");
            request.Method = "GET";
           
        }

        private void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listbox.SelectedItem == null)
            {
                return;
            }
            var template = (RssItem)listbox.SelectedItem;
            //跳转到详情页面，并且传递参数
            Frame.Navigate(typeof(DetailPage), template);
            listbox.SelectedItem = null;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (rssURL.Text != "")
            {
                RssService.GetRssItems(rssURL.Text.ToString().Trim(), async (items) =>
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        listbox.ItemsSource = items;//显示列表
                    });
                }, async (exception) =>
                {
                    //请求出现异常，把异常显示出来
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await new MessageDialog(exception).ShowAsync();
                    });
                }, null);
            }
            else
            {
                await new MessageDialog("请输入RSS地址").ShowAsync();
            }
        }
    }
}
