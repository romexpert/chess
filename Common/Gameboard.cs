using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Point = System.Drawing.Point;

namespace Common
{
    static class Gameboard
    {
        internal static byte BoardSize = 8;
        private static Dictionary<Point, Chessman> _board = new Dictionary<Point, Chessman>();

        public static void AddFeagure(Point point, Chessman chessman)
        {
            _board.Add(point, chessman);
        }

        public static void MoveFeagure(Point newPosition, Chessman chessman)
        {
            RemoveFeagure(chessman);

            Chessman enemy;
            if (_board.TryGetValue(newPosition, out enemy))
            {
                if(enemy.Color == chessman.Color)
                    throw new Exception("Cannibalism");
                
                _board.Remove(newPosition);
            }

            AddFeagure(newPosition, chessman);
        }

        public static void RemoveFeagure(Chessman chessman)
        {
            var entry = _board.First(_ => _.Value == chessman);
            _board.Remove(entry.Key);
        }

        public static Color Busy(Point position)
        {
            Chessman chessman;
            if (_board.TryGetValue(position, out chessman))
            {
                 return _board[position].Color;
            }

            return Color.None;
        }

        public static FeagureType BusyType(Point position)
        {
            return _board[position].Type;
        }

        /// <summary>
        /// Determine if some enemy can hit this position
        /// </summary>
        /// <param name="point">Position of current feagure</param>
        /// <param name="color">Color of current feagure</param>
        /// <returns></returns>
        public static bool IsKingBecamaUnderAttack(Point point, Color color)
        {
            var enemies = _board.Values.Where(_ => _.Color != color).Where(_ => _.CanHit(point)).ToList();

            foreach (var enemy in  enemies)
            {
                Debug.WriteLine("{0} {1}", enemy.Color, enemy.Type);
            }

            return enemies.Any();
        }

        public static Chessman GetKingAttacker(Color color, Point? exceptPoint, Chessman chessman, bool showMessage = true)
        {
            var king = _board.Values.First(_ => _.Type == FeagureType.King && _.Color == color);

            var added = false;
            Chessman removedChessman = null;
            
            //TODO
            if (chessman != null)
                _board.Remove(chessman.CurrentPosition);
            if (exceptPoint.HasValue && chessman != null)
            {
                if (_board.ContainsKey(exceptPoint.Value))
                {
                    removedChessman = _board[exceptPoint.Value];
                    _board.Remove(exceptPoint.Value);
                }

                _board.Add(exceptPoint.Value, chessman);
                added = true;
            }
            
            var enemies = _board.Values.Where(_ => _.Color != color && _.CurrentPosition != exceptPoint).Where(_ => _.CanHit(king.CurrentPosition)).ToList();

            foreach (var enemy in enemies)
            {
                Debug.WriteLine("{0} {1}", enemy.Color, enemy.Type);
            }

            if (added)
                _board.Remove(exceptPoint.Value);
            if (chessman != null)
                _board.Add(chessman.CurrentPosition, chessman);
            if (removedChessman != null)
                _board.Add(exceptPoint.Value, removedChessman);

            if (enemies.Any())
            {
                if (enemies.Count > 1)
                    throw new FatalityException { WinColor = ColorHelper.GetOpposite(color) };

                if (showMessage)
                    MessageBox.Show("Your king need to run!");
                return enemies.First();
            }

            return null;
        }

        public static bool IsKingUnderAttack(Color color, Point? exceptPoint, Chessman chessman, bool showMessage = true)
        {
            var attacker = GetKingAttacker(color, exceptPoint, chessman, showMessage);

            return attacker != null;
        }

        public static bool IsProtectedPoint(Point point)
        {
            var chessman = _board[point];

            _board.Remove(point);

            var protectedPoint = IsKingBecamaUnderAttack(point, ColorHelper.GetOpposite(chessman.Color));

            _board.Add(point, chessman);

            return protectedPoint;
        }

        public static bool CheckFatality(Color initiator)
        {
            var enemyKing = _board.Values.Single(_ => _.Color == ColorHelper.GetOpposite(initiator) && _.Type == FeagureType.King);

            var attacker = GetKingAttacker(enemyKing.Color, null, null, false);
            if (attacker != null)
            {
                //King cannot run
                if (!enemyKing.CanEscape())
                {
                    //Is somebody can kill attacker
                    var saviors =
                        _board.Values.Where(
                            _ =>
                                _.Color == enemyKing.Color && _.Type != FeagureType.King &&
                                _.CanHit(attacker.CurrentPosition)).ToList();

                    saviors = saviors.Where(_ => !IsKingUnderAttack(enemyKing.Color, attacker.CurrentPosition, _, false)).ToList();

                    Debug.WriteLine("*****BEGING Saviors ******");

                    foreach (var savior in saviors)
                    {
                        Debug.WriteLine("{0} {1}", savior.Type, savior.CurrentPosition);
                    }

                    Debug.WriteLine("*****END Saviors ******");

                    if (saviors.Any())
                        return false;

                    IEnumerable<Point> safePoints = new List<Point>();
                    //Is somebody can saмe king
                    if (attacker.CurrentPosition.X == enemyKing.CurrentPosition.X)
                    {
                        int start = attacker.CurrentPosition.Y, end = enemyKing.CurrentPosition.Y;
                        if (start > end)
                        {
                            var temp = end;
                            end = start;
                            start = temp;
                        }

                        for (var i = start + 1; i < end; i++)
                        {
                            if (SomebodyCanSave(enemyKing, new Point(attacker.CurrentPosition.X, i)))
                                return false;
                        }

                    } 
                    else if (attacker.CurrentPosition.Y == enemyKing.CurrentPosition.Y)
                    {
                        int start = attacker.CurrentPosition.X, end = enemyKing.CurrentPosition.X;
                        if (start > end)
                        {
                            var temp = end;
                            end = start;
                            start = temp;
                        }

                        for (var i = start + 1; i < end; i++)
                        {
                            if (SomebodyCanSave(enemyKing, new Point(i, attacker.CurrentPosition.Y)))
                                return false;
                        }

                    }
                    else
                    {
                        int startX = attacker.CurrentPosition.X, endX = enemyKing.CurrentPosition.X;
                        if (startX > endX)
                        {
                            var temp = endX;
                            endX = startX;
                            startX = temp;
                        }

                        int startY = attacker.CurrentPosition.Y, endY = enemyKing.CurrentPosition.Y;
                        if (startY > endY)
                        {
                            var temp = endY;
                            endY = startY;
                            startY = temp;
                        }

                        for (int x = startX + 1, y = startY + 1; x < endX; x++, y++)
                        {
                            if (SomebodyCanSave(enemyKing, new Point(x, y)))
                                return false;
                        }

                        return true;
                    }
                    Trace.TraceWarning("not implemented");
                }
            }

            return false;
        }

        static bool SomebodyCanSave(Chessman king, Point safaPoint)
        {
            var saviors =
                        _board.Values.Where(
                            _ =>
                                _.Color == king.Color && _.Type != FeagureType.King &&
                                _.CanGo(safaPoint)).ToList();

            saviors = saviors.Where(_ => !IsKingUnderAttack(king.Color, safaPoint, _, false)).ToList();

            Debug.WriteLine("*****BEGING Saviors ******");

            foreach (var savior in saviors)
            {
                Debug.WriteLine("{0} {1}", savior.Type, savior.CurrentPosition);
            }

            Debug.WriteLine("*****END Saviors ******");

            return saviors.Any();
        }
    }
}