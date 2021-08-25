using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickObjectColor : MonoBehaviour
{
    public enum ColorOptions { DEFAULT, RED, ORANGE, MAGENTA, YELLOW, GREEN, BLUE, CYAN};
    
    [HideInInspector] public Color m_Color;
    [SerializeField] public ColorOptions m_ChosenColor;
    [SerializeField] bool m_IsLever;


    private void Awake()
    {
        SetColor();
        SetMeshColor();
    }    

    void SetColor()
    {
        switch (m_ChosenColor)
        {
            case ColorOptions.RED:
                m_Color = Color.red;
                break;
            case ColorOptions.ORANGE:
                m_Color = new Color(1, 0.5f, 0, 1);
                break;
            case ColorOptions.MAGENTA:
                m_Color = Color.magenta;
                break;
            case ColorOptions.YELLOW:
                m_Color = Color.yellow;
                break;
            case ColorOptions.GREEN:
                m_Color = Color.green;
                break;
            case ColorOptions.BLUE:
                m_Color = Color.blue;
                break;
            case ColorOptions.CYAN:
                m_Color = Color.cyan;
                break;
            default:
                Debug.Log("Error in " + gameObject.name + " Pick a color");
                break;
        }
    }
    void SetMeshColor()
    {
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

        if (m_IsLever)
        {
            foreach (MeshRenderer mesh in meshRenderers)
            {
                if (mesh.gameObject.name == "Handle Ball")
                {
                    mesh.material.color = m_Color;
                }
            }
        }
        else
        {
            foreach (MeshRenderer mesh in meshRenderers)
            {
                mesh.material.color = m_Color;
            }
        }                
    }
}
