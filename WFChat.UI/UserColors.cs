using System.Drawing;

namespace WFChat.UI
{
    /// <summary>Gives every user name its own color in the chat. The same name always gets the same color, on every machine.</summary>
    public static class UserColors
    {
        //dark enough to read on white, distinct from each other and from the navy used for "me"
        static readonly Color[] Palette =
        {
            Color.FromArgb(0x1F, 0x6F, 0xB2), //blue
            Color.FromArgb(0x2E, 0x7D, 0x32), //green
            Color.FromArgb(0xB3, 0x5C, 0x00), //amber
            Color.FromArgb(0x7B, 0x3F, 0xA0), //purple
            Color.FromArgb(0x00, 0x7C, 0x7C), //teal
            Color.FromArgb(0xA6, 0x1E, 0x4D), //crimson
            Color.FromArgb(0x5D, 0x6B, 0x00), //olive
            Color.FromArgb(0x8D, 0x4E, 0x2A), //brown
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
