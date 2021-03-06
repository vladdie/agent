﻿using UnityEngine;
using System.Collections;


namespace FSM
{
    public class Undertaker : Agent
    {
        public int CorpseID = -1;
		public string name = "Undertaker";
		//public string name = "Undertaker";
        // Here is the StateMachine that the Outlaw uses to drive the agent's behaviour
        private StateMachine<Undertaker> stateMachine;
        public StateMachine<Undertaker> StateMachine
        {
            get { return stateMachine; }
            set { stateMachine = value; }
        }

        public Undertaker()
            : base()
        {
            stateMachine = new StateMachine<Undertaker>(this);
            stateMachine.CurrentState = new HoverInTheOffice();
            stateMachine.GlobalState = new UndertakerGlobalState();
			//name = "Undertaker";
            Location = Location.undertakers;
			ID = 5;
        }

        // This method is invoked by the Game object as a result of XNA updates 
//        public override void Update()
//        {
//            stateMachine.Update();
//        }

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
