using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameController : MonoBehaviour
{
    public Transform player_transform;

    private static List<GameObject> enemies = new List<GameObject>();

    public void Awake()
    {
        StreamReader file_reader = new StreamReader("Assets/Scripts/game_data.txt");
        Dictionary<string, float> game_data = new Dictionary<string, float>();
        string next_line = file_reader.ReadLine();
        while (next_line != null)
        {
            if (next_line.Trim() != "")
            {
                string[] line = next_line.Split(':');
                string key = line[0];
                float val = float.Parse(line[1].Trim());
                game_data.Add(key, val);
            }
            next_line = file_reader.ReadLine();
        }
        Player.SetPlayerVals(
            game_data["player_health"],
            game_data["player_meter_loss_rate"],
            game_data["player_move_speed"],
            game_data["player_accuracy"],
            game_data["player_range"],
            game_data["dark_fire_rate"],
            game_data["light_fire_rate"],
            game_data["light_damage"],
            game_data["dark_damage"]
        );

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
        return enemies;
    }

    void Update()
    {
        Vector3 new_pos = player_transform.position;
        new_pos.z = -10;
        transform.position = new_pos;
    }
}
