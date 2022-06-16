using System;
using System.Collections.Generic;
using System.Text;

namespace TerangaDatasParser
{
    [System.Serializable]
    public class Storytelling_preset_struct
    {
        public string character;
        public string text;
        public string background;
    }

    [System.Serializable]
    public class Storytelling_level_struct
    {
        public int level;
        public Storytelling_preset_struct intro;
        public Storytelling_preset_struct outro;
    }

    [System.Serializable]
    public class Storytelling_struct
    {
        public List<Storytelling_level_struct> levels;
    }
}
