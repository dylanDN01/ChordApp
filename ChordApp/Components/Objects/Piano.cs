

namespace ChordApp.Components.Objects
{
    public class Piano
    {
        private int[] HighlightedIndices;
        private string Chord;
        
        public Piano(string Chord )
        {
            this.Chord = Chord;
            this.HighlightedIndices = new int[24];
        }

        public int[] GetChordIndices() { return []; }

        /// <summary>
        /// Given a string input "Base" representing the root note of a scale,
        /// Get the indices of the notes on the 24 'note' keyboard in the scale using base
        /// Checks the CHordType 
        /// </summary>
        /// <returns></returns>
        public int[] GetScaleIndices(string Base, string ChordType) { return []; } 
    }
}
