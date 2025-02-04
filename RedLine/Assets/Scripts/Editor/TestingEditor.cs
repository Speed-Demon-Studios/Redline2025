using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TestingEditor : EditorWindow
{
    GameObject m_prefab;
    ShipVariant m_variant;
    GameObject m_player;
    Vector3 m_spawnPos;
    string[] levels = new string[3] { "Easy", "Medium", "Hard" };
    int indexForLevel;

    string[] ships = new string[3] { "Splitwing", "Fulcrum", "Cutlass" };
    int indexForShip;

    [MenuItem("Window/Testing Panel")]
    public static void ShowMyEditor()
    {
        GetWindow<TestingEditor>("Testing Panel");
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        GameObject ship = EditorGUILayout.ObjectField("Player Ship", m_prefab, typeof(GameObject), false) as GameObject;

        ShipVariant variant = EditorGUILayout.ObjectField("Player Variant", m_variant, typeof(ShipVariant), false) as ShipVariant;

        GUILayout.Space(15f);

        Vector3 spawnPos = EditorGUILayout.Vector3Field("SpawnPos", m_spawnPos);

        GUILayout.Space(25f);

        if (m_player != null && Application.isPlaying)
        {
            EditorGUI.BeginChangeCheck();
            indexForLevel = EditorGUILayout.Popup(indexForLevel, levels);
            if (EditorGUI.EndChangeCheck())
            {
                switch (indexForLevel)
                {
                    case 0:
                        GameManager.gManager.difficultyChange = 0.8f;
                        break;
                    case 1:
                        GameManager.gManager.difficultyChange = 1f;
                        break;
                    case 2:
                        GameManager.gManager.difficultyChange = 1.2f;
                        break;
                }
            }
        }

        GUILayout.Space(35f);

        if (GUILayout.Button("Spawn Ship", GUILayout.Width(150f)))
        {
            SpawnShip();
        }

        if (GUILayout.Button("Switch Variant", GUILayout.Width(150f)))
        {
            SwitchModel();
        }

        if (m_player != null)
        {
            GUI.color = Color.red;
            if (GUILayout.Button("Delete Ship", GUILayout.Width(150f)))
            {
                DeleteTestShip();
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Changed Script");

            m_prefab = ship;
            m_variant = variant;
            m_spawnPos = spawnPos;
        }
    }

    private void SpawnShip()
    {
        if (m_prefab == null)
        {
            Debug.LogError("You need a ship in the prefab field to spawn a test ship");
        }
        else if(m_variant == null)
        {
            Debug.LogError("You need a variant in the variant field to spawn a test ship");
        }
        if(m_player != null)
        {
            Debug.LogError("Test ship is already in the scene. cant spawn another ship");
        }
        else
        {
            m_player = Instantiate(m_prefab);
            m_player.transform.position = m_spawnPos;

            m_player.GetComponent<ShipsControls>().VariantObject = m_variant;

            m_player.GetComponent<ShipsControls>().isTestShip = true;

            m_player.GetComponent<ActionMappingControl>().UpdateActionMapForRace();

            m_player.GetComponent<PlayerInputScript>().enabled = true;
            m_player.GetComponent<PlayerInputScript>().cam.gameObject.SetActive(true);

            m_player.GetComponent<ShipsControls>().AttachModels();

            m_player.GetComponent<ShipsControls>().enabled = true;

            m_player.GetComponent<ShipsControls>().Initialize();
            //m_player.GetComponent<PlayerInputScript>().Inistialize();
        }
    }

    private void SwitchModel()
    {
        if (m_player == null)
        {
            Debug.LogError("There is no test ship in the scene");
        }
        else
        {
            m_player.GetComponent<ShipsControls>().VariantObject = m_variant;
            m_player.GetComponent<ShipsControls>().AttachModels();
        }
    }

    private void DeleteTestShip()
    {
        if (m_player == null)
        {
            Debug.LogError("There is no test ship in the scene");
        }
        else
        {
            DestroyImmediate(m_player);
            m_player = null;
        }
    }
}
