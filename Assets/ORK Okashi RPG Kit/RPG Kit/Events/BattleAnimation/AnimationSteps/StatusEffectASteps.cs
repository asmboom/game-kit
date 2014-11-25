
using System.Collections;


public class CheckStatusEffectAStep : AnimationStep
{
	public CheckStatusEffectAStep(BattleAnimationType t) : base(t)
	{
		
	}
	
	public override void Execute(BattleAnimation battleAnimation)
	{
		bool check = false;
		if(StatusOrigin.USER.Equals(this.statusOrigin) && 
			battleAnimation.battleAction.user != null && 
			battleAnimation.battleAction.user.prefabInstance != null)
		{
			check = battleAnimation.battleAction.user.IsEffectSet(this.number);
		}
		else if(StatusOrigin.TARGET.Equals(this.statusOrigin))
		{
			check = true;
			for(int i=0; i<battleAnimation.battleAction.target.Length; i++)
			{
				if(battleAnimation.battleAction.target[i] != null)
				{
					if(!battleAnimation.battleAction.target[i].IsEffectSet(this.number))
					{
						check = false;
						break;
					}
				}
			}
		}
		if(check == this.show)
		{
			battleAnimation.StepFinished(this.next);
		}
		else
		{
			battleAnimation.StepFinished(this.nextFail);
		}
	}
	
	public override Hashtable GetData()
	{
		Hashtable ht = base.GetData();
		ht.Add("number", this.number.ToString());
		ht.Add("statusorigin", this.statusOrigin.ToString());
		ht.Add("show", this.show.ToString());
		ht.Add("nextfail", this.nextFail.ToString());
		return ht;
	}
}
