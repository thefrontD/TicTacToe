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
public enum BoardObject { None, Player, Wall, Minion }

[JsonConverter(typeof(StringEnumConverter))]
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
public enum ColorCardEffect {ColorAndMove, Color, Color3, Color4, Color5, Color6, Color7, Color8, Color9, Color10, Color11}
public enum TriggerCondition { None, ColoredSpaceExists, EnemyWillAttack, PlayerHealthExceeds30, MoveCardInHand }

public enum ColorTargetPosition {All, P1, P4, P5, C, V, H, P3V, P3H}
