using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Example11_5
{
    public class HoursToDayStringConverter : IValueConverter
    {

        //由数据源到目标对象
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (Int16.Parse(value.ToString())<12)
            {
                return "尊敬的用户，上午好.";
            }else if (Int16.Parse(value.ToString().Trim())>12)
            {
                return "尊敬的用户，下午好.";
            }else
            {
                return "尊敬的用户，中午好.";
            }
        }

        //由目标对象到数据源
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DateTime.Now.Hour;
        }
    }
}
