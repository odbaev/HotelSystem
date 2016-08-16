using System;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;


namespace HotelSystem
{
    public class Model : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private uint period;
        public uint Period
        {
            get { return period; }
            set { period = value; }
        }

        private int totalRequests;
        public int TotalRequests
        {
            get { return totalRequests; }
            private set
            {
                totalRequests = value;
                NotifyPropertyChanged();
            }
        }

        private int doneRequests;
        public int DoneRequests
        {
            get { return doneRequests; }
            private set
            {
                doneRequests = value;
                NotifyPropertyChanged();
            }
        }

        private decimal profit;
        public decimal Profit
        {
            get { return profit; }
            private set
            {
                profit = value;
                NotifyPropertyChanged();
            }
        }

        private double[] occupancyPercentage;
        public double[] OccupancyPercentage
        {
            get { return occupancyPercentage; }
        }

        private int msHour;

        public Model()
        {
            period = 12;
            msHour = 500;

            totalRequests = 0;
            doneRequests = 0;
            profit = 0;
        }

        private void Reset(RoomType[] roomTypes)
        {
            foreach (RoomType roomType in roomTypes)
            {
                roomType.Reset();
            }

            TotalRequests = 0;
            DoneRequests = 0;
            Profit = 0;

            occupancyPercentage = new double[roomTypes.Length];
        }

        private double[] MakeDist(RoomType[] roomTypes)
        {
            double[] dist = new double[roomTypes.Length];
            decimal sum = roomTypes.Sum(x => x.Cost);

            for (int i = 0; i < dist.Length - 1; i++)
            {
                dist[i] = (double)((sum - roomTypes[i].Cost) / sum / (roomTypes.Length - 1));
            }

            for (int i = 1; i < dist.Length - 1; i++)
            {
                dist[i] += dist[i - 1];
            }
            dist[dist.Length - 1] = 1;

            return dist;
        }

        private void UpdateOccupiedNumber(int[] occupied, RoomType[] roomTypes)
        {
            for (int i = 0; i < occupied.Length; i++)
            {
                occupied[i] += roomTypes[i].Rooms.Count(x => !x.IsVacant);
            }
        }

        private void SetOccupancyPercentage(int[] occupied, RoomType[] roomTypes, int day)
        {
            for (int i = 0; i < occupancyPercentage.Length; i++)
            {          
                occupancyPercentage[i] = (double)occupied[i] / ((int)roomTypes[i].Number * day) * 100;
            }
        }

        public void Start(RoomType[] roomTypes, BackgroundWorker backgroundWorker)
        {
            Reset(roomTypes);

            int[] occupied = new int[roomTypes.Length];
            
            double[] dist = MakeDist(roomTypes);
            double d;

            Random rand = new Random();

            int day = 1, hours = 0, wait;
            int type;
            int date1, date2;
            bool isBooking;
            bool isDoneRequest;

            while (day <= period)
            {
                if (backgroundWorker.CancellationPending)
                {
                    UpdateOccupiedNumber(occupied, roomTypes);
                    SetOccupancyPercentage(occupied, roomTypes, day);
                    return;
                }

                wait = rand.Next(1, 6);
                Thread.Sleep(wait * msHour);

                hours += wait;
                if (hours >= 24)
                {
                    hours -= 24;
                    day++;

                    UpdateOccupiedNumber(occupied, roomTypes);

                    backgroundWorker.ReportProgress(day);

                    foreach (RoomType roomType in roomTypes)
                    {
                        roomType.Update(day);
                    }

                    if (day > period)
                    {
                        SetOccupancyPercentage(occupied, roomTypes, day - 1);
                        break;
                    }
                }

                TotalRequests++;

                isBooking = rand.Next(0, 2) == 1;

                d = rand.NextDouble();
                type = Array.FindIndex(dist, x => x > d);

                if (isBooking)
                {
                    if (day == period) continue;

                    date1 = rand.Next(day + 1, (int)period + 1);
                }
                else
                {
                    date1 = day;
                }
                date2 = rand.Next(date1, (int)period + 1);

                isDoneRequest = roomTypes[type].ProcessRequest(isBooking, new Tuple<int, int>(date1, date2));

                if (isDoneRequest)
                {
                    DoneRequests++;

                    Profit += roomTypes[type].Cost * (date2 - date1 + 1);
                }
            }
        }
    }
}
