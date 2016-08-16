using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace HotelSystem
{
    public static class ProgramCommands
    {
        private static RoutedUICommand start;
        public static RoutedUICommand Start
        {
            get { return start; }
        }

        static ProgramCommands()
        {
            start = new RoutedUICommand();
        }
    }
}
