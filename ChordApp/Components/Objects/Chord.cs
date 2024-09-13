using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Primitives;
using System;

namespace ChordApp.Components.Objects
{
    public class Chord
    {
        private Note root; // root note

        private string[] validNotes; // from constructor; notes from input chord

        private int SETDEPTH = 3; // up to a 7th if set to 4, triad if 3

        private string[] ChordTypes;

        /// <summary>
        /// Constructs a Chord
        /// </summary>
        /// <param name="_baseRep">Note the begin</param>
        public Chord(string[] _baseRep, int depth) 
        {
            this.ChordTypes = CreateChordLabels(depth);
            this.SETDEPTH = depth;
            this.validNotes = _baseRep; // chords must have these
            this.root = ConstructChordTree(_baseRep[0], 0);
        }

        /// <summary>
        /// Generates all the chord labels for the string representation
        /// </summary>
        /// <param name="depth">The integer value representing the depth</param>
        /// <returns>The string array containing all the valid labels for the given depth</returns>
        private string[] CreateChordLabels(int depth)
        {
            switch (depth)
            {
                case 1:
                    return ["Note"];
                case 2:
                    return ["Minor 3rd", "Major 3rd"];
                case 3:
                    return ["Diminished", "Minor", "Major", "Augmented"];
                case 4:
                    return ["Diminished 7th", "Diminished Minor 7th", "Minor 7th", "Minor Major 7th", "Dominant 7th", "Major 7th", "Augmented Major 7th", "Augmented 7th"];
                default:
                    return [];
            }   
        }

        /// <summary>
        /// Helper for constucting a Chord
        /// </summary>
        /// <param name="_baseRep">Base note string Representation</param>
        /// <param name="depth">Current depth</param>
        /// <returns>A head "Note" object that is the head for following notes</returns>
        public Note ConstructChordTree(string _baseRep, int depth)
        {
            Note ChordNote = new Note(_baseRep);
            if (depth < SETDEPTH)
            {
                ChordNote.next = ConstructChordTree(ChordNote.NextMajor(3), depth + 1);
                ChordNote.previous = ConstructChordTree(ChordNote.NextMinor(3), depth + 1);
            }
            
            return ChordNote;
        }



        /// <summary>
        /// Returns the list of all chords possible
        /// </summary>
        /// <returns>A List of strings, with each entry being a complete chord</returns>
        public List<string> GetChordSet()
        {
            return BuildTreeString(root, "", 0).Split(' ').Where(chord => !chord.Contains('-'))
            .Select(chord => validateChord(chord) ? chord : ".")
            .ToList();
        }

        /// <summary>
        /// Returns the string representation of all the possible chords
        /// </summary>
        /// <returns>A string of each chord, seperated by a "space"</returns>
        public override string ToString()
        {


            string result = "";
            List<string> chords = GetChordSet();

            for (int i = 0; i < ChordTypes.Length; i++)
            { 
                if (chords.ElementAt(i) != ".")
                {
                    
                    result += ChordTypes[i] + ": " + chords.ElementAt(i) + ", \n";
                }
                
            }
            return result;
        }

        /// <summary>
        /// Checks if a chord is valid for this Chord's notes
        /// Returns True if the Chord contains all the make up of the input (or alternate),
        /// Returns False otherwise
        /// </summary>
        /// <param name="inputChord">Standard Form Chord 1/2/3/ </param>
        /// <returns>True or False for existing</returns>
        private bool validateChord(string inputChord)
        {
            if (validNotes == null || String.IsNullOrEmpty(inputChord))
            {
                return false; // or handle this case as appropriate
            }
            string[] chordNotes = inputChord.Split('/');
        
            for (int i = 0; i < validNotes.Length; i++)
            {
                bool ContainsSoFar = false;
                for (int j = 0; j < chordNotes.Length; j++)
                {
                    Note first = new Note(chordNotes[j]);
                    Note second = new Note(validNotes[i]);
                    if (first.CompareTo(second) == 0) { ContainsSoFar = true; continue; }
                }
                if (ContainsSoFar == false) {
                    return false; 
                }
            }

            return true;

        }

        /// <summary>
        /// Helper method to recursively build up all chords
        /// </summary>
        /// <param name="node">The root node of the chord</param>
        /// <param name="result">The "So Far" built string</param>
        /// <param name="depth">The current depth counter</param>
        /// <returns>The unfiltered string object that contains all the steps and final product</returns>
        private string BuildTreeString(Note node, string result, int depth)
        {
            if (depth == MaxDepth())
            {

                return result;
                      
            }
            // seperator '-' only exists for incomplete (non max depth) branches
            // The " " is important to seperate the steps
            return result + "-" + BuildTreeString(node.previous, result + node.ToString() + "/", depth + 1) + " " + BuildTreeString(node.next, result + node.ToString() + "/", depth + 1);
        }

        /// <summary>
        /// Finds the Solution Chord if there is only 1
        /// </summary>
        /// <returns>A string representing the solution chord, or "" if there is none</returns>
        public string SolutionChord()
        {
            List<string> ChordSet = GetChordSet().Where(chord => !chord.Equals(".")).ToList();
            ChordSet.ForEach(chord => Console.WriteLine(chord));

            if (ChordSet.Count == 1)
            {
                return ChordSet[0]; // first element is the solution
            }
            return "";

        }

        /// <summary>
        /// Given a string input in valid format 1/2/3/, and the root note input (using method GetRoot()), and a match floor (default 0)
        /// Returns if the Chord Exists in the tree
        /// </summary>
        /// <param name="chord">a string representation of the chord 1/2/3/</param>
        /// <param name="curr">the current note (for recursive searching)</param>
        /// <param name="matches">number of valid notes found</param>
        /// <returns>if there is a chord that matches that order</returns>
        public bool ChordExists(string chord, Note curr, int matches = 0)
        {
            List<Note> ChordList = chord.Split('/').Select(x => new Note(x)).ToList();
            if (ChordList.Any(x => x.CompareTo(curr) == 0))
            {
                if (curr.next == null || curr.previous == null)
                {
                    return true;
                }
                return (true && (ChordExists(String.Join('/', ChordList), curr.next, matches + 1)) || (ChordExists(String.Join('/', ChordList), curr.previous, matches + 1)));
            }

            // enough matching notes
            if (matches == SETDEPTH) { return true; }

            // not enough matching notes
            return false;

        }
        

        /// <summary>
        /// Returns the tallest branch of the tree (usually set at 3 for triad)
        /// </summary>
        /// <returns>Returns the Maximum height for a tree</returns>
        public int MaxDepth() 
        {   
            return SETDEPTH;
        }

        public Note GetRoot() { return root; }

    }
}
