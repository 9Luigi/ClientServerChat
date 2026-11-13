using System.Drawing;

namespace WFChat.UI
{
    /// <summary>Gives every user name its own color in the chat. The same name always gets the same color, on every machine.</summary>
    public static class UserColors
    {
        //bright enough to read on black, distinct from each other and from the almost white used for "me"
        static readonly Color[] Palette =
        {
            Color.FromArgb(0x00, 0xFF, 0x41), //matrix green
            Color.FromArgb(0x7C, 0xFF, 0x4F), //lime
            Color.FromArgb(0x00, 0xE5, 0xC0), //aqua
            Color.FromArgb(0xB6, 0xFF, 0x00), //chartreuse
            Color.FromArgb(0x4D, 0xFF, 0xB8), //mint
            Color.FromArgb(0x00, 0xC2, 0xFF), //cyan
            Color.FromArgb(0xE6, 0xFF, 0x66), //pale yellow
            Color.FromArgb(0x9D, 0xFF, 0x9D), //pale green
        };

        public static Color For(string userName)
        {
            //string.GetHashCode is randomized per process, so a fixed hash (FNV-1a) keeps colors stable between runs
            uint hash = 2166136261;
            foreach (char c in (userName ?? "").ToLowerInvariant())
            {
                hash ^= c;
                hash *= 16777619;
            }
            return Palette[hash % (uint)Palette.Length];
        }
    }
}
