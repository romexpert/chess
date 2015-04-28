using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Game
    {
        private Color _turn;

        Game()
        {
            _turn = Color.White;
        }

        public bool IsTurn(Color color)
        {
            return color == _turn;
        }

        public void ChangeTurn()
        {
            _turn = _turn == Color.White ? Color.Black : Color.White;
        }

        private static Game _currentGame;
        public static Game Current
        {
            get
            {
                return _currentGame ?? (_currentGame = new Game());
            }
        }
    }
}