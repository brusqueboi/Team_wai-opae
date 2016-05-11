using UnityEngine;

public class UnderwaterFog : MonoBehaviour 
{
	public Color darkestColor = Color.blue;
	public Color lightestColor = Color.cyan;
	public float minDensity = 0.025f;
	public float maxDensity = 0.15f;
	public float densityVariability = 0.25f;
	public float colorVariability = 0.25f;

	public float densityBias = 0.5f;
	public float colorBias = 0.5f;

	void Start()
	{
		RenderSettings.fog = true;
		RenderSettings.fogMode = FogMode.Exponential;
		Update();
	}

    void Update()
    {
		colorBias = Mathf.Clamp((colorBias + (((Mathf.PerlinNoise(0.0f, Time.time) * 2.0f) - 1.0f) * colorVariability * Time.deltaTime)), 
			0.0f, 1.0f);
		RenderSettings.fogColor = Color.Lerp(darkestColor, lightestColor, colorBias);
		densityBias = Mathf.Clamp(densityBias + (
			((Mathf.PerlinNoise(0.0f, Time.time) * 2.0f) - 1.0f) * densityVariability), 
			0.0f, 1.0f);
		RenderSettings.fogDensity = minDensity + ((maxDensity - minDensity) * densityBias);
	}
	
}
