
using Sfs2X.Entities.Data;

namespace bot;
public class Grid
{
    private List<Gem> Gems = new();
    private ISFSArray GemsCode;
    public HashSet<GemType> GemTypes { get; set; } = new();
    private HashSet<GemType> MyHeroGemType { get; }

    public Grid(ISFSArray gemsCode, HashSet<GemType> gemTypes)
    {
        UpdateGems(gemsCode);
        MyHeroGemType = gemTypes;
    }

    public void UpdateGems(ISFSArray gemsCode) {
        Gems.Clear();
        GemTypes.Clear();
        for (var i = 0; i < gemsCode.Size(); i++) {
            var gem = new Gem(i, (GemType)gemsCode.GetByte(i));
            Gems.Add(gem);
            GemTypes.Add(gem.Type);
        }
    }

    public Pair<int> recommendSwapGem() {
        var listMatchGem = suggestMatch();

        Console.WriteLine("recommendSwapGem " + listMatchGem.Count);
        if (listMatchGem.Count == 0) {
            return new Pair<int>(-1, -1);
        }

        var matchGemSizeThanFour = listMatchGem.FirstOrDefault(gemMatch => gemMatch.sizeMatch > 4);
        if (matchGemSizeThanFour != null) {
            return matchGemSizeThanFour.getIndexSwapGem();
        }
        var matchGemSizeThanThree = listMatchGem.FirstOrDefault(gemMatch => gemMatch.sizeMatch > 3);
        if (matchGemSizeThanThree != null) {
            return matchGemSizeThanThree.getIndexSwapGem();
        }
        var matchGemSword = listMatchGem.FirstOrDefault(gemMatch => gemMatch.type == GemType.SWORD);
        if (matchGemSword != null) {
            return matchGemSword.getIndexSwapGem();
        }

        foreach (GemType type in MyHeroGemType) {
            var matchGem = listMatchGem.FirstOrDefault(gemMatch => gemMatch.type == type);
                    //listMatchGem.stream().filter(gemMatch -> gemMatch.getType() == type).findFirst();
            if (matchGem != null) {
                return matchGem.getIndexSwapGem();
            }
        }
        return listMatchGem[0].getIndexSwapGem();
    }

    private List<GemSwapInfo> suggestMatch() {
        var listMatchGem = new List<GemSwapInfo>();

        var tempGems = new List<Gem>(Gems);
        foreach (var currentGem in tempGems) {
            Gem swapGem;
            // If x > 0 => swap left & check
            if (currentGem.PosX > 0) {
                swapGem = Gems[GetGemIndexAt(currentGem.PosX - 1, currentGem.PosY)];
                checkMatchSwapGem(listMatchGem, currentGem, swapGem);
            }
            // If x < 7 => swap right & check
            if (currentGem.PosX < 7) {
                swapGem = Gems[GetGemIndexAt(currentGem.PosX + 1, currentGem.PosY)];
                checkMatchSwapGem(listMatchGem, currentGem, swapGem);
            }
            // If y < 7 => swap up & check
            if (currentGem.PosY < 7) {
                swapGem = Gems[GetGemIndexAt(currentGem.PosX, currentGem.PosY + 1)];
                checkMatchSwapGem(listMatchGem, currentGem, swapGem);
            }
            // If y > 0 => swap down & check
            if (currentGem.PosY > 0) {
                swapGem = Gems[GetGemIndexAt(currentGem.PosX, currentGem.PosY - 1)];
                checkMatchSwapGem(listMatchGem, currentGem, swapGem);
            }
        }
        return listMatchGem;
    }

    private void checkMatchSwapGem(List<GemSwapInfo> listMatchGem, Gem currentGem, Gem swapGem) {
        Swap(currentGem, swapGem);
        HashSet<Gem> matchGems = matchesAt(currentGem.PosX, currentGem.PosY);

        Swap(currentGem, swapGem);
        if (matchGems.Count > 0) {
            listMatchGem.Add(new GemSwapInfo(currentGem.Index, swapGem.Index, matchGems.Count, currentGem.Type));
        }
    }

    private static int GetGemIndexAt(int x, int y) {
        return x + y * 8;
    }

    private void Swap(Gem a, Gem b) {
        var tempIndex = a.Index;
        var tempX = a.PosX;
        var tempY = a.PosY;

        // update reference
        Gems[a.Index] = b;
        Gems[b.Index] = a;

        // update data of element
        a.Index = b.Index;
        a.PosX = b.PosX;
        a.PosY = b.PosY;

        b.Index = tempIndex;
        b.PosX = tempX;
        b.PosY = tempY;
    }

    private HashSet<Gem> matchesAt(int x, int y) {
        var res = new HashSet<Gem>();
        var center = GemAt(x, y);

        // check horizontally
        var hor = new List<Gem> {center};
        int xLeft = x - 1, xRight = x + 1;
        while (xLeft >= 0) {
            var gemLeft = GemAt(xLeft, y);
            if (!gemLeft.sameType(center)) {
                break;
            }
            hor.Add(gemLeft);
            xLeft--;
        }
        while (xRight < 8) {
            var gemRight = GemAt(xRight, y);
            if (!gemRight.sameType(center)) {
                break;
            }
            hor.Add(gemRight);
            xRight++;
        }
        if (hor.Count >= 3) res.UnionWith(hor);

        // check vertically
        var ver = new List<Gem> {center};
        int yBelow = y - 1, yAbove = y + 1;
        while (yBelow >= 0) {
            var gemBelow = GemAt(x, yBelow);
            if (!gemBelow.sameType(center)) {
                break;
            }
            ver.Add(gemBelow);
            yBelow--;
        }
        while (yAbove < 8) {
            var gemAbove = GemAt(x, yAbove);
            if (!gemAbove.sameType(center)) {
                break;
            }
            ver.Add(gemAbove);
            yAbove++;
        }
        if (ver.Count >= 3) res.UnionWith(ver);

        return res;
    }

    // Find Gem at Position (x, y)
    private Gem GemAt(int x, int y)
    {
        return Gems.First(g => g.PosX == x && g.PosY == y);
    }
}