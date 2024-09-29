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

public enum WeaponType
{
    NotWeapon = -1,

    BareHand = 0,
    Onehanded = 1,
    Twohanded = 2,
    Consumable = 3,
}


public enum EnemyState
{
    Move,
    Attack,
    Run,
    Die,
    Attack2,
    Attack3,
}

public enum EffectType
{
    Blood,

}

public enum EquipPart
{
    Head,
    Chest,
    Leg,
    Hand,
    Foot,
    Weapon,
}

public enum ItemSize
{
    // 앞의 4개의 비트는 y의 크기를 나타내고 뒤의 4비트는 x의 크기를 나타냄
    slot1x1 = 0b0001_0001,
    slot1x2,
    slot1x3,
    slot1x4,
    slot1x5,
    slot2x1 = 0b0010_0001,
    slot2x2,
    slot2x3,
    slot2x4,
    slot3x1 = 0b0011_0001,
    slot3x2,
    slot3x3,
    slot3x4,
    slot4x1 = 0b0100_0001,
    slot4x2,
    slot4x3,
    slot5x1 = 0b0101_0001,
    slot5x2,
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

public enum ButtonFunc
{
    MerchantButton,
    AdventureButton,
    StashButton,
    MerchantConsumButton,
    MerchantEquipButton,
    MerchantSellButton,
}