using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class SingleDialogue
{
    public string Name => _name;
    public string Context => _context;
    public string PortraitLocation => _portraitLocation;

    private string _name;
    private string _context;
    private string _portraitLocation;

    public SingleDialogue(string name, string context, string portraitLocation)
    {
        this._name = name;
        this._context = context;
        this._portraitLocation = portraitLocation;
    }
}

public class DialogueHolder
{
    public Queue<SingleDialogue> Dialogues => _dialogues;
    public List<string> DialoguePeople => _dialoguePeople;
    public List<string> InitialSetting => _initialSetting;

    [JsonProperty] private Queue<SingleDialogue> _dialogues;
    [JsonProperty] private List<string> _dialoguePeople;
    [JsonProperty] private List<string> _initialSetting;

    public DialogueHolder(Queue<SingleDialogue> dialogues, List<string> dialoguePeople, List<string> initialSetting)
    {
        this._dialogues = dialogues;
        this._dialoguePeople = dialoguePeople;
        this._initialSetting = initialSetting;
    }

    public bool CheckDialogueFin()
    {
        if (_dialogues.Count == 0) return true;
        else return false;
    }

    public SingleDialogue GetNextDialogue()
    {
        return _dialogues.Dequeue();
    }
}