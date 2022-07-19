using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
/// <summary>
/// enum들은 여기에 작성할 것
/// Card관련 enum의 경우에는 [JsonConverter(typeof(StringEnumConverter))] 를 위에 꼭 붙일것
/// </summary>
public enum BoardStates { None, Wall, Minion }
public enum BoardColor { None, Player, Enemy }

[JsonConverter(typeof(StringEnumConverter))]
public enum States{ Normal, Attack, Move, Color }

[JsonConverter(typeof(StringEnumConverter))]
public enum EnemyState { Normal, Attack, Color }

[JsonConverter(typeof(StringEnumConverter))]
public enum AttackCardEffect { Alpha, Bravo, Charlie, Delta, Echo, None }

[JsonConverter(typeof(StringEnumConverter))]
public enum MoveCardEffect { Run, Slide, Flash, None }

[JsonConverter(typeof(StringEnumConverter)), Flags]
public enum MoveDirection { All, UDLR, Diagonal, Colored, Dangerous }

[JsonConverter(typeof(StringEnumConverter))]
public enum TriggerCondition { None, ColoredSpaceExists, EnemyWillAttack, PlayerHealthExceeds30, MoveCardInHand }
public enum ColorCardEffect {Color1, Color2, BreakWallAndColor, None}

public enum ColorTargetPosition {UpLeft, Up, UpRight, Left, Center, Right, DownLeft, Down, DownRight}
