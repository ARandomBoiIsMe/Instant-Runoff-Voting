using System;
using System.Collections.Generic;
using System.Linq;

namespace Instant_Run_Off_Voting_System
{
    class Program
    {
        //Made the voters variable global to allow reuse in multiple functions
        static int voters;

        static void Main(string[] args)
        {
            //Makes list of all candidates
            Candidate[] candidates = CreateCandidateArray();

            //Collects all votes and ranks them
            Candidate[,] votes = CreateVoteArray(candidates);

            //Compares votes and generates winner
            CompareVotes(votes, candidates);

            Console.ReadLine();
        }
        
        //Creates array of candidates that will participate in the election
        static Candidate[] CreateCandidateArray()
        {
            string[] candidateString;
            do
            {
                Console.Write("Enter candidates' names (Separate with spaces): ");
                candidateString = Console.ReadLine().Split(' ');
            } while (CheckCandidates(candidateString) == false);

            //Initializes values of all candidates
            int len = candidateString.Length;
            Candidate[] candidates = new Candidate[len];
            for (int i = 0; i < len; i++)
                candidates[i] = new Candidate(candidateString[i]);
            
            return candidates;
        }

        //Creates array of voter's choices
        static Candidate[,] CreateVoteArray(Candidate[] candidates)
        {
            Console.Write("Number of voters: ");
            voters = Convert.ToInt32(Console.ReadLine());
            Candidate[,] votes = new Candidate[voters, candidates.Length];

            Console.WriteLine("-----------------------" +
                "\nCast your votes." +
                "\nType in your votes in order of preference, separated by spaces." +
                "\nDo not type in the same name twice." +
                "\nYou must cast a vote for each candidate." +
                "\n-----------------------");

            for (int i = 0; i < voters; i++)
            {
                Console.Write($"Voter #{i + 1}: ");
                Candidate[] voterPreferences = VoterPreferenceList(i, candidates);
                CopyArrayToRow(voterPreferences, votes, i);
            }

            return votes;
        }

        //Generates winner from votes
        static void CompareVotes(Candidate[,] votes, Candidate[] candidates)
        {
            //Counts initial votes
            for (int i = 0; i < votes.GetLength(0); i++)
                votes[i, 0].votes++;
            
            //Loops until a winner is found or until a tie is declared
            while (FoundWinner(votes.Length, candidates) == false)
            {
                Eliminate(votes.Length, candidates);
                TransferVotes(votes);
            }
        }

        //Generates list of voter's choices
        static Candidate[] VoterPreferenceList(int i, Candidate[] candidates)
        {
            string[] votes = Console.ReadLine().Split(' ');

            //Ensures voter submitted their votes correctly
            while (CheckVotes(votes, candidates) == false)
            {
                Console.Write($"Voter #{i + 1}: ");
                votes = Console.ReadLine().Split(' ');
            }

            //Populates output array with ordered votes
            Candidate[] output = new Candidate[votes.Length];
            for (int num = 0; num < votes.Length; num++)
                output[num] = candidates.Where(x => x.name == votes[num]).Single();

            return output;
        }

        //Sees if any candidate fits the winning conditions
        static bool FoundWinner(int len, Candidate[] candidates)
        {
            if (isTie(len, candidates) == true)
                return true;

            if (MaxVotes(candidates) <= Math.Ceiling(voters / 2.0))
                return false;

            Console.WriteLine("-----------------------");
            Console.WriteLine($"The winner is " +
                $"{candidates.Where(x => x.votes == MaxVotes(candidates)).Single().name}");
            return true;
        }

        //Checks if a tie exists between candidates
        static bool isTie(int len, Candidate[] candidates)
        {
            int min = MinVotes(len, candidates);

            foreach (Candidate candidate in candidates)
            {
                if (candidate.votes != min && candidate.eliminated == false)
                    return false;
            }

            Console.WriteLine("-----------------------");
            Console.Write("Tie declared: ");
            foreach (Candidate candidate in candidates)
            {
                if (candidate.eliminated == false)
                    Console.Write($"{candidate.name} ");
            }

            return true;
        }

        //Finds the current highest number of votes
        static int MaxVotes(Candidate[] candidates)
        {
            int max = 0;

            foreach  (Candidate candidate in candidates)
            {
                if (candidate.votes > max && candidate.eliminated == false)
                    max = candidate.votes;
            }

            return max;
        }

        //Eliminates all candidates with the lowest votes
        static void Eliminate(int len, Candidate[] candidates)
        {
            int min = MinVotes(len, candidates);

            foreach (Candidate candidate in candidates)
            {
                if (candidate.votes == min && candidate.eliminated == false)
                    candidate.eliminated = true;
            }
        }

        //Transfers votes from eliminated candidate to next valid choice
        static void TransferVotes(Candidate[,] votes)
        {
            //Moves through entire array
            for (int i = 0; i < votes.GetLength(0); i++)
            {
                for (int j = 0; j < votes.GetLength(1); j++)
                {
                    if (votes[i, j].eliminated == false)
                        continue;

                    //Looks for next valid choice
                    for (int k = j + 1; k < votes.GetLength(1); k++)
                    {
                        if (votes[i, k].eliminated == true)
                            continue;

                        votes[i, k].votes += votes[i, j].votes;
                        break;
                    }
                }
            }
        }

        //Finds the least amount of votes in the election
        static int MinVotes(int len, Candidate[] candidates)
        {
            int min = len;

            foreach (Candidate candidate in candidates)
            {
                if (candidate.votes < min && candidate.eliminated == false)
                    min = candidate.votes;
            }

            return min;
        }

        //Copies values from the 1D array to the correspondng 2D array's row
        //Creates array of votes, row by row
        static void CopyArrayToRow(Candidate[] array, Candidate[,] arrayRow, int row)
        {
            for (int i = 0; i < array.Length; i++)
                arrayRow[row, i] = array[i];
        }

        //Checks for candidate list validity (Makes sure there are only unique candidates)
        //Using big words again lel
        static bool CheckCandidates(string[] candidateInput)
        {
            if (candidateInput.Length <= 1)
            {
                Console.WriteLine("You must nominate at least two people.");
                return false;
            }

            if (HasDuplicates(candidateInput) == true)
            {
                Console.WriteLine("You cannot nominate the same person twice.");
                return false;
            }
            
            return true;
        }

        //Ensures that voter submits valid list of votes
        static bool CheckVotes(string[] votes, Candidate[] candidates)
        {
            if (votes.Length != candidates.Length)
            {
                Console.WriteLine("You must vote for the appropriate number of candidates.");
                return false;
            }

            if (HasDuplicates(votes) == true)
            {
                Console.WriteLine("You cannot vote for the same person twice.");
                return false;
            }

            foreach (Candidate candidate in candidates)
            {
                if (!votes.Contains(candidate.name))
                {
                    Console.WriteLine("You cannot vote for unregistered candidates.");
                    return false;
                }
            }

            return true;
        }

        //Checks for duplicates in arrays
        static bool HasDuplicates(string[] array)
        {
            List<string> values = new List<string>();

            foreach (string item in array)
            {
                if (!values.Contains(item.ToLower()))
                    values.Add(item.ToLower());
                else
                    return true;
            }

            return false;
        }
    }
}