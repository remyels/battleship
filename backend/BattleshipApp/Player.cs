using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BattleshipApp
{
    public class Player
    {
        public Board board;
        public string name;
        public string token;
        public string currentShipTBP;
        public int currentShipTBPindex;
        public bool isConnected;
        public bool doneplacement;
        public bool isTurn;
        public int livesLeft;

        protected Dictionary<string, int> nameToLeft;

        protected Dictionary<string, int> nameToSize;

        public Player()
        {
            livesLeft = 0;
            board = new Board();
            nameToLeft = new Dictionary<string, int>();
            nameToSize = new Dictionary<string, int>();
            foreach (Ship.typeToDim shipname in Enum.GetValues(typeof(Ship.typeToDim)))
            {
                nameToSize.Add(shipname.ToString().ToLower(), (int)shipname);
            }
            foreach (FieldInfo f in typeof(Board).GetFields())
            {
                if (f.IsStatic&&f.Name.Contains("_"))
                {
                    nameToLeft.Add(f.Name.Split('_')[1].ToLower(), (int)f.GetValue(null));
                }
            }
            foreach (Ship.typeToDim shipname in Enum.GetValues(typeof(Ship.typeToDim)))
            {
                livesLeft += nameToSize[shipname.ToString().ToLower()] * nameToLeft[shipname.ToString().ToLower()];
            }
        }

        public void askForShips()
        {
            Console.WriteLine("Time to build your board, {0}", name);
            foreach (var item in nameToLeft)
            {
                int temp = item.Value;
                while (temp > 0)
                {
                    Console.WriteLine(String.Format("Enter the starting (r, c) and ending (r, c) for the {0} ship of size {1}", item.Key, nameToSize[item.Key]));
                    int startingX, startingY, endingX, endingY;
                    string[] datastr = Console.ReadLine().Split(' ');
                    int[] data = new int[datastr.Length];
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] = int.Parse(datastr[i]);
                    }
                    startingX = data[0];
                    startingY = data[1];
                    endingX = data[2];
                    endingY = data[3];
                    if (!board.checkVacant(startingX - 1, startingY - 1, endingX - 1, endingY - 1))
                    {
                        throw new ArgumentOutOfRangeException("Occupied cell", "This cell is occupied by another ship.");
                    }
                    Ship ship = new Ship(item.Key, startingX, startingY, endingX, endingY);
                    board.addShip(ship);
                    Console.WriteLine("Your current board looks like the following: ");
                    board.printBoard();
                    temp--;
                }
            }
            Console.WriteLine("Done? Press enter!");
            Console.ReadLine();
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.WriteLine();
            }
        }

        //public bool hasLost()
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        for (int j = 0; j < 10; j++)
        //        {
        //            if (board.placed[i, j])
        //            {
        //                if (!opponent.board.hit[i, j])
        //                {
        //                    return false;
        //                }
        //            }
        //        }
        //    }
        //    return true;
        //}

        //public void printUpdatedBoard()
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        for (int j = 0; j < 10; j++)
        //        {
        //            if (opponent.board.placed[i, j] && board.hit[i, j])
        //            {
        //                Console.Write("\tx");
        //            }
        //            else
        //            {
        //                Console.Write("\t-");
        //            }
        //        }
        //        Console.Write("\n");
        //    }
        //}
    }
}
