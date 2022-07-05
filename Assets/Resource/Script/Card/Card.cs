using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

/// <summary>
/// Card 기본 클래스, AttackCard를 비롯한 각종 Card로 나눌 예정
/// 각 Card들이 가져야하는 필수적인 부분은 Card class에 abstract method선언 후 작성 부탁드립니다.
/// 실제로 Card를 Rendering 및 Animating 하는 부분은 CardUI라는 Script로 새로 작성 예정
/// </summary>
public abstract class Card {
    //
    private string cardName;
    public string CardName{
        get { return cardName; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public List<States> StatesList;

    public Card(string cardName, List<States> StatesList){
        this.cardName = cardName;
        this.StatesList = StatesList;
    }
    
    /// <summary>
    /// Card마다 가질 수 있는 사용시의 개별적인 효과는 usingCardSpecific에서 작성할 것
    /// </summary>
    public abstract void usingCardSpecific();

    public void usingCard()
    {
        foreach (States states in StatesList)
        {
            PlayerManager.Instance.StatesQueue.Enqueue(states);
        }
        
        usingCardSpecific();
        
        
    }
}
