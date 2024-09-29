using UnityEngine;

namespace Data
{
    public class KeyWordLibrary
    {
        public string[] keywords;

        public string GetPartKeyWord()
        {
            var index = Random.Range(0, keywords.Length);
            return keywords[index];
        }
    }
}
