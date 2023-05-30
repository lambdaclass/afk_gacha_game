using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
These clases are used to parse the game_settings.json data 
*/
public class boardSize{
    public uint width { get; set;}
    public uint height { get; set;}
}

public class gameConfig {
    public boardSize board_size;
    public uint server_tickrate;
    public uint game_timeout;
}
