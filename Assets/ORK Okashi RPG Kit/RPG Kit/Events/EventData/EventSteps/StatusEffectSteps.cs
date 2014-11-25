
using System.Collections;

public class ChangeStatusEffectStep : EventStep
{
	public ChangeStatusEffectStep(GameEventType t) : base(t)
	{
		
	}
	
	public override void Execute(GameEvent gameEvent)
	{
		if(this.show)
		{
			Character[] cs = null;
			if(this.show2)
			{
				cs = GameHandler.Party().GetBattleParty();
			}
			else
			{
				cs = GameHandler.Party().GetParty();
			}
			for(int i=0; i<cs.Length; i++)
			{
				for(int j=0; j<this.effect.Length; j++)
				{
					if(SkillEffect.ADD.Equals(this.effect[j]))
					{
						cs[i].AddEffect(j, cs[i]);
					}
					else if(SkillEffect.REMOVE.Equals(this.effect[j]))
					{
						cs[i].RemoveEffect(j);
					}
				}
			}
		}
		else
		{
			Character c = GameHandler.Party().GetCharacter(this.characterID);
			if(c != null)
			{
				for(int j=0; j<this.effect.Length; j++)
				{
					if(SkillEffect.ADD.Equals(this.effect[j]))
					{
						c.AddEffect(j, c);
					}
					else if(SkillEffect.REMOVE.Equals(this.effect[j]))
					{
						c.RemoveEffect(j);
					}
				}
			}
		}
		gameEvent.StepFinished(this.next);
	}
	
	public override Hashtable GetData()
	{
		Hashtable ht = base.GetData();
		ht.Add("character", this.characterID.ToString());
		ht.Add("show", this.show.ToString());
		ht.Add("show2", this.show2.ToString());
		
		ArrayList subs = new ArrayList();
		for(int i=0; i<this.effect.Length; i++)
		{
			if(!SkillEffect.NONE.Equals(this.effect[i]))
			{
				Hashtable s = new Hashtable();
				s.Add(XMLHandler.NODE_NAME, "statuseffect");
				s.Add("id", i.ToString());
				s.Add("effect", this.effect[i].ToString());
				subs.Add(s);
			}
		}
		ht.Add(XMLHandler.NODES, subs);
		return ht;
	}
}

public class CheckStatusEffectStep : EventStep
{
	public CheckStatusEffectStep(GameEventType t) : base(t)
	{
		
	}
	
	public override void Execute(GameEvent gameEvent)
	{
		bool check = false;
		
		if(this.show)
		{
			Character[] cs = null;
			if(this.show2)
			{
				cs = GameHandler.Party().GetBattleParty();
			}
			else
			{
				cs = GameHandler.Party().GetParty();
			}
			
			check = true;
			for(int i=0; i<cs.Length; i++)
			{
				if(cs[i] != null)
				{
					if(!cs[i].IsEffectSet(this.number))
					{
						check = false;
						break;
					}
				}
			}
		}
		else
		{
			Character c = GameHandler.Party().GetCharacter(this.characterID);
			if(c != null)
			{
				check = c.IsEffectSet(this.number);
			}
		}
		
		if(check == this.show3)
		{
			gameEvent.StepFinished(this.next);
		}
		else
		{
			gameEvent.StepFinished(this.nextFail);
		}
	}
	
	public override Hashtable GetData()
	{
		Hashtable ht = base.GetData();
		ht.Add("number", this.number.ToString());
		ht.Add("show", this.show.ToString());
		ht.Add("show2", this.show2.ToString());
		ht.Add("show3", this.show3.ToString());
		ht.Add("nextfail", this.nextFail.ToString());
		return ht;
	}
}
