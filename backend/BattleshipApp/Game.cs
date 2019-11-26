using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipApp
{
    public class Game
    {
        public Player p1;
        public Player p2;

        public Game()
        {
            p1 = new Player();
            p2 = new Player();
            SetShip.player1 = p1;
            SetShip.player2 = p2;
        }

        public void startGame()
        {
            bool p1turn = true;
            while (!over(p1, p2))
            {
                if (p1turn)
                {
                    Console.WriteLine("It is player 1's turn");
                }
                else
                {
                    Console.WriteLine("It is player 2's turn");
                }
                Console.Write("Please select (r, c) where you would like to launch your missile: ");
                string[] rcstr = Console.ReadLine().Split(' ');
                int[] rc = new int[rcstr.Length];
                for (int i = 0; i < rc.Length; i++)
                {
                    rc[i] = int.Parse(rcstr[i]);
                }
                int r = rc[0] - 1, c = rc[1] - 1;
                if (p1turn)
                {
                    if (p1.board.hit[r, c])
                    {
                        Console.WriteLine("You have already launched a missile here!");
                    }
                    else
                    {
                        p1.board.hit[r, c] = true;
                        if (!p2.board.placed[r, c])
                        {
                            Console.WriteLine("You have launched a missile, you didn't hit any targets!");
                        }
                        else
                        {
                            Console.WriteLine("You have hit a missile! You can find the updated board below: ");
                        }
                        //p1.printUpdatedBoard();
                    }
                    p1turn = false;
                }
                else
                {
                    if (p2.board.hit[r, c])
                    {
                        Console.WriteLine("You have already launched a missile here!");
                    }
                    else
                    {
                        p2.board.hit[r, c] = true;
                        if (!p1.board.placed[r, c])
                        {
                            Console.WriteLine("You have launched a missile, you didn't hit any targets!");
                        }
                        else
                        {
                            Console.WriteLine("You have hit a missile! You can find the updated board below: ");
                        }
                        //p2.printUpdatedBoard();
                    }
                    p1turn = true;
                }
                Console.WriteLine("Please press enter to clear the console and move on to the next turn...");
                Console.ReadLine();
            }
            //if (p1.hasLost())
            //{
            //    Console.WriteLine("\n\nPlayer 2 has won!");
            //}
            //else
            //{
            //    Console.WriteLine("\n\nPlayer 1 has won!");
            //}
        }

        public bool over(Player p1, Player p2)
        {
            //return p1.hasLost() || p2.hasLost();
            return true;
        }
    }
}
