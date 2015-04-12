using UnityEngine;
using System.Collections;

namespace FSM
{
    public abstract class State<T>
    {
        abstract public void Enter(T agent);
        abstract public void Execute(T agent);
        abstract public void Exit(T agent);
        abstract public bool OnMessage(T agent, Telegram telegram);
		abstract public bool OnSenseEvent(T agent, Sense sense);
    }

	abstract public class TravelToTarget<T> : State<T>
	{
		protected State<T> targetState;
		public Vector3 targetPos;
		public Location targetLoc;
	}
}
