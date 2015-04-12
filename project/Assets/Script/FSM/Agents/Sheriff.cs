using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FSM
{
    public class Sheriff : Agent
    {
        public Boolean OutlawSpotted = false;
		public int goldCarrying;
		public bool IsDead;
        // Here is the StateMachine that the Sheriff uses to drive the agent's behaviour
        private StateMachine<Sheriff> stateMachine;
        public StateMachine<Sheriff> StateMachine
        {
            get { return stateMachine; }
            set { stateMachine = value; }
        }

        // And it knows its bank balance at any point in time
        private int moneyInBank;
        public int MoneyInBank
        {
            get { return moneyInBank; }
            set { moneyInBank = value; }
        }

        public Sheriff()
            : base()
        {
            stateMachine = new StateMachine<Sheriff>(this);
            stateMachine.CurrentState = new PatrolRandomLocation();
            stateMachine.GlobalState = new SheriffGlobalState();

            Location = Location.sheriffsOffice;
        }

        // This method is invoked by the Game object as a result of XNA updates 
//        void Update()
//        {
//            stateMachine.Update();
//        }
		void Start()
		{
			StartCoroutine(PerformUpdate());
		}
		
		// Update is called once per frame
		IEnumerator PerformUpdate()
		{
			while (true)
			{
				stateMachine.Update();
				yield return new WaitForSeconds(0.8f);	
			}
			
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

        public Location ChooseNextLocation()
        {
            Location nextLocation = Location;
            while (nextLocation == Location.outlawCamp || nextLocation == Location)
			nextLocation = Location.saloon;

            return nextLocation;
        }
    }
}
