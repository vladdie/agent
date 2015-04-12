using UnityEngine;
using System.Collections;

namespace FSM
{
	public class Outlaw : Agent
	{
		public int BoredomCountdown = 0;
		//public int goldCarrying;
		public bool IsDead;
		public string name = "OutLaw";
		// Here is the StateMachine that the Outlaw uses to drive the agent's behaviour
		private StateMachine<Outlaw> stateMachine;
		public StateMachine<Outlaw> StateMachine
		{
			get { return stateMachine; }
			set { stateMachine = value; }
		}

		public int GoldCarrying
		{
			get { return goldCarrying; }
			set { goldCarrying = value; }
		}
		public Outlaw()
			: base()
		{
			stateMachine = new StateMachine<Outlaw>(this);
			stateMachine.CurrentState = new LurkInOutlawCamp();
			stateMachine.GlobalState = new OutlawGlobalState();
			ID = 3;
			Location = Location.outlawCamp;
			//name = "OutLaw";
		}

	
		// This method is invoked by the Game object as a result of XNA updates 
	    void Update()
		{
//			if (Location >= 0)
//			{
//				BoredomCountdown -= 1;
//			}
//			
//			stateMachine.Update();
		}
		void Start()
		{
			StartCoroutine(PerformUpdate());
		}
		
		// Update is called once per frame
		IEnumerator PerformUpdate()
		{
			while (true)
			{
				if (Location >= 0)
				{
					BoredomCountdown -= 1;
			    }
							
				stateMachine.Update();
				yield return new WaitForSeconds(0.8f);
				
			}
		}
		public bool Bored()
		{
			return (BoredomCountdown <= 0);
		}
		
		// This method is invoked when the agent receives a message
		public override bool HandleMessage(Telegram telegram)
		{
			return stateMachine.HandleMessage(telegram);
		}
		
		// This method is invoked when the agent senses
		public override bool HandleSenseEvent(Sense sense)
		{
			return stateMachine.HandleSenseEvent(sense);
		}
	}
}
