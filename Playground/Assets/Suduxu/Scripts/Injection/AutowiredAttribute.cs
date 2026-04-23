using System;

/// <summary>
/// Marks a field to be automatically assigned with a component from the GameObject.<br/>
/// If the component does not exist, it is added automatically (e.g., Rigidbody).
/// </summary>

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class AutowiredAttribute : Attribute
{

}