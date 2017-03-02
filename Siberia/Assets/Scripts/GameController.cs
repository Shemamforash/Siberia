using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class GameController : MonoBehaviour
{
    private static List<GameObject> enemies = new List<GameObject>();
    private static Dictionary<string, float> game_data = new Dictionary<string, float>();
    private static bool read_data = false;

    public TextAsset game_data_file;

    public void Awake()
    {
        if (!GameController.read_data)
        {
            string[] data_lines = Regex.Split (game_data_file.text, "\n|\r|\r\n" );

            foreach(string next_line in data_lines)
            {
                if (next_line.Trim() != "")
                {
                    string[] line = next_line.Split(':');
                    string key = line[0];
                    float val = float.Parse(line[1].Trim());
                    game_data.Add(key, val);
                }
            }
            Player.SetPlayerVals(
                game_data["player_health"],
                game_data["player_meter_loss_rate"],
                game_data["player_move_speed"],
                game_data["player_armour"],
                game_data["player_accuracy"],
                game_data["player_range"],
                game_data["dark_fire_rate"],
                game_data["light_fire_rate"],
                game_data["light_damage"],
                game_data["dark_damage"]
            );
            GameController.read_data = true;
        }
    }

    public static Dictionary<string, float> GetGameData()
    {
        return game_data;
    }

    public static void RegisterEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    public static void UnregisterEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    public static List<GameObject> Enemies()
    {
        for(int i = enemies.Count - 1; i >= 0; --i){
            if(enemies[i] == null){
                enemies.RemoveAt(i);
            }
        }
        return enemies;
    }
}
