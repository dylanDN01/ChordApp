using Microsoft.Extensions.ObjectPool;
using System.Reflection.Metadata.Ecma335;

namespace ChordApp.Components.Objects
{
    public class Note : IComparable<Note>
    {
        private string _rep; // string representation of the note object

        private List<string> _alt = new List<string>(); // alternate forms of the note. Contains the note itself

        public const int minor_3 = 3; // constant for minor 3rd
        public const int major_3 = 4; // constnat for Major 3rd

        public Note next; // next note in chord, not in scale
        public Note previous; // previous note in chord, not in scale

        private string[] scale = ["B#/C", "C#/Db", "D", "D#/Eb", "E/Fb", "E#/F", "F#/Gb", "G", "G#/Ab", "A", "A#/Bb", "B/Cb"];

        // Creates a note object, and sets the scale based on the starting note
        public Note(string rep, Note? next = null, Note? previous = null)
        {
            // validation
            if (FindAlternate(rep).Count != 0)
            {
                this._rep = rep; // string representation of the note
                this._alt = FindAlternate(rep); // find alternates
                this.next = next;
                this.previous = previous;

                Rescale(this._rep); //  set the scale using this note
            }
        }
        
        // Given an input string 'rep' that represents a note object, 
        // returns an list of strings that contain the possible forms of the given note object
        // returns a blank list if no note was found 
        public List<string> FindAlternate(string rep)
        {
            List<string> alternate = new List<string>(); // blank list

            // search for within a list of possible notes
            for (int note = 0; note < scale.Length; note++) {
                if (scale[note].Split('/').ToList().Contains(rep))
                {
                    alternate = scale[note].Split('/').ToList();
                }
            }

            return alternate;
        }


        /// <summary>
        /// Given an input string rep that represents the desired rescale note,
        /// Changes the scale of this Note object to be rooted at the parameter 'rep'
        /// </summary>
        /// <param name="rep note"></param>
        public void Rescale(string rep)
        {
            Queue<string> rescale = new Queue<string>(scale); // initialize a queue
            while (!rescale.Peek().Equals(String.Join('/', GetAlt())))
            {
                rescale.Enqueue(rescale.Dequeue());
            }
            this.scale = rescale.ToArray(); // resets the scale
        }

        /// <summary>
        /// Given an input integer '2' or '3', representing minor 2nd and minor 3rd,
        /// Return a standard string representing the note 'rep' that succeeds this 'rep',
        /// If not valid type given, return the same note
        /// </summary>
        /// <param name="minor interval"></param>
        /// <returns>string 'rep'</returns>
        public string NextMinor(int type)
        {
            int thisIndex = Array.IndexOf(scale, String.Join('/', GetAlt()));
            if (type == 2)
            {
                return scale[(thisIndex + 1) % scale.Length].Split('/').Last();
            }
            else if (type == 3)
            {
                return scale[(thisIndex + minor_3) % scale.Length].Split("/").Last();
            }
            return GetRep(); // self
        }

        /// <summary>
        /// Given an input integer '2', or '3', representing Major 2nd and Major 3rd,
        /// Return a standard string representing  the note 'rep' that succeeds this 'rep'
        /// If no valid type given, return the same note
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string NextMajor(int type)
        {
            int thisIndex = Array.IndexOf(scale, String.Join('/', GetAlt()));
            if (type == 2)
            {
                return scale[(thisIndex + 1) % scale.Length].Split('/').First();
            }
            else if (type == 3)
            {
                return scale[(thisIndex + major_3) % scale.Length].Split("/").First();
            } 
            return GetRep(); // self
        }

        /// <summary>
        /// Given an integer input "Distance", Find the next Notes string representation at the next Distance
        /// </summary>
        /// <param name="Distance"></param>
        /// <returns>The string representation of the next note within a Distance (default first entry found after split)</returns>
        public string GetNextNote(int Distance = 1)
        {
            // Distance mod Scale Length gets the next note within any distance (handles wraparound)
            string NoteRep = scale[Distance % scale.Length].Split('/').First();
            return NoteRep;
        }

        /// <summary>
        /// Given a string representation of a Note, And the scale type string (default Major)
        /// Return the amount of Sharps in that Notes scale
        /// </summary>
        /// <param name="Note">The Note to search</param>
        /// <param name="AccidentalType">Either "Sharp" or "Flat"</param>
        /// <param name="Type">Either "Major" or "Minor"</param>
        /// <returns></returns>
        public int GetNumAccidentals(string Note, string AccidentalType, string Type = "Major")
        {
            int currIndex = Array.IndexOf(scale, "B#/C");

            // chord types
            switch (Type)
            {
                case "Major":
                    currIndex = Array.IndexOf(scale, "B#/C");
                    break;
                case "Minor":
                    currIndex = Array.IndexOf(scale, "A");
                    break;
            }
            
            int NumAccidentals = 0;
            while (!scale[currIndex % scale.Length].Split('/').Contains(Note))
            {
                if (AccidentalType.Equals("Flat"))
                {
                    if (Type.Equals("Major"))
                    {
                        currIndex = currIndex + 5;
                    }
                    else if (Type.Equals("Minor"))
                    {
                        currIndex = currIndex + 7;
                    }
                   
                }
                else if (AccidentalType.Equals("Sharp"))
                {
                    if (Type.Equals("Major"))
                    {
                        currIndex = currIndex + 7;
                    }
                    else if (Type.Equals("Minor"))
                    {
                        currIndex = currIndex + 5;
                    }
                    
                }
                NumAccidentals++;
            }
            return NumAccidentals;
        }



        // Given an input other object of type "Note",
        // Return the distance to the other note on the scale.
        // A positive distance means that This Note is above on the scale,
        // A negative distance means that This Note is below on the scale,
        public int CompareTo(Note other)
        {
            int distance = Array.IndexOf(scale, String.Join('/', GetAlt()));
            distance -= Array.IndexOf(scale, String.Join('/', other.GetAlt()));

            return distance; // can be positive or negative
        }

        public override string ToString()
        {
            return _rep;
        }

        // Returns the list of alternate representations of this note
        public List<string> GetAlt() { return _alt; }

        // Returns the string representation of this note
        public string GetRep() { return _rep; }
    }
}
