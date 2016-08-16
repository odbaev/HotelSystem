using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;


namespace HotelSystem
{
    public class Room
    {
        private Rectangle roomForm;
        public Rectangle RoomForm
        {
            get { return roomForm; }
        }

        private SolidColorBrush vacantColor = new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0));
        private SolidColorBrush occupiedColor = new SolidColorBrush(Color.FromRgb(0xA0, 0xA0, 0xA0));

        private bool isVacant;
        public bool IsVacant
        {
            get { return isVacant; }
        }

        private List<Tuple<int, int>> dates = new List<Tuple<int, int>>();

        public Room()
        {
            roomForm = new Rectangle()
            {
                Height = 20,
                Width = 20,
                Fill = vacantColor,
                Stroke = Brushes.Black,
                RadiusX = 5,
                RadiusY = 5,
                Margin = new Thickness(0, 0, 10, 0),
                ToolTip = "свободно"
            };
            ToolTipService.SetShowDuration(roomForm, 7000);

            roomForm.ToolTipOpening += roomForm_ToolTipOpening;

            isVacant = true;
        }

        private string MakeDate(int day1, int day2)
        {
            if (day1 == day2)
            {
                return day1 + "-й день";
            }

            return day1 + "-й - " + day2 + "-й день";
        }

        private void roomForm_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (dates.Count == 0)
            {
                roomForm.ToolTip = "свободно";
                return;
            }

            string info = isVacant ? "бронь: " : "занято: ";

            info += MakeDate(dates[0].Item1, dates[0].Item2);

            for (int i = 1; i < dates.Count; i++)
            {
                info += "\nбронь: " + MakeDate(dates[i].Item1, dates[i].Item2);
            }

            roomForm.ToolTip = info;
        }

        private void RoomFormFill(SolidColorBrush color)
        {
            roomForm.Dispatcher.BeginInvoke((ThreadStart)delegate()
                { roomForm.Fill = color; });
        }

        public void Update(int day)
        {
            if (dates.Count == 0) return;

            bool changedVacant = false;

            if (dates[0].Item2 < day)
            {
                dates.RemoveAt(0);
                changedVacant = !changedVacant;
            }

            if (dates.Count != 0 && dates[0].Item1 == day)
            {
                changedVacant = !changedVacant;
            }

            if (changedVacant)
            {
                isVacant = !isVacant;

                if (isVacant)
                {
                    RoomFormFill(vacantColor);
                }
                else
                {
                    RoomFormFill(occupiedColor);
                }
            }
        }

        public bool ProcessRequest(bool isBooking, Tuple<int, int> date)
        {
            if (dates.Count == 0)
            {
                dates.Add(date);

                if (!isBooking)
                {
                    isVacant = false;
                    RoomFormFill(occupiedColor);
                }
                return true;
            }

            int idx = dates.FindIndex(x => x.Item1 > date.Item1);

            if (idx != -1)
            {
                if ((idx == 0 || dates[idx - 1].Item2 < date.Item1) && dates[idx].Item1 > date.Item2)
                {
                    dates.Insert(idx, date);

                    if (!isBooking && idx == 0)
                    {
                        isVacant = false;
                        RoomFormFill(occupiedColor);
                    }
                    return true;
                }
            }
            else
            {
                if (dates[dates.Count - 1].Item2 < date.Item1)
                {
                    dates.Add(date);
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            if (!isVacant)
            {
                RoomFormFill(vacantColor);
                isVacant = true;
            }

            dates.Clear();
        }
    }
}
