using System.Collections.Generic;

namespace mqfDecryptor.Models
{
    public class MqfFile
    {
        public MqfFileObjects mqfFileObjects { get; set; }
    }

    public class MqfFileObjects
    {
        public byte[] fileFormat { get; set; }
        public int questType { get; set; }
        public byte[] unkBytes { get; set; }
        public List<CardObjects> cardObj { get; set; }
        public List<CardRewards> cardRewards { get; set; }
        public CardRewards2 cardRewards2 { get; set; }

    }

    public class CardObjects
    {
        public ushort reqType { get; set; }
        public byte type { get; set; }
        public byte mapId { get; set; }
        public byte limitCount { get; set; }
        public byte weaponClass { get; set; }
        public ushort weaponId { get; set; }
        public byte[] unkBytes1 { get; set; }
    }

    public class CardRewards
    {
        public int gp { get; set; }
        public int xp { get; set; }
        public int medals { get; set; }
        public List<CardRewardsObjects> cardRewardsObjects { get; set; }
    }

    public class CardRewardsObjects
    {
        public int unk { get; set; }
        public int type { get; set; }
        public int itemId { get; set; }
        public int itemCount { get; set; }
    }

    public class CardRewards2
    {
        public int goldResult { get; set; }
        public byte[] unkBytes { get; set; }
        public List<CardRewards2Objects> cardRewardsObjects { get; set; }
    }

    public class CardRewards2Objects
    {
        public int unkI { get; set; }
        public int itemType { get; set; } //1 - unidade | 2 - dias
        public int itemId { get; set; }
        public int itemCount { get; set; }
    }
}

