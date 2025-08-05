using System;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTargetingEM : TargetingEM
{
    private PlayerAbilityEM m_playerAbilityEM;

    protected override void Awake()
    {
        base.Awake();
        m_playerAbilityEM = m_abilityEM as PlayerAbilityEM;
    }

    protected override void Update()
    {
        //Check for inputs before base because it returns if there's no m_mainTarget
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(m_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
                    m_abilityEM.UseAbility(m_playerAbilityEM.LeftClickAbility, new AbilityContextBuilder()
                        .SetTargetPosition(hit.point.OffsetY(1))
                        .SetOwnerEntity(m_linkedEntity)
                        .SetTargetEntity(null)
                        .Build());
            }
        
            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(m_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
                    m_abilityEM.UseAbility(m_playerAbilityEM.RightClickAbility, new AbilityContextBuilder()
                        .SetTargetPosition(hit.point.OffsetY(1))
                        .SetOwnerEntity(m_linkedEntity)
                        .SetTargetEntity(null)
                        .Build());
            }
        }
        
        base.Update();
    }
}