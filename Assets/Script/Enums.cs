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
    Equipment, // ����, ����ó�� �ȵ�
    Consumable, // �Ҹ�ǰ, ����ó�� 2�� 3����
    Coin, // �Ĺ����� ������, ����ó�� 10
    Antique, // �Ĺ����� ��ǰ��, ����ó�� �ȵ�
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