using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Shapes;


namespace HotelSystem
{
    public class RoomsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<Rectangle> roomForms = new List<Rectangle>();

            foreach (Room room in (ObservableCollection<Room>)values[0])
            {
                roomForms.Add(room.RoomForm);
            }

            return roomForms;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
