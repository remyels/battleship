using System;
using System.Collections.Generic;
using System.Text;

namespace BattleshipApp
{
    class State
    {
        public Game game;
        public string message;

        public State(Game game, string message)
        {
            this.game = game;
            this.message = message;
        }
    }
}
