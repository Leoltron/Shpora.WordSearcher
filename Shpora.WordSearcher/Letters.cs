using System.Collections.Generic;
using System.Linq;

namespace Shpora.WordSearcher
{
    public static class Letters
    {
        private static readonly string[] Alphabet =
            {А, Б, В, Г, Д, Е, Ё, Ж, З, И, Й, К, Л, М, Н, О, П, Р, С, Т, У, Ф, Х, Ц, Ч, Ш, Щ, Ъ, Ы, Ь, Э, Ю, Я};

        public static Dictionary<long, char> AlphabetViews = new Dictionary<long, char>
        {
            [LetterLayoutToViewHash(А)] = 'А',
            [LetterLayoutToViewHash(Б)] = 'Б',
            [LetterLayoutToViewHash(В)] = 'В',
            [LetterLayoutToViewHash(Г)] = 'Г',
            [LetterLayoutToViewHash(Д)] = 'Д',
            [LetterLayoutToViewHash(Е)] = 'Е',
            [LetterLayoutToViewHash(Ё)] = 'Ё',
            [LetterLayoutToViewHash(Ж)] = 'Ж',
            [LetterLayoutToViewHash(З)] = 'З',
            [LetterLayoutToViewHash(И)] = 'И',
            [LetterLayoutToViewHash(Й)] = 'Й',
            [LetterLayoutToViewHash(К)] = 'К',
            [LetterLayoutToViewHash(Л)] = 'Л',
            [LetterLayoutToViewHash(М)] = 'М',
            [LetterLayoutToViewHash(Н)] = 'Н',
            [LetterLayoutToViewHash(О)] = 'О',
            [LetterLayoutToViewHash(П)] = 'П',
            [LetterLayoutToViewHash(Р)] = 'Р',
            [LetterLayoutToViewHash(С)] = 'С',
            [LetterLayoutToViewHash(Т)] = 'Т',
            [LetterLayoutToViewHash(У)] = 'У',
            [LetterLayoutToViewHash(Ф)] = 'Ф',
            [LetterLayoutToViewHash(Х)] = 'Х',
            [LetterLayoutToViewHash(Ц)] = 'Ц',
            [LetterLayoutToViewHash(Ч)] = 'Ч',
            [LetterLayoutToViewHash(Ш)] = 'Ш',
            [LetterLayoutToViewHash(Щ)] = 'Щ',
            [LetterLayoutToViewHash(Ъ)] = 'Ъ',
            [LetterLayoutToViewHash(Ы)] = 'Ы',
            [LetterLayoutToViewHash(Ь)] = 'Ь',
            [LetterLayoutToViewHash(Э)] = 'Э',
            [LetterLayoutToViewHash(Ю)] = 'Ю',
            [LetterLayoutToViewHash(Я)] = 'Я'
        };

        private static long LetterLayoutToViewHash(string layout)
        {
            return LetterLayoutToView(layout).CustomHashCode();
        }

        private static bool[,] LetterLayoutToView(string layout)
        {
            var split = layout.Split('\n');
            var view = new bool[split.Max(s => s.Length), split.Length];
            for (var y = 0; y < split.Length; y++)
            for (var x = 0; x < split[y].Length; x++)
            {
                view[x, y] = split[y][x] == '#';
            }

            return view;
        }

        private const string А = "   #   \n" +
                                 "  # #  \n" +
                                 "  # #  \n" +
                                 " ##### \n" +
                                 " #   # \n" +
                                 "#     #\n" +
                                 "#     #";

        private const string Б = "#######\n" +
                                 "#      \n" +
                                 "Г     \n" +
                                 "#######\n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "#######";

        private const string В = "#######\n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "###### \n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "#######";

        private const string Г = "#######\n" +
                                 "#      \n" +
                                 "#      \n" +
                                 "#      \n" +
                                 "#      \n" +
                                 "#      \n" +
                                 "#      ";

        private const string Д = " ##### \n" +
                                 " #   # \n" +
                                 " #   # \n" +
                                 " #   # \n" +
                                 " ##### \n" +
                                 "#     #\n" +
                                 "#     #";

        private const string Е = "#######\n" +
                                 "#      \n" +
                                 "#      \n" +
                                 "#######\n" +
                                 "#      \n" +
                                 "#      \n" +
                                 "#######";


        private const string Ё = " #   # \n" +
                                 "       \n" +
                                 "#######\n" +
                                 "#      \n" +
                                 "#######\n" +
                                 "#      \n" +
                                 "#######";

        private const string Ж = "#  #  #\n" +
                                 " # # # \n" +
                                 "  ###  \n" +
                                 "   #   \n" +
                                 "  ###  \n" +
                                 " # # # \n" +
                                 "#  #  #";

