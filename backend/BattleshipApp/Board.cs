using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BattleshipApp
{
    public class Board
    {
        public static int numtypeships = 4;

        public static int num_carrier = 1;
        public static int num_battleship = 2;
        public static int num_cruiser = 7;
        public static int num_destroyer = 5;

        public int num_carrier_placed = 0;
        public int num_battleship_placed = 0;
        public int num_cruiser_placed = 0;
        public int num_destroyer_placed = 0;

        public bool[,] hit;
        public bool[,] placed;

        public Board()
        {
            hit = new bool[10, 10];
            placed = new bool[10, 10];
        }

        public bool checkVacant(int startingX, int startingY, int endingX, int endingY)
        {
            for (int i = Math.Min(startingX, endingX); i <= Math.Max(startingX, endingX); i++)
            {
                for (int j = Math.Min(startingY, endingY); j <= Math.Max(startingY, endingY); j++)
                {
                    //Console.Write("Checking {0}, {1}", i, j);
                    if (placed[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void addShip(Ship ship)
        {
            for (int i = Math.Min(ship.startingX, ship.endingX); i <= Math.Max(ship.startingX, ship.endingX); i++)
            {
                for (int j = Math.Min(ship.startingY, ship.endingY); j <= Math.Max(ship.startingY, ship.endingY); j++)
                {
                    placed[i, j] = true;
                }
            }
            increaseShipOfType(ship.type);
        }

        public void increaseShipOfType(string type)
        {
            foreach (FieldInfo f in typeof(Board).GetFields())
            {
                if (!f.IsStatic)
                {
                    if (string.Compare(f.Name.Split('_')[1], type) == 0)
                    {
                        f.SetValue(this, (int)f.GetValue(this) + 1);
                        return;
                    }
                }
            }
        }

        public void printBoard()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (placed[i, j])
                    {
                        Console.Write("\to");
                    }
                    else
                    {
                        Console.Write("\t-");
                    }
                }
                Console.Write("\n");
            }
        }

        public bool checkFull(string type)
        {
            int total = -1, current = -1;
            foreach (FieldInfo f in typeof(Board).GetFields())
            {
                if (f.IsStatic&&f.Name.Contains('_'))
                {
                    if (string.Compare(f.Name.Split('_')[1], type)==0)
                    {
                        total = (int)f.GetValue(null);
                        break;
                    }
                }
            }
            foreach (FieldInfo f in typeof(Board).GetFields())
            {
                if (!f.IsStatic)
                {
                    if (string.Compare(f.Name.Split('_')[1], type) == 0)
                    {
                        current = (int)f.GetValue(this);
                        break;
                    }
                }
            }

            return current == total;
        }

        public static string[] getShips()
        {
            string[] ships = new string[numtypeships];
            int curind = 0;
            foreach (FieldInfo f in typeof(Board).GetFields())
            {
                if (f.IsStatic&&f.Name.Contains("_"))
                {
                    ships[curind] = f.Name.Split('_')[1];
                    curind++;
                }
            }
            return ships;
        }
    }

}