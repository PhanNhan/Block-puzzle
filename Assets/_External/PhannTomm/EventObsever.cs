using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyEventSystem
{
    public enum GameEvent
    {
        GE_NONE = 0,

        // Gameplay
        GE_LINE_REMOVE,
        GE_PUT_BLOCK,
        GE_ACTIVED_BLOCK,
        GE_DESTROY_ENEMY,
        GE_END_GAME_MOMENT,
        GE_UPDATE_TILE,
        GE_HOTZONE_SPAWNED,
        GE_SCORE_BOOSTER,
        GE_COMBOFX_COMPLETED,
        GE_Elementals_Collected,
        GE_Completed_Order,
        GE_Mana_Collect,
        GE_Active_Item,
        GE_Deactive_Item,

        // Non-gameplay
        GE_POPUP_CALLBACK,
        GE_LATEST_LEVEL_CHANGED,
        GE_CLAIM_DAILY_REWARD,
        GE_COIN_CHANGED,
		GE_HEART_CHANGED,
		GE_CHECK_UNLOCK_ADVENTURE,
        GE_THEME_CHANGED,
        GE_THEME_COLOR_CHANGED,
        GE_BOUGHT_ONE_MORE_CHANCE,

		// Theme Manager
		GE_THEME_UPDATE_UNLOCK,
		GE_THEME_HAVE_NOTIFICATION,
		GE_THEME_UNLOCK_VIEW,

		GE_LOGIN_FACEBOOK_SUCCESS,
		GE_NEW_SESSION,
    }

    public class EventObsever
    {

        static EventObsever _instance;

        public static EventObsever Instance {
            get {
                if (_instance == null)
                    _instance = new EventObsever ();
                return _instance;
            }
        }

        static Dictionary<GameEvent, Delegate> eventTable = new Dictionary<GameEvent, Delegate>();

        public void RegisterEvent (GameEvent gameEvent, Action action)
        {
            if (!eventTable.ContainsKey (gameEvent))
                eventTable [gameEvent] = action;
            else
                eventTable [gameEvent] = (Action)eventTable [gameEvent] + action;
        }

        public void RegisterEvent<T>(GameEvent gameEvent, Action<T> action)
        {
            if (!eventTable.ContainsKey (gameEvent))
                eventTable [gameEvent] = action;
            else
                eventTable [gameEvent] = (Action<T>)eventTable [gameEvent] + action;
        }

        public void UnregisterEvent (GameEvent gameEvent, Action action)
        {
            if (eventTable [gameEvent] != null)
                eventTable [gameEvent] = (Action)eventTable [gameEvent] - action;
            else
                eventTable.Remove (gameEvent);
        }

        public void UnregisterEvent<T>(GameEvent gameEvent, Action<T> action)
        {
            if (eventTable [gameEvent] != null)
                eventTable [gameEvent] = (Action<T>)eventTable [gameEvent] - action;
            else
                eventTable.Remove (gameEvent);
        }

        public void BroadcastEvent(GameEvent gameEvent)
        {
            Delegate d;
            if (eventTable.TryGetValue (gameEvent, out d)) {
                Action action = d as Action;
                if (action != null)
                    action ();
            }
        }

        public void BroadcastEvent<T>(GameEvent gameEvent, T data)
        {
            Delegate d;
            if (eventTable.TryGetValue (gameEvent, out d)) {
                Action<T> action = d as Action<T>;
                if (action != null)
                    action (data);
            }
        }
    }
}
