using System;
using UnityEngine;

public class TeamEM : AEntityModule
{
    [SerializeField] private Team m_team;
    [SerializeField] private Team m_targetTeam;
    
    public Team team => m_team;
    public Team TargetTeam => m_targetTeam;
}