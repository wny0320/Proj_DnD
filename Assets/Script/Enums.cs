public enum TopoTags
{
    Untagged,
    wall,
    floor,
    pillar,
    stair,
}

public enum PlayerState
{
    Move,
    Crouch,
    Die,
}

public enum EnemyState
{
    Move,
    Attack,
    Run,
    Die,
}

public enum EnemyType
{
    Human,
    Monster
}

public enum EffectType
{
    Blood,

}

public enum EquipPart
{
    NonEquip,
    Head,
    Chest,
    Arm,
    Leg,
    Foot,
}

public enum ItemSize
{
    slot1x1,
    slot1x2,
    slot1x3,
    slot1x4,
    slot1x5,
    slot2x1,
    slot2x2,
    slot2x3,
    slot3x1,
    slot3x2,
}

public enum ItemType
{
    Equipment, // 장비들, 스택처리 안됨
    Consumable, // 소모품, 스택처리 2나 3까지
    Coin, // 파밍템중 동전류, 스택처리 10
    Antique, // 파밍템중 골동품류, 스택처리 안됨
}

public enum ItemRarity
{
    Junk,
    Poor,
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Unique,
}