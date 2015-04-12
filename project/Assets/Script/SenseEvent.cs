
using System.Collections;
using UnityEngine;

namespace FSM
{    
    public enum SenseType
    {
        Sight,
        Hearing,
        Smell
    };

    public struct Sense
    {
        public string Sender;
        public string Receiver;
        public SenseType senseType;

        public Sense(string s, string r, SenseType st)
        {
            Sender = s;
            Receiver = r;
            senseType = st;
        }
    }


    public static class SenseEvent
    {
        static AStar propogator = new AStar();

        static float SENSE_RANGE = 4.0f;

        public static void UpdateSensors()
        {
			var agentManager = Object.FindObjectOfType<AgentManager>();
			for (int i = 0; i < agentManager.GetCount(); ++i)
            {
				//between two agents
				Agent a1 = agentManager.GetAgent(i);
				for (int j = 0; j < agentManager.GetCount(); ++j)
                {
                    if (i != j)
                    {
						Agent a2 = agentManager.GetAgent(j);

                        // If close enough to detect
						if (Vector3.Distance(a1.transform.position, a2.transform.position) < SENSE_RANGE)
                        {
                            // Propogate the sense
                            if (propogator.PropogateSense(a1.transform.position, a2.transform.position))
                            {
                                // Sense the agent
                                Sense sense = new Sense(a2.name, a1.name, SenseType.Sight);
                                a1.HandleSenseEvent(sense);
                            }
                        }
                    }
                }
            }
        }
    }
}
