using System;
using System.Text;

namespace Detergent
{
    public static class IncidentIdGenerator
    {
        public static string GenerateIncidentId()
        {
            StringBuilder eventId = new StringBuilder();

            for (int i = 0; i < 3; i++)
            {
                char chr = consonants[rnd.Next(consonants.Length)];
                eventId.Append(chr);
                chr = vowels[rnd.Next(vowels.Length)];
                eventId.Append(chr);
            }

            return eventId.ToString().ToUpperInvariant();
        }

        private static readonly char[] consonants = 
            {
                'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'r', 's', 't',
                'v', 'w', 'x', 'y', 'z'
            };
        private static readonly char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
        private static Random rnd = new Random();
    }
}