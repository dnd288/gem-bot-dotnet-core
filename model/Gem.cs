namespace bot; 
public class Gem {
    private const int HEIGHT = 8;
    private const int WIDTH = 8;
    public int Index { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
    public GemType Type { get; }

    public Gem(int index, GemType type) {
        Index = index;
        Type = type;
        UpdatePosition();
    }

    private void UpdatePosition() {
        PosY = Index / HEIGHT;
        PosX = Index - PosY * WIDTH;
    }

    public bool sameType(Gem other) {
        return this.Type == other.Type;
    }

    public bool sameType(GemType type) {
        return Type == type;
    }

    public bool equals(Object o) {
        if (this == o) return true;
        if (GetType() != o.GetType()) return false;

        Gem gem = (Gem) o;

        if (Index != gem.Index) return false;
        return Type == gem.Type;
    }

    public int hashCode() {
        return 31 * Index + (Type.GetHashCode());
    }
}
