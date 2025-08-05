using System;

[Serializable]
public abstract class AbilityEffect
{
    public abstract void ApplyEffect(Entity _originEntity, Entity _targetEntity);
}