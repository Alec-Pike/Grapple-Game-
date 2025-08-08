using UnityEngine;

public class DisplayGizmo : MonoBehaviour
{
	[SerializeField] private Color color = Color.red;
	[SerializeField] private float radius = 0.3f;
	[SerializeField] private bool wireFrame = false;

    private void OnDrawGizmos()
	{
		Gizmos.color = color;
		if (wireFrame) Gizmos.DrawWireSphere(transform.position, radius);
		else Gizmos.DrawSphere(transform.position, radius);
	}
}
