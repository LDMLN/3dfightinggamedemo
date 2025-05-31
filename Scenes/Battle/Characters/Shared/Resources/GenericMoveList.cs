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
                    { "state", "Moving" }
                }
            },
            {
                "44", new Dictionary<string, string> {
                    { "name", "BackDash" },
                    { "state", "Moving" }
                }
            },
            {
                "88", new Dictionary<string, string> {
                    { "name", "UpDash" },
                    { "state", "Moving" }
                }
            },
            {
                "22", new Dictionary<string, string> {
                    { "name", "DownDash" },
                    { "state", "Moving" }
                }
            },
            {
                "236P", new Dictionary<string, string>{
                    { "name", "Fireball" },
                    { "state", "Attacking" }
                }
            }
    };

    private static readonly Dictionary<string, Dictionary<string, string>> RightSideMoveList = new Dictionary<string, Dictionary<string, string>> {
            {
                "66", new Dictionary<string, string> {
                    { "name", "BackDash" },
                    { "state", "Moving" }
                }
            },
            {
                "44", new Dictionary<string, string> {
                    { "name", "Dash" },
                    { "state", "Moving" }
                }
            },
            {
                "88", new Dictionary<string, string> {
                    { "name", "UpDash" },
                    { "state", "Moving" }
                }
            },
            {
                "22", new Dictionary<string, string> {
                    { "name", "DownDash" },
                    { "state", "Moving" }
                }
            },
            {
                "214P", new Dictionary<string, string>{
                    { "name", "Fireball" },
                    { "state", "Attacking" }
                }
            }
    };
    
    public Dictionary<string, Dictionary<string, string>> GetLeftSideGenericMoveList()
    {
        return LeftSideMoveList;
    }

    internal Dictionary<string, Dictionary<string, string>> GetRightSideGenericMoveList()
    {
        return RightSideMoveList;
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
