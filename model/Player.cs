
namespace bot {
    public class Player
    {
        public int Id { get; }
        public string Name { get; }
        public string DisplayName { get; }
        public List<Hero> Heroes { get; }
        private HashSet<GemType> HeroGemType { get; }
        
        public Hero? FirstHeroFullMana => Heroes.FirstOrDefault(hero => hero.IsAlive && hero.IsFullMana);

        public Hero? FirstHeroAlive => Heroes.FirstOrDefault(s => s.IsAlive);

        public Player(int playerId, string name)
        {
            Id = playerId;
            Name = name;
            DisplayName = name;

            Heroes = new List<Hero>();
            HeroGemType = new HashSet<GemType>();
        }

        public HashSet<GemType> GetRecommendGemType() {
            HeroGemType.Clear();
            HeroGemType.UnionWith(Heroes.Where(s => s.IsAlive)
                                            .SelectMany(s => s.GemTypes));
            return HeroGemType;
        }
    }
}