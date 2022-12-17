using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instant_Run_Off_Voting_System
{
    public class Candidate
    {
        public string name;
        public int votes;
        public bool eliminated;

        //Constructor for convenient object creation 
        //Yes, I'm using big words to make myself look smart lol
        public Candidate (string _name)
        {
            name = _name;
            votes = 0;
            eliminated = false;
        }
    }
}