        private const string З = "#######\n" +
                                 "      #\n" +
                                 "      #\n" +
                                 "###### \n" +
                                 "      #\n" +
                                 "      #\n" +
                                 "#######";

        private const string И = "#     #\n" +
                                 "#    ##\n" +
                                 "#   # #\n" +
                                 "#  #  #\n" +
                                 "# #   #\n" +
                                 "##    #\n" +
                                 "#     #";


        private const string Й = "# ### #\n" +
                                 "#    ##\n" +
                                 "#   # #\n" +
                                 "#  #  #\n" +
                                 "# #   #\n" +
                                 "##    #\n" +
                                 "#     #";

        private const string К = "#     #\n" +
                                 "#   ## \n" +
                                 "# ##   \n" +
                                 "##     \n" +
                                 "# ##   \n" +
                                 "#   ## \n" +
                                 "#     #";

        private const string Л = "   #   \n" +
                                 "  # #  \n" +
                                 " #   # \n" +
                                 " #   # \n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "#     #";

        private const string М = "#     #\n" +
                                 "##   ##\n" +
                                 "# # # #\n" +
                                 "#  #  #\n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "#     #";

        private const string Н = "#     #\n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "#######\n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "#     #";

        private const string О = "   #   \n" +
                                 "  # #  \n" +
                                 " #   # \n" +
                                 "##   ##\n" +
                                 " #   # \n" +
                                 "  # #  \n" +
                                 "   #   ";

        private const string П = "#######\n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "#     #";

        private const string Р = "#######\n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "#######\n" +
                                 "#      \n" +
                                 "#      \n" +
                                 "#      ";

        private const string С = "#######\n" +
                                 "#      \n" +
                                 "#      \n" +
                                 "#      \n" +
                                 "#      \n" +
                                 "#      \n" +
                                 "#######";

        private const string Т = "#######\n" +
                                 "   #   \n" +
                                 "   #   \n" +
                                 "   #   \n" +
                                 "   #   \n" +
                                 "   #   \n" +
                                 "   #   ";

        private const string У = "#     #\n" +
                                 " #   # \n" +
                                 "  # #  \n" +
                                 "   #   \n" +
                                 "  #    \n" +
                                 " #     \n" +
                                 "#      ";

        private const string Ф = "#######\n" +
                                 "#  #  #\n" +
                                 "#  #  #\n" +
                                 "#######\n" +
                                 "   #   \n" +
                                 "   #   \n" +
                                 "   #   ";

        private const string Х = "#     #\n" +
                                 " #   # \n" +
                                 "  # #  \n" +
                                 "   #   \n" +
                                 "  # #  \n" +
                                 " #   # \n" +
                                 "#     #";

        private const string Ц = "#    # \n" +
                                 "#    # \n" +
                                 "#    # \n" +
                                 "#    # \n" +
                                 "#    # \n" +
                                 "#######\n" +
                                 "      #";

        private const string Ч = "#     #\n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "#######\n" +
                                 "      #\n" +
                                 "      #\n" +
                                 "      #";

        private const string Ш = "#  #  #\n" +
                                 "#  #  #\n" +
                                 "#  #  #\n" +
                                 "#  #  #\n" +
                                 "#  #  #\n" +
                                 "#  #  #\n" +
                                 "#######";

        private const string Щ = "#  #  #\n" +
                                 "#  #  #\n" +
                                 "#  #  #\n" +
                                 "#  #  #\n" +
                                 "#  #  #\n" +
                                 "#######\n" +
                                 "      #";

        private const string Ъ = "##     \n" +
                                 " #     \n" +
                                 " #     \n" +
                                 " ######\n" +
                                 " #    #\n" +
                                 " #    #\n" +
                                 " ######";


        private const string Ы = "#     #\n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "####  #\n" +
                                 "#  #  #\n" +
                                 "#  #  #\n" +
                                 "####  #";

        private const string Ь = "#      \n" +
                                 "#      \n" +
                                 "#      \n" +
                                 "#######\n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "#######";

        private const string Э = "  #### \n" +
                                 " #    #\n" +
                                 "#     #\n" +
                                 "   ### \n" +
                                 "#     #\n" +
                                 " #    #\n" +
                                 "  #### ";

        private const string Ю = "# #####\n" +
                                 "# #   #\n" +
                                 "# #   #\n" +
                                 "###   #\n" +
                                 "# #   #\n" +
                                 "# #   #\n" +
                                 "# #####";

        private const string Я = "#######\n" +
                                 "#     #\n" +
                                 "#     #\n" +
                                 "#######\n" +
                                 "    ###\n" +
                                 "  ##  #\n" +
                                 "##    #";
    }
}