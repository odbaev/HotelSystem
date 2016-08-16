using System;
using System.Collections.ObjectModel;


namespace HotelSystem
{
    public class RoomType
    {
        private string name;
        public string Name
        {
            get { return name; }
        }

        private decimal cost;
        public decimal Cost
        {
            get { return cost; }
            set { cost = value != 0 ? Math.Abs(value) : 0.01m; }
        }

        private uint number;
        public uint Number
        {
            get { return number; }
            set
            {
                uint oldNumber = number;

                number = value < 100 ? value : 99;

                if (number > oldNumber)
                {
                    for (int i = 0; i < number - oldNumber; i++)
                    {
                        rooms.Add(new Room());
                    }
                }
                else if (number < oldNumber)
                {
                    for (int i = 0; i < oldNumber - number; i++)
                    {                 
                        rooms.RemoveAt(0);
                    }
                }
            }
        }

        private ObservableCollection<Room> rooms;
        public ObservableCollection<Room> Rooms
        {
            get { return rooms; }
        }

        public RoomType(string typeName, decimal typeCost, uint typeNumber)
        {
            name = typeName;
            cost = typeCost;
            number = typeNumber;

            rooms = new ObservableCollection<Room>();
            for (int i = 0; i < number; i++)
            {
                rooms.Add(new Room());
            }
        }

        public void Update(int day)
        {
            foreach (Room room in rooms)
            {
                room.Update(day);
            }
        }

        public bool ProcessRequest(bool isBooking, Tuple<int, int> date)
        {      
            foreach (Room room in rooms)
            {
                if (room.ProcessRequest(isBooking, date)) return true;
            }

            return false;
        }

        public void Reset()
        {
            foreach (Room room in rooms)
            {
                room.Reset();
            }
        }
    }
}
