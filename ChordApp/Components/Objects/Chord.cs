using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Primitives;
using System;

namespace ChordApp.Components.Objects
{
    public class Chord
    {
        private Note root; // root note

        private string[] validNotes;

        private int SETDEPTH = 3; // up to a 7th if set to 4, triad if 3

        private string[] ChordTypes = ["Diminished", "Minor", "Major", "Augmented"];
        /// <summary>
        /// Constructs a Chord
        /// </summary>
        /// <param name="_baseRep">Note the begin</param>
        public Chord(string[] _baseRep) 
        {
            validNotes = _baseRep; // chords must have these
            root = ConstructChordTree(_baseRep[0], 0);
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

        private bool validateChord(string inputChord)
        {
            if (validNotes == null || String.IsNullOrEmpty(inputChord))
            {
                return false; // or handle this case as appropriate
            }
            string[] chordNotes = inputChord.Split('/');

            bool ContainsSoFar = false;
            
            for (int i = 0; i < validNotes.Length; i++)
            {
                ContainsSoFar = false;
                for (int j = 0; j < chordNotes.Length; j++)
                {
                    if (chordNotes[j].Equals(validNotes[i]))
                    {
                        ContainsSoFar = true;
                    }
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
            return result + "-" + BuildTreeString(node.previous, result + node.ToString() + "/", depth + 1) + " " + BuildTreeString(node.next, result + node.ToString() + "/", depth + 1);

            
        }

        /// <summary>
        /// Returns the tallest branch of the tree (usually set at 3 for triad)
        /// </summary>
        /// <returns>Returns the Maximum height for a tree</returns>
        public int MaxDepth() 
        {   
            return SETDEPTH;
        }

    }
}
