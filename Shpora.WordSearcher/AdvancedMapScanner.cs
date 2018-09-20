using System.Threading.Tasks;

namespace Shpora.WordSearcher
{
    public class AdvancedMapScanner : SimpleMapScanner
    {
        public AdvancedMapScanner(WordSearcherGameClient wsGameClient, int mapWidth, int mapHeight)
            : base(wsGameClient, mapWidth, mapHeight)
        {
        }

        public override async Task<bool[,]> ScanMap()
        {
        }
    }
}
