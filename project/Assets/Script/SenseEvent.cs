using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        static AStar propogator = new AStar(AStar.SearchType.SensePropogation);

        static float SENSE_RANGE = 4.0f;

        public static void UpdateSensors()
        {
            // Agents pairwise check
            for (int i = 0; i < AgentManager.GetCount(); ++i)
            {
				//between two agents
                Agent a1 = AgentManager.GetAgent(i);
                for (int j = 0; j < AgentManager.GetCount(); ++j)
                {
                    if (i != j)
                    {
                        Agent a2 = AgentManager.GetAgent(j);

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
