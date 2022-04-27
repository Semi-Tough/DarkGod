/****************************************************
	文件：SkillManager.cs
	作者：wzq
	邮箱: 1693416984@qq.com
	日期：2022年03月30日 星期三 15:00   	
	功能：技能管理器
*****************************************************/

using PEProtocol;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private ResourceService _resourceService;
    private TimerService _timerService;

    public void Init()
    {
        _resourceService = ResourceService.instance;
        _timerService = TimerService.instance;

        PeCommon.Log("技能管理器加载完成");
    }

    public void SetAttackEffect(EntityBase entity, int skillId)
    {
        SkillConfig skillConfig = _resourceService.GetSkillConfig(skillId);

        entity.SetAction(skillConfig.skillId);
        entity.SetEffectState(skillConfig.effectName, skillConfig.skillTime);
        CalculateSKillMove(entity, skillConfig);
        entity.canControl = false;
        entity.SetDirection(Vector2.zero);

        _timerService.AddTimeTask(entity.Idle, skillConfig.skillTime, 1);
    }

    private void CalculateSKillMove(EntityBase entity, SkillConfig skillConfig)
    {
        int delayTime = 0;
        foreach (int skillMoveId in skillConfig.skillMoveList)
        {
            SkillMoveConfig skillMoveConfig = _resourceService.GetSkillMoveConfig(skillMoveId);

            float skillMoveSpeed = skillMoveConfig.moveDis / ((float) skillMoveConfig.moveTime / 1000);
            delayTime += skillMoveConfig.delayTime;

            if (delayTime > 0)
            {
                _timerService.AddTimeTask(() => { entity.SetSkillMoveState(true, skillMoveSpeed); },
                    delayTime, 1);
            }
            else
            {
                entity.SetSkillMoveState(true, skillMoveSpeed);
            }

            delayTime += skillMoveConfig.moveTime;
            _timerService.AddTimeTask(() => { entity.SetSkillMoveState(false); },
                delayTime, 1);
        }
    }
}