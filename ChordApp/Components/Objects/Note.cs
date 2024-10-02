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
        /// Given a string representation of a Note, And the scale type string
        /// Return the amount of Sharps in that Notes scale. Handles Sharp and Flats automatically
        /// </summary>
        /// <param name="Note">The Note to search</param>
        /// <param name="Type">Either "Major" or "Minor"</param>
        /// <returns>int number of Accidentals, string type of Accidental (Sharp/Flat)</returns>
        public (int, string) GetNumAccidentals(string Note, string Type)
        {
            Note tempNote = new Note(Note); // create a temporary note for easier comparison
            // automatic detection for sharps and flats
            string AccidentalType = "Sharp";
            if (Type.Equals("Major"))
            {
                string[] MajorSharps = { "C", "G", "D", "A", "E", "B", "F#", "C#" };
                AccidentalType = MajorSharps.Any(n => tempNote.GetAlt().Contains(n)) ? "Sharp" : "Flat";
            }
            else if (Type.Equals("Minor"))
            {
                string[] MinorSharps = { "A", "E", "B", "F#", "C#", "G#, D#" };
                AccidentalType = MinorSharps.Any(n => tempNote.GetAlt().Contains(n)) ? "Sharp" : "Flat";
            }


            int currIndex = Array.IndexOf(scale, "B#/C");

            // chord types
            if (Type.Equals("Major"))
            {
                currIndex = Array.IndexOf(scale, "B#/C");
            }
            else if (Type.Equals("Minor"))
            {
                currIndex = Array.IndexOf(scale, "A");
            }

            // count accidentals
            int NumAccidentals = 0;
            while (!scale[currIndex % scale.Length].Split('/').Contains(Note))
            {
                int adjustment = 0;
                if (AccidentalType == "Sharp")
                {
                    adjustment += 7;
                }
                else if (AccidentalType == "Flat")
                {
                    adjustment += 5;
                }

                currIndex += adjustment;
                NumAccidentals++;
            }
            return (NumAccidentals, AccidentalType);
        }



        /// <summary>
        /// Given a string input Note representing the root, and the Type of Accidental (Sharp/Flat),
        /// Return an array of strings, representing each Accidental on the Scale. Dependent on NumAccidentals
        /// </summary>
        /// <param name="Note">Root note</param>
        /// <param name="Type">Accidental (Sharp or Flat)</param>
        /// <returns>String array of length equal to the length from GetNumAccidentals, contains all the sharps or flats</returns>
        public string[] GetAccidentals(string Note, string Type)
        {
            int NumAccidental = GetNumAccidentals(Note, Type).Item1;
            string AccidentalType = GetNumAccidentals(Note, Type).Item2;

            string[] AccidentalList = new string[NumAccidental];

            int currIndex = Array.IndexOf(scale, "F#/Gb"); // sharps by default
            if (AccidentalType.Equals("Sharp"))
            {
                currIndex = Array.IndexOf(scale, "F#/Gb");
                for (int i = 0; i < NumAccidental; i++)
                {
                    AccidentalList[i] = scale[currIndex % scale.Length].Split('/').First();
                    currIndex = currIndex + 7;
                }
            }
            else if (AccidentalType.Equals("Flat"))
            {
                currIndex = Array.IndexOf(scale, "A#/Bb");
                for (int i = 0; i < NumAccidental; i++)
                {
                    AccidentalList[i] = scale[currIndex % scale.Length].Split('/').Last();
                    currIndex = currIndex + 5;
                }
            }
            return AccidentalList;
        }



        /// <summary>
        /// Given an input string Note and a string Type, return the List of strings
        /// representing the Full Scale of the Note provided.
        /// </summary>
        /// <param name="Note">A singular Note, in format C, C#, or Cb</param>
        /// <param name="Type">Either Major or Minor, usually dependent on the "Mode"</param>
        /// <returns>A List<string> of all the notes in the scale starting from the root</string></returns>
        public List<String> FullScale(string Note, string Type)
        {
            List<String> Scale  = WhiteKeyScale(Note, Type); // step 1 get white keys (7 total)

            string[] AccListNormalized = GetAccidentals(Note, Type).Select(x => x.Substring(0, 1)).ToArray(); // Unsharp the Accidentals for comparison

            // LINQ query to get the indices within the  Scale list created above which have a key that matches an accidental
            var IndicesInAccList = Scale.AsEnumerable().Select((item, index) => new { Item = item, Index = index })
                .Where(item => AccListNormalized.Contains(item.Item.Substring(0, 1)))
                .Select(item => item.Index)
                .ToList();

            // Add accidental to white keys
            for (int i = 0; i < IndicesInAccList.Count; i++)
            {
                Scale[IndicesInAccList[i]] = GetAccidentals(Note, Type)[Array.IndexOf(AccListNormalized, Scale[IndicesInAccList[i]])];
            }

            return Scale;
        }


        /// <summary>
        /// Intended for use with FullScale() Method only.
        /// Returns a list of strings representing the white keys to be modified,
        /// Automatically handles sharps vs flat scales
        /// </summary>
        /// <param name="note">The Unmodified Note string</param>
        /// <param name="Type">The Mode of the desired scale; Either Major or Minor</param>
        /// <returns>A List of Strings containing each of the 7 White Keys in order of note</returns>
        private List<String> WhiteKeyScale(string note, string Type)
        {
            string[] TempArray = ["C", "D", "E", "F", "G", "A", "B"];

            int index = Array.IndexOf(TempArray, note.Substring(0, 1));
            if (GetNumAccidentals(note, Type).Item2.Equals("Flat"))
            {
                index++;
            }

            Queue<string> rescale = new Queue<string>(TempArray); // initialize a queue

            note = TempArray[index]; // normalize
            while (!rescale.Peek().Equals(note))
            {
                rescale.Enqueue(rescale.Dequeue());
            }
            return rescale.ToList();
        }



        // Given an input other object of type "Note",
        // Return the distance to the other note on the scale.
        // A positive distance means that This Note is above on the scale,
        // A negative distance means that This Note is below on the scale,
        public int CompareTo(Note other)
        {
            int? distance = Array.IndexOf(scale, String.Join('/', GetAlt()));
            distance -= Array.IndexOf(scale, String.Join('/', other.GetAlt()));

            return distance ?? 0; // can be positive or negative, 0 if null
        }


        /// <summary>
        /// Returns the string representation of this Note. Same function as GetRep()
        /// </summary>
        /// <returns>A string of character(s)</returns>
        public override string ToString()
        {
            return _rep;
        }

        
        /// <summary>
        /// Finds a list of strings that have the same pitch on the scale
        /// </summary>
        /// <returns>A list of Strings</returns>
        public List<string> GetAlt() { return _alt; }

        /// <summary>
        /// Returns the string representation of this Note. Same function as ToString()
        /// </summary>
        /// <returns>A string of character(s)</returns>
        public string GetRep() { return _rep; }

        /// <summary>
        /// Returns the string array representing the scale from this note
        /// </summary>
        /// <returns>string array of length 12</returns>
        public string[] GetScale() { return scale; }
    }
}
