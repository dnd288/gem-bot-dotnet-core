using Sfs2X.Entities.Data;
using System.Collections;

namespace bot
{
    public class Hero {
        private int PlayerId { get; }
        public HeroIdEnum Id { get; }
        public string Name { get; set; }
        public List<GemType> GemTypes { get; } = new();
        public int MaxHp { get; set; }
        public int MaxMana { get; set; } // Mp
        public int Attack { get; set; }
        public int Hp { get; set; }
        public int Mana { get; set; }

        public bool IsAlive => Hp > 0;
        public bool IsFullMana => Mana >= MaxMana;

        public bool IsHeroSelfSkill => HeroIdEnum.SEA_SPIRIT == Id;

        public Hero(ISFSObject objHero) {
            PlayerId = objHero.GetInt("playerId");
            Id = EnumUtil.ParseEnum<HeroIdEnum>(objHero.GetUtfString("id"));
            Name = Id.ToString();
            Attack = objHero.GetInt("attack");
            MaxHp = objHero.GetInt("maxHp");
            Hp = objHero.GetInt("hp");
            Mana = objHero.GetInt("mana");
            MaxMana = objHero.GetInt("maxMana");

            var arrGemTypes = objHero.GetSFSArray("gemTypes");
            for (var i = 0; i < arrGemTypes.Count; i++) {
                GemTypes.Add(EnumUtil.ParseEnum<GemType>(arrGemTypes.GetUtfString(i)));
            }
        }

        public void updateHero(ISFSObject objHero) {
            Attack = objHero.GetInt("attack");
            Hp = objHero.GetInt("hp");
            Mana = objHero.GetInt("mana");
            MaxMana = objHero.GetInt("maxMana");
        }

        public string PrintGemType()
        {
            var str = string.Empty;
            foreach (var gemType in GemTypes)
            {
                str += gemType.ToString();
                str += "-";
            }

            return str;
        }
    }
}