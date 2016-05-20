using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor
{
    public  class EnumData
    {
        public static Dictionary<int, string> GetProfessionData()
        {
            Dictionary<int, string> choices = new Dictionary<int, string>();
            choices.Add(0, "卫军");
            choices.Add(1, "猛将");
            choices.Add(2, "射手");
            choices.Add(3, "谋士");
            return choices;
        }
    }

    public enum EProfession
    {
        卫军,
        猛将,
        射手,
        谋士,
    }

    public enum EAttackRange
    {
        近战,
        远程,
    }

    public enum EElement
    {
        物理 = 0,
        法术 = 1,
    }

    public enum ENature
    {
        无 = 0,
        忠 = 1,
        义,
        礼,
        智,
        信,
        勇,
        恶,
        毒 = 8,
        骚,
        豪,
        萌,
        艳,
        淑,
        慧,
        威 = 15,
        枭,
        贤,
        毅,
        奸,
        仁,
        韧,
    }

    public enum EAddition
    {
        无   = 0,
        攻击 = 1,
        防御 = 2,
        速度 = 3,
        连击 = 4,
        格挡 = 5,
        反击 = 6,
        暴击率 = 7,
        暴击伤害 = 8,
        生命 = 9,
    }

    public enum EWarriorFrom
    {
        招募 = 0,
        杂兵 = 5,
        Boss = 8,
    }

    public enum ESkillTarget
    {
        我方 = 0,
        敌方 = 1,
        自己 = 2,
    }

    // TODO thy 支持批注详情
    public enum ESkillSelectRule
    {
        随机 = 0,
        前排 = 1,
        后排 = 2,
        血最少 = 3,
        防最弱 = 4,
        攻最强 = 5,
    }

    public enum ESkillOpportunity
    {
        主动技能 = 0,
        战斗开始时 = 1,
        攻击时 = 2,
        普通攻击时 = 3,
        暴击时 = 4,   
        格挡时 = 5,
        被攻击时 = 6,
        敌人死亡时 = 7,
        自身死亡时 = 9,
    }

    public enum ESkillFunction
    {
        加Buff,   // AddBuff
        治疗,     // Cure
        伤害,     // Damage
        精华,     // Purge
        复活,     // Revive
    }

    public enum ESkillSubtype
    {
        主动 = 1,
        被动 = -1,
    }

    public enum ELevelType
    {
        普通关卡 = 1,
        精英关卡 = 2,
        列传关卡 = 3,
    }
}
