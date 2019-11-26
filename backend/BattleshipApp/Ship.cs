using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipApp
{
    public class Ship
    {
        public int startingX;
        public int startingY;
        public int endingX;
        public int endingY;
        public String type;

        public enum typeToDim
        {
            CARRIER = 5,
            BATTLESHIP = 4,
            CRUISER = 3,
            DESTROYER = 2,
        }

        public Ship(String type, int startingX, int startingY, int endingX, int endingY)
        {
            this.startingX = startingX;
            this.startingY = startingY;
            this.endingX = endingX;
            this.endingY = endingY;
            startingX--;
            startingY--;
            endingX--;
            endingY--;
            this.type = type;
            //if (startingX < 0 || startingY < 0 || endingX < 0 || endingY < 0
            //    ||
            //    startingX > 9 || startingY > 9 || endingX > 9 || endingY > 9)
            //{
            //    throw new ArgumentOutOfRangeException("Invalid input", "Coordinates need to be between 1 and 10");
            //}
            //else if (!validForType())
            //{
            //    throw new ArgumentOutOfRangeException("Invalid input", "Given coordinates do not correspond");
            //}
        }

        public static bool validForType(String type, int startingX, int startingY, int endingX, int endingY)
        {
            int size;
            switch (type.ToUpper())
            {
                case "CARRIER":
                    size = (int)typeToDim.CARRIER;
                    break;
                case "BATTLESHIP":
                    size = (int)typeToDim.BATTLESHIP;
                    break;
                case "DESTROYER":
                    size = (int)typeToDim.DESTROYER;
                    break;
                default:
                    size = (int)typeToDim.CRUISER;
                    break;
            }
            return ((startingX == endingX) && (Math.Abs(startingY - endingY) == (size - 1))) || ((startingY == endingY) && (Math.Abs(startingX - endingX) == (size - 1)));
        }


        // This won't be of any pragmatic use as the user will have to select the placement of the ship
        // on the grid itself; this'll only get called if the API is being called on indirectly and not
        // through its intended use
        public static bool inBounds(int startingX, int startingY, int endingX, int endingY)
        {
            return !(startingX < 0 || startingY < 0 || endingX < 0 || endingY < 0
                ||
                startingX > 9 || startingY > 9 || endingX > 9 || endingY > 9);
        }
    }
}
