using UnityEngine;

[CreateAssetMenu(menuName = "General Settings")]
public class GeneralSettings : ScriptableObject
{
    private static GeneralSettings _GeneralSettings;

    private static GeneralSettings generalSettings
    {
        get
        {
            if (!_GeneralSettings)
            {
                _GeneralSettings = Resources.Load<GeneralSettings>($"GeneralSettings");
            }

            return _GeneralSettings;
        }
    }

    public static GeneralSettings Get()
    {
        return generalSettings;
    }
    
    [Header("Platform Settings")]
    public int PlatformPoolSize = 32;
    public Vector3 InitialPlatformScale = new (3f, 1f, 3f);
    public float PlatformMoveSpeed = 1f;
}