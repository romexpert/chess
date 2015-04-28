using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class DefaultChessmen
    {
        private static List<Chessman> _chessmen = new List<Chessman>();

        static DefaultChessmen()
        {
            _chessmen.Add(new Chessman(new Point(0, 0), FeagureType.Rook, Color.Black));
            _chessmen.Add(new Chessman(new Point(1, 0), FeagureType.Knight, Color.Black));
            _chessmen.Add(new Chessman(new Point(2, 0), FeagureType.Bishop, Color.Black));
            _chessmen.Add(new Chessman(new Point(3, 0), FeagureType.Queen, Color.Black));
            _chessmen.Add(new Chessman(new Point(4, 0), FeagureType.King, Color.Black));
            _chessmen.Add(new Chessman(new Point(5, 0), FeagureType.Bishop, Color.Black));
            _chessmen.Add(new Chessman(new Point(6, 0), FeagureType.Knight, Color.Black));
            _chessmen.Add(new Chessman(new Point(7, 0), FeagureType.Rook, Color.Black));
            
            for (byte i = 0; i < 8; i++)
            {
                _chessmen.Add(new Chessman(new Point(i, 1), FeagureType.Pawn, Color.Black));
                _chessmen.Add(new Chessman(new Point(i, 6), FeagureType.Pawn, Color.White));
            }

            _chessmen.Add(new Chessman(new Point(0, 7), FeagureType.Rook, Color.White));
            _chessmen.Add(new Chessman(new Point(1, 7), FeagureType.Knight, Color.White));
            _chessmen.Add(new Chessman(new Point(2, 7), FeagureType.Bishop, Color.White));
            _chessmen.Add(new Chessman(new Point(3, 7), FeagureType.Queen, Color.White));
            _chessmen.Add(new Chessman(new Point(4, 7), FeagureType.King, Color.White));
            _chessmen.Add(new Chessman(new Point(5, 7), FeagureType.Bishop, Color.White));
            _chessmen.Add(new Chessman(new Point(6, 7), FeagureType.Knight, Color.White));
            _chessmen.Add(new Chessman(new Point(7, 7), FeagureType.Rook, Color.White));
        }

        public static List<Chessman> Items { get { return _chessmen; } }
    }
}
