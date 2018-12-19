using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFSM<TFeed> {

    public State<TFeed> current;

    public void Update()
    {
        current.OnUpdate();
    }

    public void Feed(TFeed feed)
    {
        var next = current.GetTransition(feed);

        if(next != null)
        {
            current.OnExit();
            next.OnEnter();

            current = next;
        }
    }

    public EventFSM(State<TFeed> initialState)
    {
        current = initialState;
        current.OnEnter();
    }

}

public class State<TFeed>
{
    public string name;

    public Action OnEnter = delegate { };//Lo iniciamos vacio para no tener que chequear por null
    public Action OnUpdate = delegate { };
    public Action OnExit = delegate { };

    private Dictionary<TFeed, State<TFeed>> transitions = new Dictionary<TFeed, State<TFeed>>();

    public void AddTransition(TFeed feed, State<TFeed> next)
    {
        transitions[feed] = next;
    }

    public State<TFeed> GetTransition(TFeed feed)
    {
        if (transitions.ContainsKey(feed)) return transitions[feed];
        return null;
    }

    public State(string name)
    {
        this.name = name;
    }
}

public enum PlayerActions
{
    Moved,
    Attacked,
    Steady,
    Hurt,
    AttackReady,
    HurtReady,
    Death,
    Blocking,
    BlockingReady,
    Spell,
    SpellReady,
    Revive
}

public enum EnemyActions
{
    PlayerInSight,
    PlayerOutOfInterest,
    PlayerInRange,
    PlayerOutOfRange,
    Death,
    Damaged
}

public enum BossActions
{
    StartMoving,
    Moved,
    Shooted,
    Damaged,
    Death,
    DoneShooting,
    StartBattle
}
