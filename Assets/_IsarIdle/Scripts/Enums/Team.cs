using System;

[Flags]
public enum Team
{
    None   = 0,
    Player = 1 << 0, // 1
    Enemy  = 1 << 1  // 2
}