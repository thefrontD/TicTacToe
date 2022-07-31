using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
/// <summary>
/// enum들은 여기에 작성할 것
/// Card관련 enum의 경우에는 [JsonConverter(typeof(StringEnumConverter))] 를 위에 꼭 붙일것
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum CardType { Attack, Move, Color }

[JsonConverter(typeof(StringEnumConverter))]
public enum BoardObject { None, Player, Wall, Minion }

[JsonConverter(typeof(StringEnumConverter))]
public enum BoardColor { None, Player, Enemy }

[JsonConverter(typeof(StringEnumConverter))]
public enum States{ Normal, Attack, Move, Color }

//[JsonConverter(typeof(StringEnumConverter))]
//public enum EnemyState { Normal, Attack, Color }

[JsonConverter(typeof(StringEnumConverter))]
public enum AttackCardEffect { Alpha, Bravo, Charlie, Delta, Echo, None }

[JsonConverter(typeof(StringEnumConverter))]
public enum MoveCardEffect { Run, Slide, Flash, None }

[JsonConverter(typeof(StringEnumConverter)), Flags]
public enum MoveDirection { All, UDLR, Diagonal, Colored, Dangerous }

[JsonConverter(typeof(StringEnumConverter))]
public enum ColorCardEffect 
{
    ColorAnyAndMove,
    Color, 
    ColorCross, 
    ColorCross2, 
    ColorCloseBlock, 
    ColorAny, 
    ColorVertical, 
    ColorHorizontal, 
    ColorVertical2, 
    ColorHorizontal2,
    ColorAny2s
}


[JsonConverter(typeof(StringEnumConverter))]
public enum TriggerCondition
{
    None, 
    Attacked,
    PlayerInColoredSpace,
    EnemyWillAttack, EnemyWillWall, EnemyWillMinion, EnemyWillShield,
    PlayerWall, PlayerNotWall,
    OnlyAttackCardInHand, OnlyMoveCardInHand, OnlyColorCardInHand,
    ColoredSpaceExists,
    Bingo1=31, Bingo2=32, Bingo3=33,
    CardInHand1=41, CardInHand2=42, CardInHand3=43, CardInHand4=44, CardInHand5=45,
    AttackCardInHand1=51, AttackCardInHand2=52, AttackCardInHand3=53, AttackCardInHand4=54, AttackCardInHand5=55,
    ColorCardInHand1=61, ColorCardInHand2=62, ColorCardInHand3=63, ColorCardInHand4=64, ColorCardInHand5=65,
    MoveCardInHand1=71, MoveCardInHand2=72, MoveCardInHand3=73, MoveCardInHand4=74, MoveCardInHand5=75,
    PlayerHealthExceeds10 = 110, PlayerHealthExceeds30=130
}

[JsonConverter(typeof(StringEnumConverter))]
public enum AdditionalEffectCondition
{
    None, 
    DestroyShield, DestroyWall, DestroyMinion, DestroyWallOrMinion,
    MonsterWillAttack, MonsterWillWall, MonsterWillMinion, MonsterWillShield,
    PlayerInColoredSpace,
    MakeBingo
}

[JsonConverter(typeof(StringEnumConverter))]
public enum AdditionalEffect
{
    None,
    Shield10,
    MonsterHp1,
    PlayerHp10,
    DMG10,
    Move,
    Color,
    Re,
    Mana1, 
    Draw1, 
    Mana1Draw1,
    Dump1, DumpALL, DumpAttackCard1, DumpMoveCard1, DumpColorCard1,
    Delete
}


[JsonConverter(typeof(StringEnumConverter))]
public enum ColorTargetPosition {All, P1, P4, P5, C, V, H, P3V, P3H}
[JsonConverter(typeof(StringEnumConverter))]
public enum ColorTargetNum {Target1, Target2, Target3, Target4, Target5, T, Hand}



[JsonConverter(typeof(StringEnumConverter))]
public enum EnemyAction
{
    RowAttack, ColAttack, AllAttack, ColorAttack, UnColorAttack,
    WallSummon=10, MobSummon,
    PowerIncrease=20, DamageDecrease, HpHealing, ArmorHealing,
    PlayerPowerDecrease=30, PlayerDamageIncrease, DrawCardDecrease, CardCostIncrease
}
public enum Debuff { PowerDecrease, DamageIncrease, DrawCardDecrease, CardCostIncrease }