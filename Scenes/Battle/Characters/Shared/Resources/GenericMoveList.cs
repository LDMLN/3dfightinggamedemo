using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class GenericMoveList : Resource
{
    private static readonly Dictionary<string, Dictionary<string, string>> LeftSideMoveList = new Dictionary<string, Dictionary<string, string>> {
            {
                "66", new Dictionary<string, string> {
                    { "name", "Dash" },
                    { "state", "MovingState" }
                }
            },
            {
                "44", new Dictionary<string, string> {
                    { "name", "BackDash" },
                    { "state", "MovingState" }
                }
            },
            {
                "236P", new Dictionary<string, string>{
                    { "name", "Fireball" },
                    { "state", "AttackingState" }
                }
            }
    };
    
    public Dictionary<string, Dictionary<string, string>> GetLeftSideGenericMoveList()
    {
        return LeftSideMoveList;
    }

    /*
    private static readonly Dictionary<string, Dictionary<string, string>> LeftSideMoveList = new Dictionary<string, Dictionary<string, string>> {
            {
                "236", new Dictionary<string, string> {
                    { "name", "Fireball" },
                    { "state", "Attack" }
                }
            },
            {
                "63236", new Dictionary<string, string> {
                    { "name", "Dragon-Upper" },
                    { "state", "Attack" }
                }
            }
    };
    */

}
