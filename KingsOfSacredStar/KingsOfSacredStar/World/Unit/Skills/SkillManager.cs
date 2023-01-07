using System.Collections.Generic;
using System.Linq;
using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.World.Unit.Skills
{
    internal sealed class SkillManager
    {
        public Dictionary<Skills, ISkill> SkillList { get; }
        public ISkill ActiveSkill { get; private set; }
        private int mSkillPoints;

        public SkillManager(Players player, ContentManager content)
        {
            SkillList = new Dictionary<Skills, ISkill>
            {
                [Skills.Skill1] = new Skill1(player, 0, content),
                [Skills.Skill2] = new Skill2(player, 0, content),
                [Skills.Skill3] = new Skill3(player, 0, 16, content)
            };
            mSkillPoints = 1;
        }

        public string GetName(Skills skill)
        {
            return SkillList[skill].Name;
        }

        public int GetManaCost(Skills skill)
        {
            return SkillList[skill].ManaCost;
        }

        public int GetRemainingCooldown(Skills skill)
        {
            return SkillList[skill].GetRemainingCooldown();
        }

        public string Serialize()
        {
            var data = Enumerable.Repeat(mSkillPoints.ToString(), 1)
                .Concat(SkillList.Select(entry => (int) entry.Key + "/" + entry.Value.GetLevel()));

            return string.Join(" ", data);
        }

        public void Execute(Skills skill)
        {
            SkillList[skill].Execute();
        }

        public void Update(Vector2 position, GameTime gameTime)
        {
            ActiveSkill = null;
            foreach (var skill in SkillList.Values)
            {
                skill.Update(position, gameTime);
                if (skill.IsActive)
                {
                    ActiveSkill = skill;
                }
            }

        }

        public void AddSkillPoint()
        {
            mSkillPoints += 1;
        }

        public int GetSkillPoints()
        {
            return mSkillPoints;
        }

        public void SetSkillLevel(Skills skill, int level)
        {
            for (var i = 0; i < level; i++)
            {
                SkillList[skill].LevelUp();
            }
        }

        public void LevelUp(Skills skill)
        {
            if (mSkillPoints == 0)
            {
                return;
            }

            if (SkillList[skill].LevelUp())
            {
                mSkillPoints -= 1;
            }
        }

        public int GetLevel(Skills skill)
        {
            return SkillList[skill].GetLevel();
        }
    }
}