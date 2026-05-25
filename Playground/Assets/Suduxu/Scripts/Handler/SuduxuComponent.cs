using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class SuduxuComponent : MonoBehaviour
{
    [SerializeField] public bool launchOnStart;
    [SerializeField] public bool stopOnDestroy;

    [Injected] private Suduxu suduxu;

    private void Start()
    {
        if (launchOnStart && !suduxu.IsRunning())
        {
            suduxu.Launch();
        }

        suduxu.Log.OnLog += log => { Debug.Log(log); };

        StartCoroutine(LoadConfig(500));
    }

    JsonSerializerSettings settings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        },
        Converters =
        {
            new Newtonsoft.Json.Converters.StringEnumConverter()
        },
        NullValueHandling = NullValueHandling.Include,
        MissingMemberHandling = MissingMemberHandling.Error,
        DefaultValueHandling = DefaultValueHandling.Populate
    };

    private SuduxuConfig GetConfig()
    {
        return _ReadJson<SuduxuConfig>(SuduxuRaw.config, settings);
    }

    private T _ReadJson<T>(Func<IntPtr> nativeCall, JsonSerializerSettings settings = null)
    {
        IntPtr ptr = nativeCall();
        try
        {
            string json = Marshal.PtrToStringAnsi(ptr);

            if (settings == null)
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
        }
        finally
        {
            SuduxuRaw.free(ptr);
        }
    }

    private void OnDestroy()
    {
        if (stopOnDestroy && suduxu.IsRunning())
        {
            suduxu.Stop();
        }
    }

    private void Update()
    {
        suduxu.Input.Tick(Time.deltaTime);
    }

    private IEnumerator LoadConfig(float millis)
    {
        yield return new WaitForSecondsRealtime(millis / 1000.0f);
        SuduxuConfig.Instance = GetConfig();
    }
}