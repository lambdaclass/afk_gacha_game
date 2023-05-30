using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/*
These clases are used to parse the game_settings.json data 
*/

public class GameSettings
{
    public string path { get; set; }

    private class boardSize{
        public uint width { get; set;}
        public uint height { get; set;}
    }

    private class Settings {
        public boardSize board_size;
        public uint server_tickrate;
        public uint game_timeout;
    }

    public GameConfig parseSettings(){
        string jsonText = File.ReadAllText(this.path);
        Settings settings = JsonConvert.DeserializeObject<Settings>(jsonText);
        BoardSize bSize = new BoardSize {Width = settings.board_size.width, Height = settings.board_size.height};

        GameConfig gameConfig = new GameConfig {
            BoardSize = bSize,
            ServerTickrate = settings.server_tickrate,
            GameTimeout = settings.game_timeout
        };

        return gameConfig;
    }
}
