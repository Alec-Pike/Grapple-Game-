using UnityEngine;

public static class EnablerUtil
{
    /*
    * by TonyLi on the Unity forums
    * Link: https://discussions.unity.com/t/array-of-components-using-behaviour-or-monobehaviour-array-list/558207/7
    */
    public static void SetComponentEnabled(Component component, bool value)
    {
        if (component == null) return;
        if (component is Renderer)
        {
            (component as Renderer).enabled = value;
        }
        else if (component is Collider)
        {
            (component as Collider).enabled = value;
        }
        else if (component is Animation)
        {
            (component as Animation).enabled = value;
        }
        else if (component is Animator)
        {
            (component as Animator).enabled = value;
        }
        else if (component is AudioSource)
        {
            (component as AudioSource).enabled = value;
        }
        else if (component is MonoBehaviour)
        {
            (component as MonoBehaviour).enabled = value;
        }
        else
        {
            Debug.Log("Don't know how to enable " + component.GetType().Name);
        }
    }

    public static void SetMultipleComponentsEnabled(Component[] components, bool value)
    {
        foreach (Component comp in components) SetComponentEnabled(comp, value);
    }
}