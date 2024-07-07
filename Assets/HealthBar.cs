using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
	[HideInInspector] public int maxHealth;
	[HideInInspector] public int health;

	[SerializeField] private GameObject barPrefab;

	private GameObject bar;
	private Image background;
	private Slider slider;

	private Vector3 initialScale;

	void Start()
    {
		bar = Instantiate(barPrefab, FindObjectOfType<Canvas>().transform);
		background = bar.GetComponent<Image>();
		background.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.0f, 0.5f, 0.0f));
		slider = background.GetComponent<Slider>();
		slider.maxValue = maxHealth;
		slider.value = health;

		initialScale = background.transform.localScale;
	}

    void Update()
    {
		background.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.0f, 0.5f, 0.0f));
		background.transform.localScale = initialScale / Camera.main.orthographicSize * 5.0f;
		slider.maxValue = maxHealth;
		slider.value = health;
	}

	void OnDestroy()
	{
		Destroy(bar);
	}
}
