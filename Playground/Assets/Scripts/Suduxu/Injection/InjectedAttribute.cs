using System;

/// <summary>
/// Marks a field to be automatically injected with an instance 
/// from the corresponding dependency in the scene.<br/><br/>
/// 
/// Instances have to be marked with <strong>[Injectable]</strong>
/// </summary>

[AttributeUsage(AttributeTargets.Field)]
public class InjectedAttribute : Attribute
{

}