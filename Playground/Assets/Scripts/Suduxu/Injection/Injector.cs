using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// A provider for managing dependency injection within the system.
/// </summary>

public class Injector : MonoBehaviour
{
    private void Awake()
    {
        InjectAllDependencies();
    }

    private void InjectAllDependencies()
    {
        var components = FindObjectsOfType<MonoBehaviour>();

        foreach (var component in components)
        {
            InjectDependencies(component);
            AutowireComponents(component);
        }
    }

    private void InjectDependencies(object target)
    {
        var targetType = target.GetType();

        while (targetType != null && targetType != typeof(MonoBehaviour))
        {
            var fields = targetType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                   .Where(f => Attribute.IsDefined(f, typeof(InjectedAttribute)));

            foreach (var field in fields)
            {
                var injectableType = field.FieldType;

                var injectableInstance = GetComponentsInChildren(injectableType, true)
                                            .FirstOrDefault() as MonoBehaviour;

                if (injectableInstance == null)
                {
                    Debug.LogError($"No injectable of type {injectableType.Name} found for {target.GetType().Name}");
                    continue;
                }

                field.SetValue(target, injectableInstance);
            }

            targetType = targetType.BaseType;
        }
    }

    private void AutowireComponents(object target)
    {
        var targetType = target.GetType();
        var fields = targetType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                               .Where(f => Attribute.IsDefined(f, typeof(AutowiredAttribute)));

        foreach (var field in fields)
        {
            var componentType = field.FieldType;

            if (componentType.IsSubclassOf(typeof(Component)))
            {
                var component = (target as Component)?.gameObject.GetComponent(componentType);

                if (component == null)
                {
                    component = (target as Component)?.gameObject.AddComponent(componentType);
                }

                field.SetValue(target, component);
            }
            else
            {
                Debug.LogError($"AutoAdd attribute can only be applied to MonoBehaviour fields in {targetType.Name}");
            }
        }
    }
}