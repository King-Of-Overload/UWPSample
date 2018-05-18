using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Examples_11_6
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //public List<Girl> allGirls { get; set; }

        private ObservableCollection<Girl> girls;
        public MainPage()
        {
            this.InitializeComponent();
            /*allGirls = new List<Girl>();
            Girl girl1 = new Girl() { Name = "小缘", Avatar = "Images/xiaoyuan.jpg", Description = "大毛" };
            Girl girl2 = new Girl() { Name = "冯提莫", Avatar = "Images/ftm.jpg", Description = "大学老师" };
            Girl girl3 = new Girl() { Name = "小6想去月球", Avatar = "Images/小六.jpg", Description = "小6" };
            Girl girl4 = new Girl() { Name = "PC冷冷", Avatar = "Images/lengleng.jpg", Description = "冯冯" };
            Girl girl5 = new Girl() { Name = "小缘", Avatar = "Images/xiaoyuan.jpg", Description = "大毛" };
            Girl girl6 = new Girl() { Name = "冯提莫", Avatar = "Images/ftm.jpg", Description = "大学老师" };
            Girl girl7 = new Girl() { Name = "小6想去月球", Avatar = "Images/小六.jpg", Description = "小6" };
            Girl girl8 = new Girl() { Name = "PC冷冷", Avatar = "Images/lengleng.jpg", Description = "冯冯" };
            allGirls.Add(girl1);
            allGirls.Add(girl2);
            allGirls.Add(girl3);
            allGirls.Add(girl4);
            allGirls.Add(girl5);
            allGirls.Add(girl6);
            allGirls.Add(girl7);
            allGirls.Add(girl8);
            listBox.ItemsSource = allGirls;//将集合赋值给ListView的ItemsSource*/

            girls = new ObservableCollection<Girl>();
            Girl girl1 = new Girl() { Name = "小缘", Avatar = "Images/xiaoyuan.jpg", Description = "大毛" };
            Girl girl2 = new Girl() { Name = "冯提莫", Avatar = "Images/ftm.jpg", Description = "大学老师" };
            Girl girl3 = new Girl() { Name = "小6想去月球", Avatar = "Images/小六.jpg", Description = "小6" };
            Girl girl4 = new Girl() { Name = "PC冷冷", Avatar = "Images/lengleng.jpg", Description = "冯冯" };
            girls.Add(girl1);
            girls.Add(girl2);
            girls.Add(girl3);
            girls.Add(girl4);
            listBox.ItemsSource = girls;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            girls.Add(new Girl() { Name = "小6想去月球", Avatar = "Images/小六.jpg", Description = "小6" });
        }

        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                Girl girl = listBox.SelectedItem as Girl;
                if (girls.Contains(girl))
                {
                    girls.Remove(girl);
                }
            }
        }
    }
}
